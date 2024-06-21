using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<ProductsApiController> logger;

        public ProductsApiController(IHttpClientFactory httpClientFactory, ILogger<ProductsApiController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                logger.LogInformation("Get All Products started");
                var client = httpClientFactory.CreateClient();

                var response = await client.GetAsync("https://dummyjson.com/products/");
                logger.LogInformation($"Response from client: {response}");
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

                logger.LogInformation($"Response from ProductList with data: {JsonSerializer.Serialize(responseBody)}");

                if (responseBody?.Products != null)
                {
                    var productDTOs = responseBody.Products.Select(p => new ProductDto
                    {
                        Thumbnail = p.Thumbnail,
                        Title = p.Title,
                        Price = p.Price,
                        Description = string.Join("", p.Description.Take(100))
                    }).ToList();

                    logger.LogInformation("Get all products finished");
                    return Ok(productDTOs);
                }
                else
                {
                    logger.LogWarning("Couldn't find the products");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                logger.LogInformation("Get Product started");
                var client = httpClientFactory.CreateClient();

                var response = await client.GetAsync("https://dummyjson.com/products/" + id);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<Product>();
                logger.LogInformation("Get product finished");
                return Ok(responseBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }

        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsFilterCategoryAndPrice([FromQuery] string? filterCategory, [FromQuery] decimal? filterMinPrice, [FromQuery] decimal? filterMaxPrice)
        {
            try
            {
                logger.LogInformation("GetProductsFilterCategoryAndPrice started");
                var client = httpClientFactory.CreateClient();

                var response = await client.GetAsync("https://dummyjson.com/products/");
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

                if (responseBody?.Products == null)
                {
                    logger.LogInformation("ResponseBody == null");
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
                    logger.LogWarning("Result doesn't contain elements");
                    return NotFound();
                }

                logger.LogInformation("GetProductsFilterCategoryAndPrice finished");
                return Ok(filteredProducts);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductsByName([FromQuery] string? filterName)
        {
            try
            {
                logger.LogInformation("GetProductsByName started");
                var client = httpClientFactory.CreateClient();

                var response = await client.GetAsync("https://dummyjson.com/products/");

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();

                if (responseBody?.Products == null)
                {
                    logger.LogInformation("ResponseBody == null");
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
                    logger.LogWarning("Result doesn't contain elements");
                    return NotFound();
                }

                logger.LogInformation("GetProductsByName finished");
                return Ok(filteredProducts);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
