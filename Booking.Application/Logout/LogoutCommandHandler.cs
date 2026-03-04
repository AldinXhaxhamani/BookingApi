using Booking.Domain.Entities;
using Booking.Infrastructure;
using MediatR;
using Booking.Application.Logout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITokenBlacklistRepository _blacklistRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LogoutCommandHandler(
            ITokenBlacklistRepository blacklistRepository,
            IJwtTokenService jwtTokenService)

            {
                _blacklistRepository = blacklistRepository;
                _jwtTokenService = jwtTokenService;
            }

        public async Task Handle(LogoutCommand request, CancellationToken ct)
        {
           
            var blacklistedToken = BlacklistedToken.Create(
                request.AccessToken,
                _jwtTokenService.GetExpiration()
            );

            
            await _blacklistRepository.AddAsync(blacklistedToken, ct);
        }
    }
}
