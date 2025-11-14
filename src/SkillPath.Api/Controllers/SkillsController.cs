using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SkillPath.Application.DTOs;
using SkillPath.Application.Interfaces;

namespace SkillPath.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/skills")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<SkillSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _skillService.SearchAsync(search, page, pageSize);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        var path = Request.Path.Value ?? $"/api/v{version}/skills";
        var links = BuildPagedLinks(path, search, page, response.Pagination.TotalPages);
        return Ok(response with { Links = links });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SkillDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        return Ok(skill);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SkillDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSkillRequest request)
    {
        var skill = await _skillService.CreateAsync(request);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return Created($"/api/v{version}/skills/{skill.Id}", skill);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(SkillDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSkillRequest request)
    {
        var skill = await _skillService.UpdateAsync(id, request);
        return Ok(skill);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _skillService.DeleteAsync(id);
        return NoContent();
    }

    private static HateoasLinks BuildPagedLinks(string path, string? search, int page, int totalPages)
    {
        string BuildLink(int targetPage)
        {
            var query = new Dictionary<string, string?>
            {
                ["page"] = targetPage.ToString(),
                ["pageSize"] = "10"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                query["search"] = search;
            }

            return QueryHelpers.AddQueryString(path, query!);
        }

        var self = BuildLink(page);
        var next = page < totalPages ? BuildLink(page + 1) : null;
        var prev = page > 1 ? BuildLink(page - 1) : null;

        return new HateoasLinks(self, next, prev);
    }
}
