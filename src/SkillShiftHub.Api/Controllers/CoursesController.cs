using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Application.Interfaces;

namespace SkillShiftHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<CourseSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? skillId = null, [FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _courseService.SearchAsync(skillId, search, page, pageSize);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        var path = Request.Path.Value ?? $"/api/v{version}/courses";
        var links = BuildPagedLinks(path, skillId, search, page, pageSize, response.Pagination.TotalPages);
        return Ok(response with { Links = links });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        return Ok(course);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var course = await _courseService.CreateAsync(request);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return Created($"/api/v{version}/courses/{course.Id}", course);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(CourseDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        var course = await _courseService.UpdateAsync(id, request);
        return Ok(course);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteAsync(id);
        return NoContent();
    }

    private static HateoasLinks BuildPagedLinks(string path, int? skillId, string? search, int page, int pageSize, int totalPages)
    {
        string BuildLink(int targetPage)
        {
            var query = new Dictionary<string, string?>
            {
                ["page"] = targetPage.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (skillId.HasValue)
            {
                query["skillId"] = skillId.Value.ToString();
            }

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
