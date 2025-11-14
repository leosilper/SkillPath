using SkillPath.Application.DTOs;
using SkillPath.Application.Exceptions;
using SkillPath.Application.Interfaces;
using SkillPath.Domain.Entities;
using SkillPath.Domain.Repositories;

namespace SkillPath.Application.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;

    public SkillService(ISkillRepository repository)
    {
        _repository = repository;
    }

    public async Task<SkillDetailResponse> GetByIdAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id)
                   ?? throw new NotFoundAppException("Skill");
        return MapToDetail(skill);
    }

    public async Task<PagedResponse<SkillSummaryResponse>> SearchAsync(string? search, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = Math.Clamp(pageSize, 1, 50);

        var (items, total) = await _repository.SearchAsync(search, page, pageSize);
        var totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)pageSize);

        var data = items.Select(MapToSummary).ToList();
        var links = new HateoasLinks(Self: "", Next: null, Prev: null);

        return new PagedResponse<SkillSummaryResponse>(data, new PaginationMetadata(page, pageSize, total, totalPages), links);
    }

    public async Task<SkillDetailResponse> CreateAsync(CreateSkillRequest request)
    {
        var skill = new Skill
        {
            Name = request.Name,
            Description = request.Description
        };

        await _repository.AddAsync(skill);
        return MapToDetail(skill);
    }

    public async Task<SkillDetailResponse> UpdateAsync(int id, UpdateSkillRequest request)
    {
        var skill = await _repository.GetByIdAsync(id)
                   ?? throw new NotFoundAppException("Skill");

        skill.Name = request.Name;
        skill.Description = request.Description;

        await _repository.UpdateAsync(skill);
        return MapToDetail(skill);
    }

    public async Task DeleteAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id);
        if (skill == null)
            throw new NotFoundAppException("Skill");

        await _repository.DeleteAsync(id);
    }

    private static SkillSummaryResponse MapToSummary(Skill skill) =>
        new(skill.Id, skill.Name, skill.Description);

    private static SkillDetailResponse MapToDetail(Skill skill) =>
        new(skill.Id, skill.Name, skill.Description);
}

