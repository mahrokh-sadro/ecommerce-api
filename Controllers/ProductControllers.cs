using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Get all products with optional filters
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brands, string? types, string? sort, string? searchTerm)
        {
            try
            {
                var products = await _productService.GetProducts(brands, types, sort, searchTerm);
                return Ok(products);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // Get a single product by its id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // Create a new product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                var newProduct = await _productService.CreateProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // Update an existing product
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var updated = await _productService.UpdateProduct(id, product);
            if (updated)
            {
                return NoContent();
            }
            return BadRequest("Update failed");
        }

        // Delete a product by its id
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (deleted)
            {
                return NoContent();
            }
            return NotFound();
        }

        // Get all distinct brands
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetBrands()
        {
            try
            {
                var brands = await _productService.GetBrands();
                return Ok(brands);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // Get all distinct product types
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetTypes()
        {
            try
            {
                var types = await _productService.GetTypes();
                return Ok(types);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
