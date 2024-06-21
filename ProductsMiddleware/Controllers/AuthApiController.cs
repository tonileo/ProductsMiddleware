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
        private readonly ILogger<AuthApiController> logger;

        public AuthApiController(IHttpClientFactory httpClientFactory, ILogger<AuthApiController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                logger.LogInformation("GetAllUsers started");
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
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApiRequestDto loginDto)
        {
            try
            {
                logger.LogInformation("Login started");
                if (ModelState.IsValid)
                {
                    var client = httpClientFactory.CreateClient();

                    var requestContent = new StringContent(JsonSerializer.Serialize(loginDto), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://dummyjson.com/auth/login", requestContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        logger.LogWarning($"Response: {response}");
                        var errorContent = await response.Content.ReadAsStringAsync();

                        return StatusCode((int)response.StatusCode, errorContent);
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    logger.LogInformation($"Login finished: {jsonResponse}");
                    return Ok(jsonResponse);
                }
                else
                {
                    logger.LogWarning("Model not valid");
                    return BadRequest("Model not valid");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("loginDifferentWay")]
        public async Task<IActionResult> LoginApi([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                logger.LogInformation("LoginApi started");
                if (ModelState.IsValid)
                {
                    var client = httpClientFactory.CreateClient();
                    var response = await client.GetAsync("https://dummyjson.com/users/");
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadFromJsonAsync<UsersList>();
                    if (responseBody?.Users != null)
                    {
                        logger.LogInformation($"Response.Users not null: {responseBody?.Users}");
                        var user = responseBody.Users.FirstOrDefault(u => u.username == loginRequestDto.username && u.password == loginRequestDto.password);
                        if (user != null)
                        {
                            logger.LogInformation("Login finished");
                            return Ok("User successfully loged in!");
                        }
                        else
                        {
                            logger.LogWarning("Username or password are incorrect");
                            return BadRequest("Username or password are incorrect!");
                        }
                    }
                    else
                    {
                        logger.LogWarning("Response.Users is null");
                        return NotFound();
                    }
                }
                else
                {
                    logger.LogWarning("Model not valid");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
