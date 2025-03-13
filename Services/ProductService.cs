using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using AppContext = WebApplication1.Models.AppContext;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly AppContext _dbContext;

        public ProductService(AppContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetProducts(string? brands, string? types, string? sort)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brands))
            {
                var brandList = brands.Split(',').Select(b => b.Trim()).ToList();
                query = query.Where(p => brandList.Contains(p.Brand));
            }

            if (!string.IsNullOrWhiteSpace(types))
            {
                var typeList = types.Split(',').Select(t => t.Trim()).ToList();
                query = query.Where(p => typeList.Contains(p.Type));
            }

            query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDes" => query.OrderByDescending(p => p.Price),
                "nameAsc" => query.OrderBy(p => p.Name),
                "nameDsc" => query.OrderByDescending(p => p.Name),
                _ => query
            };

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<Product> CreateProduct(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProduct(int id, Product product)
        {
            if (id != product.Id || !ProductExists(id))
                return false;

            _dbContext.Entry(product).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return false;

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetBrands()
        {
            return await _dbContext.Products.Select(p => p.Brand).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTypes()
        {
            return await _dbContext.Products.Select(p => p.Type).Distinct().ToListAsync();
        }

        private bool ProductExists(int id)
        {
            return _dbContext.Products.Any(p => p.Id == id);
        }
    }
}
