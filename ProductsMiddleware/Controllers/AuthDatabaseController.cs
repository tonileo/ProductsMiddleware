using System.Net.Http;
using Azure;
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
        private readonly ILogger<AuthDatabaseController> logger;

        public AuthDatabaseController(UserManager<IdentityUser> userManager, ITokenRepostory tokenRepostory, ILogger<AuthDatabaseController> logger)
        {
            this.userManager = userManager;
            this.tokenRepostory = tokenRepostory;
            this.logger = logger;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            try
            {
                logger.LogInformation("Register started");
                if (ModelState.IsValid)
                {
                    var identityUser = new IdentityUser
                    {
                        UserName = registerRequestDto.Username,
                    };

                    var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
                    logger.LogInformation($"Identity result: {identityResult}");

                    if (identityResult.Succeeded && registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                    {
                        logger.LogInformation("Identity result succeeded and roles are not empty or null");
                        identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                        if (identityResult.Succeeded)
                        {
                            logger.LogInformation("Register finished");
                            return Ok("User successfully registered, now you can login!");
                        }
                    }
                    logger.LogWarning("Something happend with identity result and roles");
                    return BadRequest("Something went wrong!");
                }
                else
                {
                    logger.LogWarning("Model invalid");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                logger.LogInformation("Login started");
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByNameAsync(loginRequestDto.username);

                    if (user != null)
                    {
                        logger.LogInformation($"User not empty: {user}");
                        var checkResult = await userManager.CheckPasswordAsync(user, loginRequestDto.password);

                        if (checkResult)
                        {
                            logger.LogInformation($"CheckResult not empty: {checkResult}");
                            var roles = await userManager.GetRolesAsync(user);

                            if (roles != null)
                            {
                                logger.LogInformation($"Roles not empty: {checkResult}");
                                var jwt = tokenRepostory.CreateJWTToken(user, roles.ToList());

                                var response = new LoginResponseDto
                                {
                                    JwtToken = jwt
                                };

                                logger.LogInformation($"Login finished: {response}");
                                return Ok(response);
                            }
                        }
                    }
                    logger.LogWarning("Username or password incorrect");
                    return BadRequest("Username or password incorrect!");
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
    }
}
