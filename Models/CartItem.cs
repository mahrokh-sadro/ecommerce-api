using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public string ShoppingCartId { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
