using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using SkillPath.Tests.Infrastructure;
using Xunit;

namespace SkillPath.Tests;

public class SkillsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SkillsControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSkills_ShouldReturnPagedResultWithLinks()
    {
        var response = await _client.GetAsync("/api/v1/skills?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedSkillsResponse>();
        Assert.NotNull(payload);
        Assert.NotNull(payload!.Data);
        Assert.True(payload.Data.Count <= 5);
        Assert.NotNull(payload.Pagination);
        Assert.True(payload.Pagination!.TotalItems >= payload.Data.Count);
        Assert.NotNull(payload.Links);
        Assert.False(string.IsNullOrWhiteSpace(payload.Links!.Self));
    }

    private sealed record SkillSummary(int Id, string Name, string Description);

    private sealed record PaginationInfo(int Page, int PageSize, int TotalItems, int TotalPages);

    private sealed record LinkInfo(string Self, string? Next, string? Prev);

    private sealed class PagedSkillsResponse
    {
        public List<SkillSummary> Data { get; init; } = new();
        public PaginationInfo? Pagination { get; init; }
        public LinkInfo? Links { get; init; }
    }
}

