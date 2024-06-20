﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsMiddleware.Models.Domain;

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

        [HttpGet] //TO DO: Category is extra and description should be max 100
        public async Task<IActionResult> GetAllProducts()
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<ProductList>();
            return Ok(responseBody);
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
        public async Task<IActionResult> GetProductsFilterCategoryAndPrice([FromQuery] string filterCategory, [FromQuery] decimal filterMinPrice, [FromQuery] decimal filterMaxPrice)
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

            if (!string.IsNullOrEmpty(filterCategory) || decimal.IsPositive(filterMinPrice) || decimal.IsPositive(filterMaxPrice))
            {
                filteredProducts = filteredProducts
                    .Where(p => p.Category.Contains(filterCategory, StringComparison.OrdinalIgnoreCase) && p.Price > filterMinPrice && p.Price < filterMaxPrice)
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
