using Microsoft.EntityFrameworkCore;
using SkillShiftHub.Domain.Entities;
using SkillShiftHub.Domain.Repositories;
using SkillShiftHub.Infrastructure.Data;

namespace SkillShiftHub.Infrastructure.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _db;

    public PlanRepository(AppDbContext db) => _db = db;

    public async Task<Plan?> GetCurrentByUserIdAsync(Guid userId) =>
        await _db.Plans
            .Include(p => p.Items)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Plan?> GetByIdAsync(Guid planId) =>
        await _db.Plans
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == planId);

    public async Task AddAsync(Plan plan)
    {
        _db.Plans.Add(plan);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Plan plan)
    {
        _db.Plans.Update(plan);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Plan plan)
    {
        _db.Plans.Remove(plan);
        await _db.SaveChangesAsync();
    }
}
