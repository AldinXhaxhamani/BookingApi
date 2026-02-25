using MediatR;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Text;
using Booking.Domain.Users;


namespace Booking.Application.Users.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid> 
    {

        private readonly IUserRepository _userRepository;
        private readonly IApplicationContext _applicationContext;

        public RegisterUserCommandHandler(
            IUserRepository userRepository
            //IApplicationContext applicationContext
            )
        {
            _userRepository = userRepository;
           //_applicationContext = applicationContext;
        }

        
        public async Task<Guid>  Handle(        //implementi i metodes Handele te interfaceit  IRequestHandler<RegisterUserCommand, Guid> 
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            
            var password= request.createUserDto.Password;

           request.createUserDto.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password,13);
          
            var user = User.CreateUser(request.createUserDto);
           
            await _userRepository.Add(user);

            await _userRepository.SaveChangesAsync();

            //await _applicationContext.SaveChangesAsync(); 
            return user.Id;


        }





    }





}
