using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class CartItem
{
    public int IdCartItem { get; set; }

    public int IdUserCart { get; set; }

    public int IdProductCart { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Product IdProductCartNavigation { get; set; } = null!;

    public virtual User IdUserCartNavigation { get; set; } = null!;
}
