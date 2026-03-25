using envSafe.Api.Models;
using envSafe.Api.Services;
using envSafe.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});
builder.Services.AddSingleton<IRandomValueGenerator, RandomValueGenerator>();
builder.Services.AddSingleton<IValueSafetyValidator, ValueSafetyValidator>();
builder.Services.AddSingleton<IFormatSafetyInspector, FormatSafetyInspector>();
builder.Services.AddSingleton<IOutputFormatter, OutputFormatter>();
builder.Services.AddSingleton<IGenerationService, GenerationService>();

var app = builder.Build();

app.UseCors("AllowLocalDev");

app.MapGet("/health", () => Results.Ok(new HealthResponse("ok", "envSafe.Api")));

app.MapPost("/api/generate", (GenerateRequest request, IGenerationService generationService) =>
{
    if (!request.TryValidate(out var validationError))
    {
        return Results.BadRequest(new { error = validationError });
    }

    var response = generationService.Generate(request);
    return Results.Ok(response);
});

app.Run();

public partial class Program;
