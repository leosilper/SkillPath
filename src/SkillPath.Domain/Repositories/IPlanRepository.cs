using SkillPath.Domain.Entities;

namespace SkillPath.Domain.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetCurrentByUserIdAsync(Guid userId);
    Task<Plan?> GetByIdAsync(Guid planId);
    Task AddAsync(Plan plan);
    Task UpdateAsync(Plan plan);
    Task DeleteAsync(Plan plan);
}
