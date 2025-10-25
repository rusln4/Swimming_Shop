using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class Category
{
    public int IdCategories { get; set; }

    public string NameCategory { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
