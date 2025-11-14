using SkillPath.Domain.Entities;

namespace SkillPath.Domain.Repositories;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IReadOnlyList<Course>> GetAllAsync();
    Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchAsync(int? skillId, string? search, int page, int pageSize);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}



