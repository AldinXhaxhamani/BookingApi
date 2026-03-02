using BCrypt.Net;
using Booking.Domain.Users;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace Booking.Application.Users.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid> 
    {

        private readonly IUserRepository _userRepository;
        private readonly RegisterUserCommandValidation _validator;
        private readonly IApplicationContext _applicationContext;

        public RegisterUserCommandHandler(
            IUserRepository userRepository
            //IApplicationContext applicationContext
            )
        {
            _userRepository = userRepository;
            _validator = new RegisterUserCommandValidation();
           //_applicationContext = applicationContext;
        }

        
        public async Task<Guid>  Handle(        //implementi i metodes Handele te interfaceit  IRequestHandler<RegisterUserCommand, Guid> 
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {

            var validationResult = await _validator.ValidateAsync(
            request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);


            var emailExists = await _userRepository.ExistsWithEmailAsync(
                request.createUserDto.Email, cancellationToken);

            if (emailExists)
                throw new InvalidOperationException( 
                    $"Email '{request.createUserDto.Email}' is already registered.");


            var password = request.createUserDto.Password;

           request.createUserDto.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password,13);
          
            var user = User.CreateUser(request.createUserDto);
           
            await _userRepository.Add(user);

            await _userRepository.SaveChangesAsync();

            //await _applicationContext.SaveChangesAsync(); 
            return user.Id;


        }





    }





}
