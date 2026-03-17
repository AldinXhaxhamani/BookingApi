using Booking.Application.Apartaments;
using Booking.Domain.Bookings.Dtos;
using Booking.Domain.Enum;
using Booking.Domain.Bookings;
using MediatR;
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

        // business rules — fixed at platform level
        private const decimal CleaningFeeAmount = 25m;
        private const decimal ServiceFeeRate = 0.10m;  // 10%
        private const decimal TaxRate = 0.08m;  // 8% on subtotal

        public CreateBookingCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository,
            IBookingRepository bookingRepository)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<CreateBookingResponseDto> Handle(
            CreateBookingCommand request, CancellationToken ct)
        {
            var checkIn = request.CheckIn.Date;
            var checkOut = request.CheckOut.Date;
            var nights = (checkOut - checkIn).Days;

            // ── Step 1: Validate input — no DB calls yet ───────────
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

            // ── Step 2: Load property ──────────────────────────────
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (!property.IsActive || !property.IsApproved)
                throw new InvalidOperationException(
                    "This property is not available for booking.");

            // ── Step 3: Validate guests ────────────────────────────
            if (request.GuestCount > property.MaxGuests)
                throw new InvalidOperationException(
                    $"This property allows a maximum of " +
                    $"{property.MaxGuests} guests.");

            // ── Step 4: Validate stay duration ─────────────────────
            if (nights < property.MinimumStayNights)
                throw new InvalidOperationException(
                    $"Minimum stay for this property is " +
                    $"{property.MinimumStayNights} nights.");

            if (nights > property.MaximumStayNights)
                throw new InvalidOperationException(
                    $"Maximum stay for this property is " +
                    $"{property.MaximumStayNights} nights.");

            // ── Step 5: Check availability ─────────────────────────
            var isAvailable = await _availabilityRepository
                .AreDatesAvailableAsync(
                    request.PropertyId, checkIn, checkOut, ct);

            if (!isAvailable)
                throw new InvalidOperationException(
                    "The selected dates are not available.");

            // ── Step 6: Calculate price ────────────────────────────
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

            // ── Step 7: Create booking entity ──────────────────────
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

            // ── Step 8: Remove dates from availability ─────────────
            // staged before SaveChanges — both succeed or both fail
            await _availabilityRepository.RemoveBookedDatesAsync(
                request.PropertyId, checkIn, checkOut, ct);

            // ── Step 9: Increment booking count ────────────────────
            property.IncrementBookingCount();

            // ── Step 10: Save everything in one transaction ────────
            await _bookingRepository.AddAsync(booking, ct);
            await _bookingRepository.SaveChangesAsync(ct);

            // ── Step 11: Return price breakdown to guest ───────────
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

        
        // Pool=5, Gym=6, Spa=7 → $10 per premium amenity per stay
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

