using SkillPath.Domain.Entities;

namespace SkillPath.Domain.Repositories;

public interface ISkillRepository
{
    Task<Skill?> GetByIdAsync(int id);
    Task<IReadOnlyList<Skill>> GetAllAsync();
    Task<(IReadOnlyList<Skill> Items, int TotalCount)> SearchAsync(string? search, int page, int pageSize);
    Task AddAsync(Skill skill);
    Task UpdateAsync(Skill skill);
    Task DeleteAsync(int id);
}



