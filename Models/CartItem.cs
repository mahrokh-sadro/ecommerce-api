using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public string ShoppingCartId { get; set; } = null!;

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string PictureUrl { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
