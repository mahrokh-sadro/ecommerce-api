using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ShoppingCart
{
    public string Id { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
