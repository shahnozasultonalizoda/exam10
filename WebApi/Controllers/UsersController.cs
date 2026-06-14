using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = UserRoles.Admin)]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await userService.GetAllAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await userService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create( CreateUserDto dto)
    {
        var result = await userService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserDto dto)
    {
        var result = await userService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await userService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        return Ok(result.Value);
    }
}