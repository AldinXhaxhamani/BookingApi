using Booking.Application.Dtos;
using Booking.Domain.Users;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.Update
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserProfileDto>
    {

        private readonly IUserRepository _userRepository;
        private readonly UpdateUserCommandValidator _validator;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _validator = new UpdateUserCommandValidator();
        }

        public async Task<UserProfileDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {

            var validationResult=await _validator.ValidateAsync(request,cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);


            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new KeyNotFoundException("User not found.");



            user.UpdatePersonalInfo(
            request.updateUserDto.FirstName,
            request.updateUserDto.LastName,
            request.updateUserDto.PhoneNumber
            );

            await _userRepository.SaveChangesAsync();

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.Name,
                LastName = user.LastName,
                Email = user.Email,         
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                LastModifiedAt = user.LastModifiedAt  
            };
        }
    }
}
