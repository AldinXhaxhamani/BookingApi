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
        //private readonly IApplicationContext _applicationContext;

        public RegisterUserCommandHandler(
            IUserRepository userRepository
            //IApplicationContext applicationContext
            )
        {
            _userRepository = userRepository;
           // _applicationContext = applicationContext;
        }

        
        public async Task<Guid>  Handle(        //implementi i metodes Handele te interfaceit  IRequestHandler<RegisterUserCommand, Guid> 
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("1) entered handler");

            var password= request.createUserDto.Password;

           // request.createUserDto.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password,13);
            Console.WriteLine("2) after request password");

            Console.WriteLine("3) para krijimit te user");
            var user = User.CreateUser(request.createUserDto);
            Console.WriteLine("4) pas krijimit te userit");


            Console.WriteLine("5) before add");
            await _userRepository.Add(user);
            Console.WriteLine("6) after add");

            Console.WriteLine("7) before save");

            await _userRepository.SaveChangesAsync();

           // await _applicationContext.SaveChangesAsync();

            Console.WriteLine("8) after save    ");

            return user.Id;


        }





    }





}
