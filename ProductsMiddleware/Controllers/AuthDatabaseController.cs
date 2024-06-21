using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductsMiddleware.Models.Domain;
using ProductsMiddleware.Models.Dto;
using ProductsMiddleware.Repositories;

namespace ProductsMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthDatabaseController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepostory tokenRepostory;

        public AuthDatabaseController(UserManager<IdentityUser> userManager, ITokenRepostory tokenRepostory)
        {
            this.userManager = userManager;
            this.tokenRepostory = tokenRepostory;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (ModelState.IsValid)
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
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(loginRequestDto.username);

                if (user != null)
                {
                    var checkResult = await userManager.CheckPasswordAsync(user, loginRequestDto.password);

                    if (checkResult)
                    {
                        var roles = await userManager.GetRolesAsync(user);

                        if (roles != null)
                        {
                            var jwt = tokenRepostory.CreateJWTToken(user, roles.ToList());

                            var response = new LoginResponseDto
                            {
                                JwtToken = jwt
                            };

                            return Ok(response);
                        }
                    }
                }
                return BadRequest("Username or password incorrect!");
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
