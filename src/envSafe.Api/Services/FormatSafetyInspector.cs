using envSafe.Api.Models;

namespace envSafe.Api.Services;

public interface IFormatSafetyInspector
{
    EscapingMap DetectEscaping(string value);
}

public sealed class FormatSafetyInspector : IFormatSafetyInspector
{
    public EscapingMap DetectEscaping(string value)
    {
        var envNeedsEscaping = RequiresEnvQuote(value);
        var jsonNeedsEscaping = value.Any(static ch => ch is '"' or '\\' || char.IsControl(ch));
        var yamlNeedsEscaping = RequiresYamlQuote(value);

        return new EscapingMap(
            Env: envNeedsEscaping,
            Bash: true,
            PowerShell: true,
            Json: jsonNeedsEscaping,
            Yaml: yamlNeedsEscaping,
            DockerCompose: true);
    }

    private static bool RequiresEnvQuote(string value)
    {
        if (string.IsNullOrEmpty(value) || value.StartsWith('#'))
        {
            return true;
        }

        return value.Any(ch =>
            char.IsWhiteSpace(ch) ||
            ch is '"' or '\'' or '\\' or '$' or '`' or '#' or ';' or '&' or '=');
    }

    private static bool RequiresYamlQuote(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        if (value.StartsWith('-') || value.StartsWith('?') || value.StartsWith(':') || value.StartsWith('#'))
        {
            return true;
        }

        if (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return value.Any(ch => char.IsWhiteSpace(ch) || ch is ':' or '#' or '"' or '\'' or '\\');
    }
}
