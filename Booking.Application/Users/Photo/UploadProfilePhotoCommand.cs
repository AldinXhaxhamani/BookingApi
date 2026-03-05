using MediatR;


namespace Booking.Application.Users.Photo
{
    public class UploadProfilePhotoCommand : IRequest<string>
    {

        public Guid UserId { get; set; }
        public Stream FileStream { get; set; } = Stream.Null; //bytet e fotos se upload-uar
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; //ruan llojin e filet qe behet upload

    }
}
