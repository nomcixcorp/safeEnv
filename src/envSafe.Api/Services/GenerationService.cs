using envSafe.Api.Models;

namespace envSafe.Api.Services;

public interface IGenerationService
{
    GenerateResponse Generate(GenerateRequest request);
}

public class GenerationService : IGenerationService
{
    private readonly IRandomValueGenerator _generator;
    private readonly IValueSafetyValidator _validator;
    private readonly IOutputFormatter _formatter;
    private readonly IFormatSafetyInspector _formatSafetyInspector;

    public GenerationService(
        IRandomValueGenerator generator,
        IValueSafetyValidator validator,
        IOutputFormatter formatter,
        IFormatSafetyInspector formatSafetyInspector)
    {
        _generator = generator;
        _validator = validator;
        _formatter = formatter;
        _formatSafetyInspector = formatSafetyInspector;
    }

    public GenerateResponse Generate(GenerateRequest request)
    {
        var variableName = NormalizeVariableName(request.VariableName);
        var options = GenerationOptions.From(request);
        var candidates = new List<GeneratedCandidate>(capacity: 3);

        for (var i = 0; i < 3; i++)
        {
            candidates.Add(GenerateCandidate(variableName, options));
        }

        return new GenerateResponse(variableName, request.ValueType, request.Mode, request.Length, candidates);
    }

    private GeneratedCandidate GenerateCandidate(string variableName, GenerationOptions options)
    {
        var attempt = 0;
        var regenerated = false;
        string rawValue;
        ValidationResult validation;

        do
        {
            rawValue = _generator.Generate(options);
            validation = _validator.Validate(rawValue);
            attempt++;
            regenerated = regenerated || attempt > 1;
        } while (attempt < 6 && validation.ContainsSevereRisks);

        var outputs = _formatter.Format(variableName, rawValue, validation);
        var escaping = _formatSafetyInspector.DetectEscaping(rawValue);
        var safetyMetadata = new CandidateSafetyMetadata(regenerated, escaping, validation.Warnings);
        var explanation = validation.WhyThisIsSafe.FirstOrDefault() ?? "Validated for safer cross-format usage.";
        return new GeneratedCandidate(rawValue, outputs, safetyMetadata, explanation);
    }

    private static string NormalizeVariableName(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return "ENVSAFE_VALUE";
        }

        var candidate = raw.Trim();
        var chars = candidate
            .Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? char.ToUpperInvariant(ch) : '_')
            .ToArray();
        var normalized = new string(chars).Trim('_');

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return "ENVSAFE_VALUE";
        }

        if (!char.IsLetter(normalized[0]) && normalized[0] != '_')
        {
            normalized = $"ENVSAFE_{normalized}";
        }

        return normalized;
    }
}
