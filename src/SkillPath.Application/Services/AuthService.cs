using System.Security.Cryptography;
using System.Text;
using SkillPath.Application.DTOs;
using SkillPath.Application.Exceptions;
using SkillPath.Application.Interfaces;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;

namespace SkillPath.Application.Services;

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

    public async Task<UserResponse> GetUserAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId)
                   ?? throw new NotFoundAppException("User");
        
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CurrentJob,
            user.TargetArea,
            user.EducationLevel,
            user.CreatedAt
        );
    }

    public async Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _users.GetByIdAsync(userId)
                   ?? throw new NotFoundAppException("User");

        // Verificar se o email já está em uso por outro usuário
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var existing = await _users.GetByEmailAsync(request.Email);
            if (existing is not null && existing.Id != userId)
                throw new ConflictAppException("E-mail já cadastrado.");
        }

        // Atualizar apenas os campos fornecidos
        if (!string.IsNullOrWhiteSpace(request.Name))
            user.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = Hash(request.Password);

        if (!string.IsNullOrWhiteSpace(request.CurrentJob))
            user.CurrentJob = request.CurrentJob;

        if (!string.IsNullOrWhiteSpace(request.TargetArea))
            user.TargetArea = request.TargetArea;

        if (!string.IsNullOrWhiteSpace(request.EducationLevel))
            user.EducationLevel = request.EducationLevel;

        await _users.UpdateAsync(user);

        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CurrentJob,
            user.TargetArea,
            user.EducationLevel,
            user.CreatedAt
        );
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundAppException("User");

        await _users.DeleteAsync(userId);
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
