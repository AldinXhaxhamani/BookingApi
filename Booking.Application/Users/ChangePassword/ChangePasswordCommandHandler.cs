using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {

        private readonly IUserRepository _userRepository;
        private readonly ChangePasswordCommandValidator _validator;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _validator = new ChangePasswordCommandValidator();
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            
            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, ct);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            
            var currentPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(
                request.CurrentPassword, user.Password);

            if (!currentPasswordValid)
                throw new UnauthorizedAccessException(
                    "Current password is incorrect.");

            
            var newPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(
                request.NewPassword, 13);

            
            user.ChangePassword(newPasswordHash);

            
            await _userRepository.SaveChangesAsync();
        }

    }
}
