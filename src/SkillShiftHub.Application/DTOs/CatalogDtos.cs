using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillShiftHub.Application.DTOs;

public record SkillSummaryResponse(int Id, string Name, string Description);

public record CourseSummaryResponse(int Id, int SkillId, string SkillName, string Name, string Provider, string Url);

public record PaginationMetadata(int Page, int PageSize, int TotalItems, int TotalPages);

public record HateoasLinks(string Self, string? Next, string? Prev);

public record PagedResponse<T>(IReadOnlyList<T> Data, PaginationMetadata Pagination, HateoasLinks Links);

// DTOs para CRUD de Skills
public record class CreateSkillRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = null!;

    [Required, StringLength(400)]
    public string Description { get; init; } = null!;
}

public record class UpdateSkillRequest
{
    [Required, StringLength(200)]
    public string Name { get; init; } = null!;

    [Required, StringLength(400)]
    public string Description { get; init; } = null!;
}

public record SkillDetailResponse(int Id, string Name, string Description);

// DTOs para CRUD de Courses
public record class CreateCourseRequest
{
    [Required]
    public int SkillId { get; init; }

    [Required, StringLength(220)]
    public string Name { get; init; } = null!;

    [Required, StringLength(160)]
    public string Provider { get; init; } = null!;

    [Required, StringLength(400), Url]
    public string Url { get; init; } = null!;
}

public record class UpdateCourseRequest
{
    [Required]
    public int SkillId { get; init; }

    [Required, StringLength(220)]
    public string Name { get; init; } = null!;

    [Required, StringLength(160)]
    public string Provider { get; init; } = null!;

    [Required, StringLength(400), Url]
    public string Url { get; init; } = null!;
}

public record CourseDetailResponse(int Id, int SkillId, string SkillName, string Name, string Provider, string Url);

