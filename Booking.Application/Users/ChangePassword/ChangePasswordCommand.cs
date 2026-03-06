using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public Guid UserId { get; set; }                           //nga tokeni   
        public string CurrentPassword { get; set; } = string.Empty;  //nga databaza
        public string NewPassword { get; set; } = string.Empty;      
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
