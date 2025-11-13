using Microsoft.EntityFrameworkCore;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;
using SkillShiftHub.Infrastructure.Data;

namespace SkillShiftHub.Infrastructure.Repositories;

public class StaticCatalogRepository : IStaticCatalogRepository
{
    private readonly AppDbContext _db;

    public StaticCatalogRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Skill>> GetSkillsAsync() =>
        await _db.Skills.AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<Course>> GetCoursesAsync() =>
        await _db.Courses.Include(c => c.Skill).AsNoTracking().ToListAsync();

    public async Task<(IReadOnlyList<Skill> Items, int TotalCount)> SearchSkillsAsync(string? search, int page, int pageSize)
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

    public async Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchCoursesAsync(int? skillId, string? search, int page, int pageSize)
    {
        var query = _db.Courses
            .Include(c => c.Skill)
            .AsNoTracking();

        if (skillId.HasValue)
        {
            query = query.Where(c => c.SkillId == skillId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim().ToUpperInvariant()}%";
            query = query.Where(c =>
                EF.Functions.Like(c.Name.ToUpper(), pattern) ||
                EF.Functions.Like(c.Provider.ToUpper(), pattern));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
