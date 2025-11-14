using System.ComponentModel.DataAnnotations;

namespace SkillPath.Application.DTOs;

public record class RegisterRequest
{
    [Required, StringLength(120)]
    public string Name { get; init; } = null!;

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; init; } = null!;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; init; } = null!;

    [Required, StringLength(120)]
    public string CurrentJob { get; init; } = null!;

    [Required, StringLength(120)]
    public string TargetArea { get; init; } = null!;

    [Required, StringLength(120)]
    public string EducationLevel { get; init; } = null!;
}

public record class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    public string Password { get; init; } = null!;
}

public record AuthResponse(Guid UserId, string Name, string Email, string Token);
