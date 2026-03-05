using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.Update
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {

        public UpdateUserCommandValidator() {

            RuleFor(x => x.updateUserDto.FirstName)
           .NotEmpty()
               .WithMessage("First name is required.")
           .MaximumLength(50)
               .WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.updateUserDto.LastName)
                .NotEmpty()
                    .WithMessage("Last name is required.")
                .MaximumLength(50)
                    .WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.updateUserDto.PhoneNumber)
                .NotEmpty()
                    .WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{9,15}$")
                    .WithMessage("Phone number format is invalid.");
        

        }
    }
}
