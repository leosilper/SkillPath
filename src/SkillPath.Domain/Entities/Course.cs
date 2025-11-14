namespace SkillPath.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Provider { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
}
