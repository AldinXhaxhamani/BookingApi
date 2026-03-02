using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.Register
{
    public class RegisterUserCommandValidation : AbstractValidator<RegisterUserCommand>
    {

        public RegisterUserCommandValidation()

        {
            RuleFor(x => x.createUserDto.Password)
            .MinimumLength(6)
            .Matches("[0-9]").WithMessage("Password must contain at least one numeric digit.")
            .Matches("[!@#$%^&*(),.?\":{}|<>]")
            .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.createUserDto.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.createUserDto.FirstName)
                  .NotEmpty().WithMessage("First name is required.")
                  .MaximumLength(30);

            RuleFor(x => x.createUserDto.LastName)
                  .NotEmpty().WithMessage("Last name is required.")
                  .MaximumLength(30);
        }

    }
}
