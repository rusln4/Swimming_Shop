using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class User
{
    public int IdUsers { get; set; }

    public int RoleUser { get; set; }

    public string MailUser { get; set; } = null!;

    public string PasswordUser { get; set; } = null!;

    public string NameUser { get; set; } = null!;

    public string LastnameUser { get; set; } = null!;

    public string PhoneUser { get; set; } = null!;

    public string AddressUser { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual UserRole RoleUserNavigation { get; set; } = null!;
}
