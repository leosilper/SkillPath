using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Exceptions;
using SkillShiftHub.Application.Interfaces;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;

namespace SkillShiftHub.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ISkillRepository _skillRepository;

    public CourseService(ICourseRepository courseRepository, ISkillRepository skillRepository)
    {
        _courseRepository = courseRepository;
        _skillRepository = skillRepository;
    }

    public async Task<CourseDetailResponse> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id)
                    ?? throw new NotFoundAppException("Course");
        return MapToDetail(course);
    }

    public async Task<PagedResponse<CourseSummaryResponse>> SearchAsync(int? skillId, string? search, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = Math.Clamp(pageSize, 1, 50);

        var (items, total) = await _courseRepository.SearchAsync(skillId, search, page, pageSize);
        var totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)pageSize);

        var data = items.Select(MapToSummary).ToList();
        var links = new HateoasLinks(Self: "", Next: null, Prev: null);

        return new PagedResponse<CourseSummaryResponse>(data, new PaginationMetadata(page, pageSize, total, totalPages), links);
    }

    public async Task<CourseDetailResponse> CreateAsync(CreateCourseRequest request)
    {
        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill == null)
            throw new NotFoundAppException("Skill");

        var course = new Course
        {
            SkillId = request.SkillId,
            Name = request.Name,
            Provider = request.Provider,
            Url = request.Url
        };

        await _courseRepository.AddAsync(course);
        
        // Reload with skill
        course = await _courseRepository.GetByIdAsync(course.Id) ?? course;
        return MapToDetail(course);
    }

    public async Task<CourseDetailResponse> UpdateAsync(int id, UpdateCourseRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(id)
                    ?? throw new NotFoundAppException("Course");

        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill == null)
            throw new NotFoundAppException("Skill");

        course.SkillId = request.SkillId;
        course.Name = request.Name;
        course.Provider = request.Provider;
        course.Url = request.Url;

        await _courseRepository.UpdateAsync(course);
        
        // Reload with skill
        course = await _courseRepository.GetByIdAsync(course.Id) ?? course;
        return MapToDetail(course);
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new NotFoundAppException("Course");

        await _courseRepository.DeleteAsync(id);
    }

    private static CourseSummaryResponse MapToSummary(Course course) =>
        new(course.Id, course.SkillId, course.Skill?.Name ?? string.Empty, course.Name, course.Provider, course.Url);

    private static CourseDetailResponse MapToDetail(Course course) =>
        new(course.Id, course.SkillId, course.Skill?.Name ?? string.Empty, course.Name, course.Provider, course.Url);
}



