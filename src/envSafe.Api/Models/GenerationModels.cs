using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace envSafe.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SecretValueType
{
    Password,
    ApiKey,
    Token,
    UrlSafeString,
    ConnectionStringSafeValue
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GenerationMode
{
    StrictSafe,
    Balanced,
    MaxEntropy
}

public sealed class GenerateRequest
{
    public string VariableName { get; init; } = "ENVSAFE_VALUE";

    [EnumDataType(typeof(SecretValueType))]
    public SecretValueType ValueType { get; init; } = SecretValueType.Token;

    [EnumDataType(typeof(GenerationMode))]
    public GenerationMode Mode { get; init; } = GenerationMode.Balanced;

    [Range(8, 256)]
    public int Length { get; init; } = 32;

    public GenerationAdvancedOptions? Advanced { get; init; }
}

public sealed record GenerationAdvancedOptions(
    bool ExcludeSimilarCharacters = false,
    bool ExcludeAmbiguousCharacters = false,
    CharacterCaseMode CharacterCaseMode = CharacterCaseMode.Mixed);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CharacterCaseMode
{
    Mixed,
    LowercaseOnly,
    UppercaseOnly
}

public sealed record GenerateResponse(
    string VariableName,
    SecretValueType ValueType,
    GenerationMode Mode,
    int Length,
    IReadOnlyList<GeneratedCandidate> Candidates);

public sealed record GeneratedCandidate(
    string RawValue,
    CandidateOutputs Outputs,
    CandidateSafetyMetadata Metadata,
    string WhyThisIsSafe);

public sealed record CandidateOutputs(
    string Env,
    string Bash,
    string PowerShell,
    string Json,
    string Yaml,
    string DockerCompose);

public sealed record CandidateSafetyMetadata(
    bool WasRegenerated,
    EscapingMap EscapingApplied,
    IReadOnlyList<string> Flags);

public sealed record EscapingMap(
    bool Env,
    bool Bash,
    bool PowerShell,
    bool Json,
    bool Yaml,
    bool DockerCompose);

public sealed record HealthResponse(string Status, string Service);
