using Application.DTOs.Users;
using Application.Interfaces;
using Application.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService(UserManager<User> userManager) : IUserService
{
    public async Task<Result<GetUserDto>> CreateAsync(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return Result<GetUserDto>.Failure("Fullname is required", ErrorType.Validation);
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return Result<GetUserDto>.Failure("Email is required", ErrorType.Validation);
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return Result<GetUserDto>.Failure("Password is required", ErrorType.Validation);
        }

        if (dto.Password.Length < 6)
        {
            return Result<GetUserDto>.Failure("Password must be at least 6 characters long.", ErrorType.Validation);
        }

        var existingEmail = await userManager.Users.AnyAsync(u => u.Email == dto.Email);
        if (existingEmail)
        {
            return Result<GetUserDto>.Failure("This email is already registered.", ErrorType.Conflict);
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
            return Result<GetUserDto>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, dto.Role);

        var roles = await userManager.GetRolesAsync(user);
        return Result<GetUserDto>.Ok(new GetUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles.ToList()
        });
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Result<bool>.Failure("User not found", ErrorType.NotFound);
        }

        await userManager.DeleteAsync(user);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<GetUserDto>>> GetAllAsync()
    {
        var users = userManager.Users.ToList();
        var result = new List<GetUserDto>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new GetUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName ?? "",
                Roles = roles.ToList()
            });
        }

        return Result<List<GetUserDto>>.Ok(result);
    }

    public async Task<Result<GetUserDto>> GetByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Result<GetUserDto>.Failure("User not found", ErrorType.NotFound);
        }

        var roles = await userManager.GetRolesAsync(user);
        return Result<GetUserDto>.Ok(new GetUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName ?? "",
            Roles = roles.ToList()
        });
    }

    public async Task<Result<GetUserDto>> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Result<GetUserDto>.Failure("User not found", ErrorType.NotFound);
        }

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return Result<GetUserDto>.Failure("Fullname is required", ErrorType.Validation);
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return Result<GetUserDto>.Failure("Email is required", ErrorType.Validation);
        }

        if (dto.FullName != null)
        {
            user.FullName = dto.FullName;
        }

        if (dto.Email != null)
        {
            user.Email = dto.Email;
            user.UserName = dto.Email;
        }

        await userManager.UpdateAsync(user);

        if (dto.Role != null)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, dto.Role);
        }

        var roles = await userManager.GetRolesAsync(user);
        return Result<GetUserDto>.Ok(new GetUserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName!,
            Roles = roles.ToList()
        });
    }
}