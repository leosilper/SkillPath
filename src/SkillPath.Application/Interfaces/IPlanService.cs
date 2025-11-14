using SkillPath.Application.DTOs;

namespace SkillPath.Application.Interfaces;

public interface IPlanService
{
    Task<PlanResult> GenerateOrGetCurrentAsync(GeneratePlanRequest request);
    Task<PlanResponse> GetCurrentByUserAsync(Guid userId);
    Task<PlanResponse> ToggleItemCompletionAsync(Guid userId, Guid planId, int order);
    Task DeleteAsync(Guid userId, Guid planId);
}
