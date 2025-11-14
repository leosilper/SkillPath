using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SkillPath.Application.DTOs;
using SkillPath.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace SkillPath.Tests;

public class PlansV2ControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public PlansV2ControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Generate_ShouldRespectEducationLevelRules()
    {
        var registerRequest = new RegisterRequest
        {
            Name = "Usuario V2",
            Email = "usuario.v2@example.com",
            Password = "SenhaSegura123!",
            CurrentJob = "Assistente Administrativo",
            TargetArea = "Tecnologia",
            EducationLevel = "Ensino Fundamental completo"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v2/auth/register", registerRequest);
        if (!registerResponse.IsSuccessStatusCode)
        {
            _output.WriteLine(await registerResponse.Content.ReadAsStringAsync());
        }
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

        var generateResponse = await _client.PostAsync("/api/v2/plans", null);
        if (!generateResponse.IsSuccessStatusCode)
        {
            _output.WriteLine(await generateResponse.Content.ReadAsStringAsync());
        }
        Assert.Equal(HttpStatusCode.Created, generateResponse.StatusCode);

        var plan = await generateResponse.Content.ReadFromJsonAsync<PlanResponse>();
        Assert.NotNull(plan);
        Assert.True(plan!.TotalItems is >= 4 and <= 6);
        Assert.Contains(plan.Items, i => i.Skill.Contains("Pensamento Computacional", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(plan.Items, i => i.Skill.Contains("Gestão de Tempo", StringComparison.OrdinalIgnoreCase));
    }
}




