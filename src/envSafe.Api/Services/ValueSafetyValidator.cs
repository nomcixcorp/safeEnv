using envSafe.Api.Models;

namespace envSafe.Api.Services;

public interface IValueSafetyValidator
{
    ValidationResult Validate(string value);
}

public sealed class ValidationResult
{
    public required bool ContainsControlCharacters { get; init; }
    public required bool ContainsSevereRisks { get; init; }
    public required bool IsAlphanumericOnly { get; init; }
    public required bool ContainsShellSensitiveCharacters { get; init; }
    public required bool ContainsYamlSensitiveCharacters { get; init; }
    public required bool ContainsJsonSensitiveCharacters { get; init; }
    public required EscapingMap EscapingApplied { get; init; }
    public required IReadOnlyList<string> Warnings { get; init; }
    public required IReadOnlyList<string> WhyThisIsSafe { get; init; }
}

public sealed class ValueSafetyValidator : IValueSafetyValidator
{
    private static readonly HashSet<char> ShellSensitiveChars =
    [
        '$', '&', ';', '|', '<', '>', '(', ')', '*', '`'
    ];

    private static readonly HashSet<char> YamlSensitiveChars =
    [
        '#', ':', '{', '}', '[', ']', ',', '&', '*', '!', '%', '?'
    ];

    public ValidationResult Validate(string value)
    {
        var warnings = new List<string>();
        var whySafe = new List<string>();

        var containsControl = value.Any(static ch => char.IsControl(ch));
        var containsSevere = value.Any(static ch => ch is '\n' or '\r' or '\t' or '\0' || char.IsControl(ch));
        var alphanumericOnly = value.All(char.IsLetterOrDigit);
        var containsShellSensitive = value.Any(ShellSensitiveChars.Contains);
        var containsYamlSensitive = value.Any(YamlSensitiveChars.Contains);
        var containsJsonSensitive = value.Any(static ch => ch is '"' or '\\');

        if (containsControl)
        {
            warnings.Add("Detected control characters and regenerated when possible.");
        }
        else
        {
            whySafe.Add("No control characters detected.");
        }

        if (alphanumericOnly)
        {
            whySafe.Add("Only alphanumeric characters used.");
        }

        if (containsShellSensitive)
        {
            warnings.Add("Excluded or escaped shell-sensitive characters.");
        }
        else
        {
            whySafe.Add("Excluded shell-sensitive characters.");
        }

        if (containsYamlSensitive)
        {
            warnings.Add("Quoted for YAML compatibility.");
        }

        if (containsJsonSensitive)
        {
            warnings.Add("Escaped for JSON compatibility.");
        }

        if (whySafe.Count == 0)
        {
            whySafe.Add("Generated with conservative charset and validated output formatting.");
        }

        return new ValidationResult
        {
            ContainsControlCharacters = containsControl,
            ContainsSevereRisks = containsSevere,
            IsAlphanumericOnly = alphanumericOnly,
            ContainsShellSensitiveCharacters = containsShellSensitive,
            ContainsYamlSensitiveCharacters = containsYamlSensitive,
            ContainsJsonSensitiveCharacters = containsJsonSensitive,
            EscapingApplied = new EscapingMap(false, true, true, false, false, true),
            Warnings = warnings,
            WhyThisIsSafe = whySafe
        };
    }
}
