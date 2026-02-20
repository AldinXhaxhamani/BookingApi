
using MediatR;
using Booking.Domain.Users;

namespace Booking.Application.Users.Register
{
    public class RegisterUserCommand :IRequest<Guid> 
    {                                                   
        public CreateUserDto createUserDto {  get; set; }
    }
}
