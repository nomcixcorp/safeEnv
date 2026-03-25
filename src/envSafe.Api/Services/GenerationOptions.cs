using envSafe.Api.Models;

namespace envSafe.Api.Services;

public sealed record GenerationOptions(
    SecretValueType ValueType,
    GenerationMode Mode,
    int Length,
    bool ExcludeSimilarCharacters,
    bool ExcludeAmbiguousCharacters,
    CharacterCaseMode CharacterCaseMode)
{
    public static GenerationOptions From(GenerateRequest request) =>
        // Advanced options are optional in the request payload.
        // Default behavior keeps backwards compatibility with existing clients.
        new(
            request.ValueType,
            request.Mode,
            request.Length,
            request.Advanced?.ExcludeSimilarCharacters ?? false,
            request.Advanced?.ExcludeAmbiguousCharacters ?? false,
            request.Advanced?.CharacterCaseMode ?? CharacterCaseMode.Mixed);
}
