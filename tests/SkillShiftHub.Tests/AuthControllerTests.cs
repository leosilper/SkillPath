using System.Net;
using System.Net.Http.Json;
using SkillShiftHub.Application.DTOs;
using SkillShiftHub.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace SkillShiftHub.Tests;

public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public AuthControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Register_Then_Login_ShouldReturnToken()
    {
        var registerRequest = new RegisterRequest
        {
            Name = "Usuário Teste",
            Email = "usuario.teste@example.com",
            Password = "Secreta123!",
            CurrentJob = "Operador de Caixa",
            TargetArea = "Tecnologia",
            EducationLevel = "Ensino Médio"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        if (registerResponse.StatusCode != HttpStatusCode.Created)
        {
            var body = await registerResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Register response: {body}");
        }

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse!.Token));

        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        if (loginResponse.StatusCode != HttpStatusCode.OK)
        {
            var body = await loginResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Login response: {body}");
        }

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginAuth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginAuth);
        Assert.False(string.IsNullOrWhiteSpace(loginAuth!.Token));
    }
}

