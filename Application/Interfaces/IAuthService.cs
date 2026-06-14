using Application.DTOs.Auth;
using Application.Results;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(RegisterDto dto);
    Task<Result<UserInfoDto>> LoginAsync(LoginDto dto);
    Task<Result<string>> ChangePasswordAsync(string id, ChangePasswordDto dto);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto dto);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordDto dto);
}
