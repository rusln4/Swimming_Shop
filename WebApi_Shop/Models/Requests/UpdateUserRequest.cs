namespace WebApi_Shop.Models.Requests
{
    public class UpdateUserRequest
    {
        public int? IdUsers { get; set; }
        public string? NameUser { get; set; }
        public string? LastnameUser { get; set; }
        public string? AddressUser { get; set; }
        public string? PasswordUser { get; set; }
    }
}