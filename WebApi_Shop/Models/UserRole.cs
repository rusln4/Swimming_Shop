using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class UserRole
{
    public int IdUserRoles { get; set; }

    public string NameUserRole { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
