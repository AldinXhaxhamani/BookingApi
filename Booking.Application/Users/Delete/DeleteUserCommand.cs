using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.Delete
{
    public class DeleteUserCommand : IRequest

    {
        public string TargetEmail { get; set; } = string.Empty;
        public Guid AdminId { get; set; } // perdorim adminId qe gjate fshirjes,admini mos te fshije veten 
                                           // AdminId do te merret nga JWT
    }
}
