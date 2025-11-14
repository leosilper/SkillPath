namespace SkillPath.Domain.Entities;

public class Plan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = "Trilha de Requalificação";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<PlanItem> Items { get; set; } = new List<PlanItem>();
}
