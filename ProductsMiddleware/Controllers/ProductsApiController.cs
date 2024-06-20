﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [Route("{id}")]
        public async Task<IActionResult> GetProduct (int id)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/products/" + id);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(responseBody);
        }
    }
}
