using Booking.Application.Users;
using Booking.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Roles.Assign
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public AssignRoleCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task Handle(AssignRoleCommand request, CancellationToken ct)
        {
            //ngarkojme nga databaza userin sebashku me rolet 
            var user = await _userRepository.GetByIdWithRolesAsync(
                request.TargetUserId, ct);

            if (user is null)
                throw new KeyNotFoundException(
                    "User not found.");

            //kontrollojme nese roli ndodhet ne db
            //mund edhe te mos  e vendosim sepse i kam shtuar rolet manualisht ne db
            var role = await _roleRepository.GetByNameAsync(request.RoleName, ct);

            if (role is null)
                throw new KeyNotFoundException(
                    $"Role '{request.RoleName}' not found.");

            
            //kontrollojme nese useri e ka kete rol assign nga me perpara
            var alreadyHasRole = user.UserRoles
                .Any(ur => ur.Role.Name == request.RoleName);

            if (alreadyHasRole)
                throw new InvalidOperationException(
                    $"User already has the '{request.RoleName}' role.");

            //i japim rolit duke ruajtur userId dhe RoleId ne tabelen UserRole
            var userRole = new UserRole(user.Id, role.Id);
            user.UserRoles.Add(userRole);

            await _userRepository.SaveChangesAsync();

        }
    }
}
