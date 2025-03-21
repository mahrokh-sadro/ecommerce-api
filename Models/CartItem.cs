using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
