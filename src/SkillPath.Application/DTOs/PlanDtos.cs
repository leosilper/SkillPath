using System;
using System.Collections.Generic;

namespace SkillPath.Application.DTOs;

public record GeneratePlanRequest(Guid UserId);

public record PlanItemResponse(int SkillId, int Order, string Skill, string Description, bool IsCompleted, DateTime? CompletedAt);

public record PlanLinks(string Self, string ToggleItemTemplate, string CoursesTemplate);

public record PlanResponse(Guid PlanId, string Title, DateTime CreatedAt, int TotalItems, int CompletedItems, IReadOnlyList<PlanItemResponse> Items, PlanLinks Links)
{
    public int ProgressPercent => TotalItems == 0
        ? 0
        : (int)Math.Round((CompletedItems / (double)TotalItems) * 100, MidpointRounding.AwayFromZero);
}

public record PlanResult(PlanResponse Plan, bool Created);
