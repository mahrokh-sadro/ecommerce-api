using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts(string? brands, string? types, string? sort, string? searchTerm);
        Task<Product?> GetProductById(int id);
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(int id, Product product);
        Task<bool> DeleteProduct(int id);
        Task<IEnumerable<string>> GetBrands();
        Task<IEnumerable<string>> GetTypes();
    }
}
