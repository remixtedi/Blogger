using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Models;
using Blogger.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ILogger<AuthController> logger, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Auth([FromBody] Login loginRequest)
    {
        var result = await signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            logger.LogInformation("User logged in.");
            var user = await userManager.FindByEmailAsync(loginRequest.Email);
            var token = await tokenService.GenerateAsync(user);
            return Ok(token);
        }
        else if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return Unauthorized("Error: User account locked out.");
        }
        else
        {
            logger.LogWarning("Invalid login attempt.");
            return Unauthorized("Error: Invalid login attempt.");
        }
    }
}