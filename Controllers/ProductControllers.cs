using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using WebApplication1.Models;
//using WebApplication1.Services;
using AppContext = WebApplication1.Models.AppContext;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppContext _dbContext;
        public ProductController(AppContext appContext)
        {
            _dbContext = appContext;
        }

        [HttpGet]
        [Route("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brands,string? types)
        {
            try
            {
                var query=_dbContext.Products.AsQueryable();
                if (!string.IsNullOrWhiteSpace(brands))
                {
                    var brandList=brands.Split(',').Select(b=>b.Trim()).ToList();   
                    query=query.Where(p=>brandList.Contains(p.Brand));
                }

                if (!string.IsNullOrWhiteSpace(types)) { 
                    var typeList=types.Split(',').Select(t=>t.Trim()).ToList(); 
                    query=query.Where(p=>types.Contains(p.Type));   
                }

                var products = await query.ToListAsync();
                return products;

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        //add product
        //[FromBody]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        //update 
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || ProductExists(id)) {
                return BadRequest("Update Failed");
            }

            _dbContext.Entry(product).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok();

        }

        public bool ProductExists(int id)
        {
            return _dbContext.Products.Any(p => p.Id == id);
        }
        //delete
        //[HttpDelete("{id:int}")]
        //public async Task<ActionResult> DeleteProduct(int id)
        //{
        //    var product = _dbContext.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _dbContext.Products.Remove(await product);
        //    await _dbContext.SaveChangesAsync();
        //}



        //getproductbyid
        public async Task<Product> GetProductById(int id)
        {
            return await _dbContext.Products.FindAsync(id); 
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<String>>> GetBrands()
        {
            var brands=await _dbContext.Products.Select( p => p.Brand).Distinct().ToListAsync();
            return brands;
        }
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<String>>> GetTypes()
        {
            var types = await _dbContext.Products.Select( p => p.Type).Distinct().ToListAsync();
            return types;
        }
    }
}
