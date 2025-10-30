namespace WebApi_Shop.Models.Requests
{
    public class LoginRequest
    {
        public string MailUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
    }
}