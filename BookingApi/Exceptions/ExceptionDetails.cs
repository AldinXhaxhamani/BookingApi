namespace BookingApi.Exceptions
{
    public class ExceptionDetails
    {
        public int StatusCode { get; set; } //statusi eshte numer (401,403,400)
        public string Title { get; set; }=string.Empty;// Unauthorized,Not Found, ValidatinError etc.
        public string Message { get; set; }= string.Empty;// nje mesazh qe e krijojme ne ne cdo rast


    }
}
