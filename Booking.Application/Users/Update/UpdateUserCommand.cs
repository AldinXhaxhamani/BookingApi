using Booking.Application.Dtos;
using Booking.Domain.Users;
using MediatR;


namespace Booking.Application.Users.Update
{
    public class UpdateUserCommand : IRequest<UserProfileDto> //kthen vlerat e update-uara
    {

        public Guid UserId { get; set; } // duhet te merret nga tokeni
        public UpdateUserDto updateUserDto { get; set; } = new();

    }   
}
