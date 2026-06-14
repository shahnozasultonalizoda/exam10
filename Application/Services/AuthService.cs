using Application.DTOs.Auth;
using Application.DTOs.Users;
using Application.Interfaces;
using Application.Results;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService(
    UserManager<User> userManager,
    IJwtTokenService tokenService,
    IEmailService emailService) : IAuthService
{
    public async Task<Result<string>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return Result<string>.Failure("User already exists", ErrorType.Conflict);
        }

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, UserRoles.User);
        return Result<string>.Ok("Registered successfully");
    }

    public async Task<Result<UserInfoDto>> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<UserInfoDto>.Failure("User not found", ErrorType.NotFound);
        }

        var result = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!result)
        {
            return Result<UserInfoDto>.Failure("Username or password is incorrect", ErrorType.Validation);
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.GenerateToken(user);

        return Result<UserInfoDto>.Ok(new UserInfoDto
        {
            AccessToken = token,
            User = new GetUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName!,
                Roles = roles.ToList()
            }
        });
    }

    public async Task<Result<string>> ChangePasswordAsync(string id, ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Result<string>.Failure("User not found", ErrorType.NotFound);
        }

        var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            return Result<string>.Failure("Failed to change password", ErrorType.Validation);
        }

        return Result<string>.Ok("Password changed successfully");
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<string>.Failure("User not found", ErrorType.NotFound);
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        user.ResetCode = code;
        user.ResetCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        await emailService.SendEmailAsync(user.Email!, "Reset Code", $"Your code is: {code}");
        return Result<string>.Ok("Email sent successfully");
    }

    public async Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<bool>.Failure("User not found", ErrorType.NotFound);
        }

        if (user.ResetCode != dto.Code || user.ResetCodeExpiresAt < DateTime.UtcNow)
        {
            return Result<bool>.Failure("Invalid or expired code", ErrorType.Validation);
        }

        return Result<bool>.Ok(true);
    }

    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<string>.Failure("User not found", ErrorType.NotFound);
        }

        if (user.ResetCode != dto.Code || user.ResetCodeExpiresAt < DateTime.UtcNow)
        {
            return Result<string>.Failure("Invalid or expired code", ErrorType.Validation);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        if (!result.Succeeded)
        {
            return Result<string>.Failure("Failed to reset password");
        }

        user.ResetCode = null;
        user.ResetCodeExpiresAt = null;
        await userManager.UpdateAsync(user);

        return Result<string>.Ok("Password reset successfully");
    }
}