using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsMiddleware.Models.Domain;
using ProductsMiddleware.Models.Dto;

namespace ProductsMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductsApiController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

            if (responseBody?.Products != null)
            {
                var productDTOs = responseBody.Products.Select(p => new ProductDto
                {
                    Thumbnail = p.Thumbnail,
                    Title = p.Title,
                    Price = p.Price,
                    Description = string.Join("", p.Description.Take(100))
                }).ToList();

                return Ok(productDTOs);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/" + id);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<Product>();
            return Ok(responseBody);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsFilterCategoryAndPrice([FromQuery] string? filterCategory, [FromQuery] decimal? filterMinPrice, [FromQuery] decimal? filterMaxPrice)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

            if (responseBody?.Products == null)
            {
                return NotFound();
            }

            var filteredProducts = responseBody.Products.AsQueryable();

            if (!string.IsNullOrEmpty(filterCategory))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Category.Contains(filterCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (filterMinPrice.HasValue && filterMinPrice > 0)
            {
                filteredProducts = filteredProducts.Where(p => p.Price > filterMinPrice);
            }

            if (filterMaxPrice.HasValue && filterMaxPrice > 0)
            {
                filteredProducts = filteredProducts.Where(p => p.Price < filterMaxPrice);
            }

            var result = filteredProducts.ToList();

            if (!result.Any())
            {
                return NotFound();
            }

            return Ok(filteredProducts);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductsByName([FromQuery] string? filterName)
        {

            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/");

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

            if (responseBody?.Products == null)
            {
                return NotFound();
            }

            var filteredProducts = responseBody.Products;

            if (!string.IsNullOrEmpty(filterName))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Title.Contains(filterName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!filteredProducts.Any())
            {
                return NotFound();
            }

            return Ok(filteredProducts);
        }
    }
}
