using SkillPath.Application.DTOs;

namespace SkillPath.Application.Interfaces;

public interface ICourseService
{
    Task<CourseDetailResponse> GetByIdAsync(int id);
    Task<PagedResponse<CourseSummaryResponse>> SearchAsync(int? skillId, string? search, int page, int pageSize);
    Task<CourseDetailResponse> CreateAsync(CreateCourseRequest request);
    Task<CourseDetailResponse> UpdateAsync(int id, UpdateCourseRequest request);
    Task DeleteAsync(int id);
}



