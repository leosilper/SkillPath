namespace SkillShiftHub.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string CurrentJob { get; set; } = null!;
    public string TargetArea { get; set; } = null!;
    public string EducationLevel { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Plan> Plans { get; set; } = new List<Plan>();
}
