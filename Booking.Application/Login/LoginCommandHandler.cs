using Booking.Application.Dtos;
using Booking.Application.Users;
using Booking.Domain.Users;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly LoginCommandValidator _validator;



        public LoginCommandHandler(
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService )
        {

            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _validator = new LoginCommandValidator();

        }
           

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);



            var user = await _userRepository.GetByEmailWithRolesAsync(
                request.Email, cancellationToken);


            //nga useri kontrollojme passwordin(ne fillim e hashojme dhe e kontrollojme te hashuar)
            if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(
                request.Password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException(
                    "Your account has been deactivated. Please contact administrator.");

            var token = _jwtTokenService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                Roles = user.UserRoles.Select(ur => ur.Role.Name),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}
