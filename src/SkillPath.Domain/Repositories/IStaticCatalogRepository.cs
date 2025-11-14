using SkillPath.Domain.Entities;

namespace SkillPath.Domain.Repositories;

public interface IStaticCatalogRepository
{
    Task<IReadOnlyList<Skill>> GetSkillsAsync();
    Task<IReadOnlyList<Course>> GetCoursesAsync();
    Task<(IReadOnlyList<Skill> Items, int TotalCount)> SearchSkillsAsync(string? search, int page, int pageSize);
    Task<(IReadOnlyList<Course> Items, int TotalCount)> SearchCoursesAsync(int? skillId, string? search, int page, int pageSize);
}
