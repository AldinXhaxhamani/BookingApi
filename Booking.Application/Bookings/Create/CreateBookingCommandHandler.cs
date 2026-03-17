using Booking.Application.Apartaments;
using Booking.Application.Email;
using Booking.Domain.Bookings;
using Booking.Domain.Bookings.Dtos;
using Booking.Domain.Email;
using Booking.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Bookings.Create
{
    public class CreateBookingCommandHandler
        : IRequestHandler<CreateBookingCommand, CreateBookingResponseDto>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailService _emailService;              
        private readonly IApplicationContext _context;


        private const decimal CleaningFeeAmount = 25m;
        private const decimal ServiceFeeRate = 0.10m;  
        private const decimal TaxRate = 0.08m;  

        public CreateBookingCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository,
            IBookingRepository bookingRepository,
            IEmailService emailService,                          
            IApplicationContext context)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
            _bookingRepository = bookingRepository;
            _emailService = emailService;
            _context = context; 
        }

        public async Task<CreateBookingResponseDto> Handle(
            CreateBookingCommand request, CancellationToken ct)
        {
            var checkIn = request.CheckIn.Date;
            var checkOut = request.CheckOut.Date;
            var nights = (checkOut - checkIn).Days;

            // Validate input 
            if (checkIn < DateTime.UtcNow.Date)
                throw new InvalidOperationException(
                    "Check-in date cannot be in the past.");

            if (checkOut <= checkIn)
                throw new InvalidOperationException(
                    "Check-out must be after check-in.");

            if (nights < 1)
                throw new InvalidOperationException(
                    "Stay must be at least 1 night.");

            if (request.GuestCount < 1)
                throw new InvalidOperationException(
                    "At least 1 guest is required.");

            // Load property
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (!property.IsActive || !property.IsApproved)
                throw new InvalidOperationException(
                    "This property is not available for booking.");

            // Validate guests 
            if (request.GuestCount > property.MaxGuests)
                throw new InvalidOperationException(
                    $"This property allows a maximum of " +
                    $"{property.MaxGuests} guests.");

            // Validate stay duration
            if (nights < property.MinimumStayNights)
                throw new InvalidOperationException(
                    $"Minimum stay for this property is " +
                    $"{property.MinimumStayNights} nights.");

            if (nights > property.MaximumStayNights)
                throw new InvalidOperationException(
                    $"Maximum stay for this property is " +
                    $"{property.MaximumStayNights} nights.");

            // Check availability 
            var isAvailable = await _availabilityRepository
                .AreDatesAvailableAsync(
                    request.PropertyId, checkIn, checkOut, ct);

            if (!isAvailable)
                throw new InvalidOperationException(
                    "The selected dates are not available.");

            // Calculate price
            // get season price if exists — otherwise use base price
            var season = SeasonHelper.GetSeason(checkIn);
            var seasonPrice = await _availabilityRepository
                .GetSeasonPriceAsync(request.PropertyId, season, ct);

            var pricePerNight = seasonPrice?.PricePerNight
                                 ?? property.PricePerNight;

            var priceForPeriod = pricePerNight * nights;

            // apply discount if guest qualifies
            var discount = await _availabilityRepository
                .GetDiscountAsync(request.PropertyId, ct);

            var discountAmount = 0m;
            if (discount is not null && nights >= discount.MinimumNights)
                discountAmount = discount.DiscountPerNight * nights;

            var discountedPeriodPrice = priceForPeriod - discountAmount;

            // amenities upcharge — Pool=5, Gym=6, Spa=7 → $10 each
            var amenitiesUpCharge = CalculateAmenitiesUpCharge(
                property.AmenitiesRaw);

            // subtotal = discounted price + cleaning + amenities
            var subtotal = discountedPeriodPrice +
                           CleaningFeeAmount +
                           amenitiesUpCharge;

            // service fee = 10% of discounted period price
            var serviceFee = Math.Round(
                discountedPeriodPrice * ServiceFeeRate, 2);

            // tax = 8% of subtotal
            var taxAmount = Math.Round(subtotal * TaxRate, 2);

            // total = subtotal + service fee + tax
            var totalPrice = subtotal + serviceFee + taxAmount;



            // Create booking entity 
            var booking = BookingEntity.Create(
                propertyId: request.PropertyId,
                guestId: request.GuestId,
                startDate: checkIn,
                endDate: checkOut,
                guestCount: request.GuestCount,
                priceForPeriod: discountedPeriodPrice,
                cleaningFee: CleaningFeeAmount,
                amenitiesUpCharge: amenitiesUpCharge,
                totalPrice: totalPrice);

            // Remove dates from availability
            await _availabilityRepository.RemoveBookedDatesAsync(
                request.PropertyId, checkIn, checkOut, ct);

            // Increment booking count 
            property.IncrementBookingCount();

            //Save everything in one transaction
            await _bookingRepository.AddAsync(booking, ct);
            await _bookingRepository.SaveChangesAsync(ct);


            //dergimi i email tek useri

            var guest = await _context.Users
                .Where(u => u.Id == request.GuestId)
                .Select(u => new { u.Email, u.Name, u.LastName })
                .FirstOrDefaultAsync(ct);

            var propertyAddress = await _context.Addresses
                .Where(a => a.Id == property.AddressId)
                .Select(a => new { a.City, a.Country })
                .FirstOrDefaultAsync(ct);

            if (guest is not null)
            {
                var emailDto = new BookingEmailDto
                {
                    GuestEmail = guest.Email,
                    GuestName = $"{guest.Name} {guest.LastName}",
                    BookingId = booking.Id,
                    PropertyName = property.Name,
                    PropertyCity = propertyAddress?.City ?? string.Empty,
                    PropertyCountry = propertyAddress?.Country ?? string.Empty,
                    CheckIn = booking.StartDate,
                    CheckOut = booking.EndDate,
                    Nights = nights,
                    GuestCount = booking.GuestCount,
                    PriceForPeriod = booking.PriceForPeriod,
                    CleaningFee = booking.CleaningFee,
                    AmenitiesUpCharge = booking.AmenitiesUpCharge,
                    ServiceFee = serviceFee,
                    TaxAmount = taxAmount,
                    TotalPrice = booking.TotalPrice,
                    ExpiresAtUtc = booking.ExpiresAtUtc!.Value
                };

                await _emailService
                    .SendBookingConfirmationToGuestAsync(emailDto, ct);
            }



            // response to the client
            return new CreateBookingResponseDto
            {
                BookingId = booking.Id,
                Status = booking.BookingStatus.ToString(),
                CheckIn = booking.StartDate,
                CheckOut = booking.EndDate,
                Nights = nights,
                GuestCount = booking.GuestCount,
                PriceForPeriod = booking.PriceForPeriod,
                CleaningFee = booking.CleaningFee,
                AmenitiesUpCharge = booking.AmenitiesUpCharge,
                ServiceFee = serviceFee,
                TaxAmount = taxAmount,
                TotalPrice = booking.TotalPrice,
                ExpiresAtUtc = booking.ExpiresAtUtc!.Value,
                Message = "Booking created successfully. " +
                                    "Awaiting owner confirmation."
            };
        }

        
       
        private static decimal CalculateAmenitiesUpCharge(string? amenitiesRaw)
        {
            if (string.IsNullOrEmpty(amenitiesRaw))
                return 0m;

            var premiumAmenities = new HashSet<int> { 5, 6, 7 };

            var count = amenitiesRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => int.TryParse(a, out var id) ? id : 0)
                .Count(id => premiumAmenities.Contains(id));

            return count * 10m;
        }
    }

}

