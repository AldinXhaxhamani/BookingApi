using MediatR;
using Microsoft.AspNetCore.Hosting;
using Booking.Domain.Users;



namespace Booking.Application.Users.Photo
{
    public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public UploadProfilePhotoCommandHandler(
            IUserRepository userRepository,
            IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
        }

        public async Task<string> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
        {

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };//vetem keto 3 formate pranojme per foton
            if (!allowedTypes.Contains(request.ContentType))
                throw new InvalidOperationException(
                    "Only JPEG, PNG and WebP images are allowed.");

            if (request.FileStream.Length > 5 * 1024 * 1024) //vendosim madhesine maksimale te fotots 5MB
                throw new InvalidOperationException(
                    "File size cannot exceed 5MB.");


            var user = await _userRepository.GetByIdWithRolesAsync(request.UserId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldFilePath = Path.Combine(
                    _environment.WebRootPath,
                    user.ProfileImageUrl.TrimStart('/'));

                if (File.Exists(oldFilePath))  
                    File.Delete(oldFilePath);//nese kemi nje url(foto) te vjeter, e fshijme
            }

            var extension = Path.GetExtension(request.FileName);
            var uniqueFileName = $"{request.UserId}_{Guid.NewGuid()}{extension}";



            var uploadFolder = Path.Combine(
                _environment.WebRootPath, "Uploads", "Profiles"); 



            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // ruajme filet ne  folder
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await request.FileStream.CopyToAsync(fileStream, cancellationToken);

            //ndertojme url e fotos qe do e ruajme ne databaze 
            var photoUrl = $"/uploads/profiles/{uniqueFileName}";

            
            user.UpdateProfilePhoto(photoUrl);
            await _userRepository.SaveChangesAsync();

            return photoUrl;


        }
    }
}
