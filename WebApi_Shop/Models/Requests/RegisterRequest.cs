namespace WebApi_Shop.Models.Requests
{
    public class RegisterRequest
    {
        public string MailUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
        public string NameUser { get; set; } = null!;
        public string LastnameUser { get; set; } = null!;
        public string PhoneUser { get; set; } = null!;
        public string AddressUser { get; set; } = null!;
    }
}