using Microsoft.EntityFrameworkCore;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;
using SkillPath.Infrastructure.Data;

namespace SkillPath.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _db.Users.FindAsync(id);

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}
