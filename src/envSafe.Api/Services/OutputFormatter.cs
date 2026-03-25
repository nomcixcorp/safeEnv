using System.Text;
using System.Text.Json;
using envSafe.Api.Models;

namespace envSafe.Api.Services;

public interface IOutputFormatter
{
    CandidateOutputs Format(string key, string value, ValidationResult validation);
}

public sealed class OutputFormatter : IOutputFormatter
{
    public CandidateOutputs Format(string key, string value, ValidationResult validation)
    {
        var envValue = RequiresEnvQuote(value)
            ? $"\"{EscapeDoubleQuoted(value)}\""
            : value;
        var bashValue = value.Replace("'", "'\"'\"'");
        var psValue = value.Replace("'", "''");
        var jsonSnippet = JsonSerializer.Serialize(new Dictionary<string, string> { [key] = value });
        var yamlValue = RequiresYamlQuote(value)
            ? $"\"{EscapeDoubleQuoted(value)}\""
            : value;
        var dockerValue = EscapeDoubleQuoted(value);

        return new CandidateOutputs(
            $"{key}={envValue}",
            $"export {key}='{bashValue}'",
            $"$env:{key} = '{psValue}'",
            jsonSnippet,
            $"{key}: {yamlValue}",
            $"environment:\n  - {key}=\"{dockerValue}\"");
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

    private static string EscapeDoubleQuoted(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            switch (ch)
            {
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '"':
                    builder.Append("\\\"");
                    break;
                default:
                    builder.Append(ch);
                    break;
            }
        }

        return builder.ToString();
    }
}
