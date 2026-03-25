using System.Net;
using System.Net.Http.Json;
using envSafe.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace envSafe.Api.Tests;

public sealed class ApiEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(payload);
        Assert.Equal("ok", payload!.Status);
        Assert.Equal("envSafe.Api", payload.Service);
    }

    [Fact]
    public async Task GenerateEndpoint_ReturnsThreeCandidates()
    {
        var request = new GenerateRequest
        {
            VariableName = "API_TOKEN",
            ValueType = SecretValueType.Token,
            Mode = GenerationMode.Balanced,
            Length = 24,
            Advanced = new GenerationAdvancedOptions(
                ExcludeSimilarCharacters: false,
                ExcludeAmbiguousCharacters: true,
                CharacterCaseMode: CharacterCaseMode.Mixed)
        };

        var response = await _client.PostAsJsonAsync("/api/generate", request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GenerateResponse>();
        Assert.NotNull(payload);
        Assert.Equal(3, payload!.Candidates.Count);
        Assert.DoesNotContain(payload.Candidates.Select(candidate => candidate.RawValue).SelectMany(value => value), ch => ch is '0' or 'O' or '1' or 'l' or 'I');
    }

    [Fact]
    public async Task GenerateEndpoint_WithInvalidLength_ReturnsBadRequest()
    {
        var request = new
        {
            variableName = "API_TOKEN",
            valueType = "Token",
            mode = "Balanced",
            length = 6
        };

        var response = await _client.PostAsJsonAsync("/api/generate", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
