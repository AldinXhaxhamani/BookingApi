
using FluentValidation;


namespace Booking.Application.Apartaments.Update
{
    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {

        public UpdatePropertyCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty()
                    .WithMessage("Property name is required.")
                .MaximumLength(100)
                    .WithMessage("Property name cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(1000)
                    .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Dto.MaxGuests)
                .GreaterThan(0)
                    .WithMessage("Max guests must be at least 1.")
                .LessThanOrEqualTo(20)
                    .WithMessage("Max guests cannot exceed 20.");

            RuleFor(x => x.Dto.CheckInHour)
                .InclusiveBetween(0, 23)
                    .WithMessage("Check-in hour must be between 0 and 23.");

            RuleFor(x => x.Dto.CheckOutHour)
                .InclusiveBetween(0, 23)
                    .WithMessage("Check-out hour must be between 0 and 23.");

            RuleFor(x => x.Dto.PricePerNight)
                .GreaterThan(0)
                    .WithMessage("Price per night must be greater than 0.");

            RuleFor(x => x.Dto.Amenities)
                .NotNull()
                    .WithMessage("Amenities list cannot be null.");

            RuleFor(x => x.Dto.Country)
                .NotEmpty()
                    .WithMessage("Country is required.")
                .MaximumLength(100)
                    .WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.Dto.City)
                .NotEmpty()
                    .WithMessage("City is required.")
                .MaximumLength(100)
                    .WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Street)
                .NotEmpty()
                    .WithMessage("Street is required.")
                .MaximumLength(200)
                    .WithMessage("Street cannot exceed 200 characters.");

            RuleFor(x => x.Dto.PostalCode)
                .NotEmpty()
                    .WithMessage("Postal code is required.")
                .MaximumLength(10)
                    .WithMessage("Postal code cannot exceed 10 characters.");
        }

    }
}
