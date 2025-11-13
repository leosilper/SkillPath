using System.Security.Cryptography;
using System.Text;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Exceptions;
using SkillShiftHub.Application.Interfaces;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;

namespace SkillShiftHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenProvider _tokenProvider;

    public AuthService(IUserRepository users, ITokenProvider tokenProvider)
    {
        _users = users;
        _tokenProvider = tokenProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictAppException("E-mail já cadastrado.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = Hash(request.Password),
            CurrentJob = request.CurrentJob,
            TargetArea = request.TargetArea,
            EducationLevel = request.EducationLevel
        };

        await _users.AddAsync(user);

        var token = _tokenProvider.GenerateToken(user);

        return new AuthResponse(user.Id, user.Name, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email)
                   ?? throw new UnauthorizedAppException("Credenciais inválidas.");

        if (user.PasswordHash != Hash(request.Password))
            throw new UnauthorizedAppException("Credenciais inválidas.");

        var token = _tokenProvider.GenerateToken(user);

        return new AuthResponse(user.Id, user.Name, user.Email, token);
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}

public interface ITokenProvider
{
    string GenerateToken(User user);
}
