namespace SkillShiftHub.Domain.Entities;

public class PlanItem
{
    public int Id { get; set; }
    public Guid PlanId { get; set; }
    public int SkillId { get; set; }
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
