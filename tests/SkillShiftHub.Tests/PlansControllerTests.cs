using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace SkillShiftHub.Tests;

public class PlansControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public PlansControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Generate_Get_Toggle_ShouldRespectWorkflow()
    {
        var registerRequest = new RegisterRequest
        {
            Name = "Plano Usuário",
            Email = "plano.usuario@example.com",
            Password = "SenhaSegura123!",
            CurrentJob = "Operador de Caixa",
            TargetArea = "Tecnologia",
            EducationLevel = "Ensino Médio"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        if (!registerResponse.IsSuccessStatusCode)
        {
            _output.WriteLine(await registerResponse.Content.ReadAsStringAsync());
        }
        registerResponse.EnsureSuccessStatusCode();

        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

        var generateResponse = await _client.PostAsync("/api/v1/plans", null);
        if (generateResponse.StatusCode != HttpStatusCode.Created)
        {
            _output.WriteLine(await generateResponse.Content.ReadAsStringAsync());
        }
        Assert.Equal(HttpStatusCode.Created, generateResponse.StatusCode);

        var plan = await generateResponse.Content.ReadFromJsonAsync<PlanResponse>();
        Assert.NotNull(plan);
        Assert.True(plan!.TotalItems is >= 3 and <= 6);
        Assert.Equal(0, plan.ProgressPercent);
        Assert.Contains(plan.Items, i => i.Skill.Contains("Excel", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(plan.PlanId.ToString(), plan.Links.ToggleItemTemplate, StringComparison.OrdinalIgnoreCase);

        var getResponse = await _client.GetAsync("/api/v1/plans");
        if (!getResponse.IsSuccessStatusCode)
        {
            _output.WriteLine(await getResponse.Content.ReadAsStringAsync());
        }
        getResponse.EnsureSuccessStatusCode();
        var retrievedPlan = await getResponse.Content.ReadFromJsonAsync<PlanResponse>();
        Assert.NotNull(retrievedPlan);
        Assert.Equal(plan.PlanId, retrievedPlan!.PlanId);

        var toggleResponse = await _client.PutAsync($"/api/v1/plans/{plan.PlanId}/items/1", null);
        if (!toggleResponse.IsSuccessStatusCode)
        {
            _output.WriteLine(await toggleResponse.Content.ReadAsStringAsync());
        }
        toggleResponse.EnsureSuccessStatusCode();
        var toggledPlan = await toggleResponse.Content.ReadFromJsonAsync<PlanResponse>();
        Assert.NotNull(toggledPlan);
        Assert.True(toggledPlan!.CompletedItems >= 1);
        Assert.True(toggledPlan.ProgressPercent > 0);
        Assert.NotNull(toggledPlan.Items.First(i => i.Order == 1).CompletedAt);
    }
}

