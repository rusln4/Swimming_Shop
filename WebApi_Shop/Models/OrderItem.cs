using System;
using System.Collections.Generic;

namespace WebApi_Shop.Models;

public partial class OrderItem
{
    public int IdOrderItem { get; set; }

    public int IdOrder { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
