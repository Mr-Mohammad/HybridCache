using HybridCache.CacheService;
using HybridCache.Models;
using Microsoft.AspNetCore.Mvc;

namespace HybridCache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IHybridCache _hybridCache;

        public ProductsController(IHybridCache hybridCache)
        {
            _hybridCache = hybridCache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            string cacheKey = $"Product-{id}";

            // Try to get product from cache
            var product = await _hybridCache.GetAsync<Product>(cacheKey);
            if (product != null)
            {
                return Ok(product);
            }

            // Simulate fetching data from a database
            product = new Product { Id = id, Name = $"Product {id}", Price = id * 10 };

            // Cache the product
            await _hybridCache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));

            return Ok(product);
        }
    }

}
