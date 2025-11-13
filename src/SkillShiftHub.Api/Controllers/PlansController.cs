using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Exceptions;
using SkillShiftHub.Application.Interfaces;

namespace SkillShiftHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/plans")]
[Authorize]
public class PlansController : ControllerBase
{
    private readonly IPlanService _plans;

    public PlansController(IPlanService plans)
    {
        _plans = plans;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Generate()
    {
        try
        {
            var userId = GetUserId();
            var result = await _plans.GenerateOrGetCurrentAsync(new GeneratePlanRequest(userId));

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
            var plan = WithVersionedLinks(result.Plan, version);
            if (result.Created)
            {
                return Created($"/api/v{version}/plans", plan);
            }

            return Ok(plan);
        }
        catch (UnauthorizedAppException)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Token inválido ou ausente. Faça login novamente." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrent()
    {
        var userId = GetUserId();
        var response = await _plans.GetCurrentByUserAsync(userId);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return Ok(WithVersionedLinks(response, version));
    }

    [HttpPut("{planId:guid}/items/{order:int}")]
    [ProducesResponseType(typeof(PlanResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Toggle(Guid planId, int order)
    {
        var userId = GetUserId();
        var response = await _plans.ToggleItemCompletionAsync(userId, planId, order);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
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
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAppException("Usuário não autenticado.");
        }

        var sub = User.Claims.FirstOrDefault(c => 
            c.Type == "sub" || 
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type.EndsWith("/nameidentifier"))?.Value;
            
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var id))
        {
            throw new UnauthorizedAppException("Token inválido: claim 'sub' não encontrada ou inválida.");
        }
        
        return id;
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
