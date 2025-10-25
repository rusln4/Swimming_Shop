using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public string NameProduct { get; set; } = null!;

    public string DescriptionProduct { get; set; } = null!;

    public decimal PriceProduct { get; set; }

    public int CountProduct { get; set; }

    public int IdCategoryProduct { get; set; }

    public string PictureProduct { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category IdCategoryProductNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
