using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductsMiddleware.Models.Dto;
using ProductsMiddleware.Repositories;

namespace ProductsMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AuthApiController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync("https://dummyjson.com/users/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<UsersList>();

            if (responseBody?.Users != null)
            {
                return Ok(responseBody.Users);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var client = httpClientFactory.CreateClient();

            var requestContent = new StringContent(JsonSerializer.Serialize(loginDto), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://dummyjson.com/auth/login", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, errorContent);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return Ok(jsonResponse);
        }

        [HttpPost]
        [Route("loginDifferentWay")]
        public async Task<IActionResult> LoginApi([FromBody] LoginRequestDto loginRequestDto)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://dummyjson.com/users/");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<UsersList>();
            if (responseBody?.Users != null)
            {
                var user = responseBody.Users.FirstOrDefault(u => u.Username == loginRequestDto.Username && u.Password == loginRequestDto.Password);
                if (user != null)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Username or paswword are incorrect!");
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
