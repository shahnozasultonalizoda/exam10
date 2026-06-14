using Application.DTOs.Users;

namespace Application.DTOs.Auth;

public class UserInfoDto
{
    public GetUserDto User { get; set; } = null!; 
    public string AccessToken { get; set; } = null!;
}
