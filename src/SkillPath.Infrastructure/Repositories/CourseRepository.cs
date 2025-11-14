using Microsoft.EntityFrameworkCore;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;
using SkillPath.Infrastructure.Data;

namespace SkillPath.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;

    public CourseRepository(AppDbContext db) => _db = db;

    public async Task<Course?> GetByIdAsync(int id) =>
        await _db.Courses.Include(c => c.Skill).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Course>> GetAllAsync() =>
        await _db.Courses.Include(c => c.Skill).AsNoTracking().ToListAsync();

    public async Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchAsync(int? skillId, string? search, int page, int pageSize)
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

    public async Task AddAsync(Course course)
    {
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _db.Courses.Update(course);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course != null)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }
    }
}



