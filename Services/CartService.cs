﻿using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using WebApplication1.Interfaces;
using WebApplication1.Views;
using AppContext = WebApplication1.Models.AppContext;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly IDatabase _redisDb;
        private readonly AppContext _dbContext;

        public CartService(IConnectionMultiplexer redis, AppContext dbContext)
        {
            _redisDb = redis.GetDatabase();
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteCart(string key)
        {
            return await _redisDb.KeyDeleteAsync(key);
        }

        //public async Task<ShoppingCart?> GetCart(string key)
        //{
        //    var data = await _redisDb.StringGetAsync(key);
        //    if (data.IsNullOrEmpty) return null;
        //    return JsonSerializer.Deserialize<ShoppingCart?>(data!);
        //}

        //public async Task<ShoppingCart?> SetCart(ShoppingCart cart)
        //{
        //    bool isSet = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));
        //    return isSet ? cart : null;
        //}
        // Method to save cart data to Redis
        // Set cart data in Redis
        public async Task<ShoppingCartView?> SetCart(ShoppingCartView cart)
        {
            var jsonData = JsonSerializer.Serialize(cart);
            bool success= await _redisDb.StringSetAsync($"cart:{cart.Id}", jsonData);

            if (success)
            {
                return cart;
            }
            else
            {
                throw new Exception("Failed to save cart to redit");
            }
        }

        public async Task<ShoppingCartView?> GetCart(string cartId)
        {
            var cartData = await _redisDb.StringGetAsync($"cart:{cartId}");
            if (cartData.IsNullOrEmpty) return null;

            var cart = JsonSerializer.Deserialize<ShoppingCartView>(cartData!);

            // Fetch product details from MySQL
            if (cart != null)
            {
                foreach (var item in cart.CartItems)
                {
                    var product = await _dbContext.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductId = product.Id;
                        item.Description = product.Description;
                        item.Name = product.Name;
                        item.Price = product.Price;
                        item.Image = product.Image;
                    }
                }
            }

            return cart;
        }
    }
}
