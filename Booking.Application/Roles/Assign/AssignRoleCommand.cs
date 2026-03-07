using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Roles.Assign
{
    public class AssignRoleCommand : IRequest
    {
        public Guid TargetUserId { get; set; } // useri qe do i japim rolin
        public string RoleName { get; set; } = string.Empty;

    }
}
