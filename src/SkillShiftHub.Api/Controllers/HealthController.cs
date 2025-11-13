using Microsoft.AspNetCore.Mvc;

namespace SkillShiftHub.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
}
