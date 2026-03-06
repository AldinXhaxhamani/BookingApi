using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;


namespace Booking.Application.Users.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {

        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                    .WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                    .WithMessage("New password is required.")
                .MinimumLength(6)
                    .WithMessage("New password must be at least 6 characters.")
                .Matches("[0-9]")
                    .WithMessage("New password must contain at least one number.")
                .Matches("[!@#$%^&*(),.?\":{}|<>]")
                    .WithMessage("New password must contain at least one special character.")
                .NotEqual(x => x.CurrentPassword)
                    .WithMessage("New password must be different from current password.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                    .WithMessage("Confirm password is required.")
                .Equal(x => x.NewPassword)
                    .WithMessage("Passwords do not match.");
        }

    }
}
