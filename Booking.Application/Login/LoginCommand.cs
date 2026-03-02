using Booking.Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Login
{
    public  class LoginCommand : IRequest<AuthResponse>
    {

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
