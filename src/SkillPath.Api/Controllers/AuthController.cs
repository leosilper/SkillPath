using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillPath.Application.DTOs;
using SkillPath.Application.Exceptions;
using SkillPath.Application.Interfaces;

namespace SkillPath.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _auth.RegisterAsync(request);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return Created($"/api/v{version}/auth/me", result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = GetUserId();
            var user = await _auth.GetUserAsync(userId);
            return Ok(user);
        }
        catch (UnauthorizedAppException)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Token inválido ou ausente. Faça login novamente." });
        }
    }

    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request)
    {
        try
        {
            var userId = GetUserId();
            var user = await _auth.UpdateUserAsync(userId, request);
            return Ok(user);
        }
        catch (UnauthorizedAppException)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Token inválido ou ausente. Faça login novamente." });
        }
    }

    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMe()
    {
        try
        {
            var userId = GetUserId();
            await _auth.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (UnauthorizedAppException)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Token inválido ou ausente. Faça login novamente." });
        }
    }

    private Guid GetUserId()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAppException("Usuário não autenticado.");
        }

        var sub = User.Claims.FirstOrDefault(c => 
            c.Type == "sub" || 
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type.EndsWith("/nameidentifier"))?.Value;
            
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var id))
        {
            throw new UnauthorizedAppException("Token inválido: claim 'sub' não encontrada ou inválida.");
        }
        
        return id;
    }
}
