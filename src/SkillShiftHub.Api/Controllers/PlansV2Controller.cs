using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Exceptions;
using SkillShiftHub.Application.Interfaces;

namespace SkillShiftHub.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/plans")]
[Authorize]
public class PlansV2Controller : ControllerBase
{
    private readonly IPlanServiceV2 _plans;

    public PlansV2Controller(IPlanServiceV2 plans)
    {
        _plans = plans;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate()
    {
        var userId = GetUserId();
        var result = await _plans.GenerateOrGetCurrentAsync(new GeneratePlanRequest(userId));

        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
        var plan = WithVersionedLinks(result.Plan, version);

        if (result.Created)
        {
            return Created($"/api/v{version}/plans", plan);
        }

        return Ok(plan);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrent()
    {
        var userId = GetUserId();
        var response = await _plans.GetCurrentByUserAsync(userId);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
        return Ok(WithVersionedLinks(response, version));
    }

    [HttpPut("{planId:guid}/items/{order:int}")]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Toggle(Guid planId, int order)
    {
        var userId = GetUserId();
        var response = await _plans.ToggleItemCompletionAsync(userId, planId, order);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "2.0";
        return Ok(WithVersionedLinks(response, version));
    }

    [HttpDelete("{planId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid planId)
    {
        var userId = GetUserId();
        await _plans.DeleteAsync(userId, planId);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var sub = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type.EndsWith("/nameidentifier"))?.Value;
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAppException();
    }

    private static PlanResponse WithVersionedLinks(PlanResponse plan, string version)
    {
        var links = new PlanLinks(
            Self: $"/api/v{version}/plans",
            ToggleItemTemplate: $"/api/v{version}/plans/{plan.PlanId}/items/{{order}}",
            CoursesTemplate: $"/api/v{version}/courses?skillId={{skillId}}"
        );

        return plan with { Links = links };
    }
}




