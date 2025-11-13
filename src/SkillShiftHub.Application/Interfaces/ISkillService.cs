using SkillShiftHub.Application.DTOs;

namespace SkillShiftHub.Application.Interfaces;

public interface ISkillService
{
    Task<SkillDetailResponse> GetByIdAsync(int id);
    Task<PagedResponse<SkillSummaryResponse>> SearchAsync(string? search, int page, int pageSize);
    Task<SkillDetailResponse> CreateAsync(CreateSkillRequest request);
    Task<SkillDetailResponse> UpdateAsync(int id, UpdateSkillRequest request);
    Task DeleteAsync(int id);
}



