using Microsoft.EntityFrameworkCore;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;
using SkillShiftHub.Infrastructure.Data;

namespace SkillShiftHub.Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _db;

    public SkillRepository(AppDbContext db) => _db = db;

    public async Task<Skill?> GetByIdAsync(int id) =>
        await _db.Skills.FindAsync(id);

    public async Task<IReadOnlyList<Skill>> GetAllAsync() =>
        await _db.Skills.AsNoTracking().ToListAsync();

    public async Task<(IReadOnlyList<Skill> Items, int TotalCount)> SearchAsync(string? search, int page, int pageSize)
    {
        var query = _db.Skills.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim().ToUpperInvariant()}%";
            query = query.Where(s =>
                EF.Functions.Like(s.Name.ToUpper(), pattern) ||
                EF.Functions.Like(s.Description.ToUpper(), pattern));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(Skill skill)
    {
        _db.Skills.Add(skill);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Skill skill)
    {
        _db.Skills.Update(skill);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var skill = await _db.Skills.FindAsync(id);
        if (skill != null)
        {
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
        }
    }
}



