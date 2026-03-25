using envSafe.Api.Models;

namespace envSafe.Api.Services;

public sealed record GenerationOptions(SecretValueType ValueType, GenerationMode Mode, int Length)
{
    public static GenerationOptions From(GenerateRequest request) =>
        new(request.ValueType, request.Mode, request.Length);
}
