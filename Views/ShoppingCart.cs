using System;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Views
{
    public class ShoppingCart
    {
        public string Id { get; set; } = null!;  // unique Cart ID 

        public List<CartItem> CartItems { get; set; } = new(); // stored in Redis
              
        public string? ClientSecret { get; set; }
       
        public string? PaymentIntentId { get; set; }

        public int? DeliveryMethodId { get; set; }


    }
}
