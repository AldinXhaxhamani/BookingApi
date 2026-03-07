using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Users.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {

            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken ct)
        {
            
            var user = await _userRepository.GetByEmailWithRolesAsync(
                request.TargetEmail, ct);

            
            if (user is null)
                throw new KeyNotFoundException(
                    $"User with email '{request.TargetEmail}' not found.");

            
            if (!user.IsActive)
                throw new InvalidOperationException(
                    "User is already deactivated.");

            //ketu vendosim kusht qe admini nuk fshin dot veten
            if (user.Id == request.AdminId)
                throw new InvalidOperationException(
                    "You cannot deactivate your own account.");

            
            user.Deactivate();

            
            await _userRepository.SaveChangesAsync();
        }

    }
}
