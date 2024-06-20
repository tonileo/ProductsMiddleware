using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductsMiddleware.Models.Domain;
using ProductsMiddleware.Models.Dto;

namespace ProductsMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(IHttpClientFactory httpClientFactory, UserManager<IdentityUser> userManager)
        {
            this.httpClientFactory = httpClientFactory;
            this.userManager = userManager;
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

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
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

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded && registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                if (identityResult.Succeeded)
                {
                    return Ok("User successfully registered, now you can login!");
                }
            }

            return BadRequest("Something went wrong!");
        }
    }
}
