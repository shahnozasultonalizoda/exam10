using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase 
{
    [HttpPost("register")]
    public async Task<IActionResult> Register( RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login( LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword( ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await authService.ChangePasswordAsync(userId, dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword( ForgotPasswordDto dto)
    {
        var result = await authService.ForgotPasswordAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode( VerifyCodeDto dto)
    {
        var result = await authService.VerifyCodeAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword( ResetPasswordDto dto)
    {
        var result = await authService.ResetPasswordAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }
}