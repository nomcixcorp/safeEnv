using System.Text.Json;
using envSafe.Api.Services;

namespace envSafe.Api.Tests;

public sealed class OutputFormatterTests
{
    private readonly OutputFormatter _formatter = new();
    private readonly ValueSafetyValidator _validator = new();

    [Fact]
    public void Format_ProducesValidJsonSnippet_WhenValueHasEscapableCharacters()
    {
        const string key = "ENVSAFE_TOKEN";
        const string value = "abc\"def\\ghi";

        var validation = _validator.Validate(value);
        var outputs = _formatter.Format(key, value, validation);

        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(outputs.Json);
        Assert.NotNull(parsed);
        Assert.Equal(value, parsed![key]);
    }

    [Fact]
    public void Format_QuotesYaml_WhenValueHasYamlFootgunCharacters()
    {
        const string key = "ENVSAFE_TOKEN";
        const string value = "a:b#c";

        var validation = _validator.Validate(value);
        var outputs = _formatter.Format(key, value, validation);

        Assert.Equal($"{key}: \"a:b#c\"", outputs.Yaml);
    }

    [Fact]
    public void Format_UsesSafeShellQuoting_WhenValueContainsSingleQuote()
    {
        const string key = "ENVSAFE_SECRET";
        const string value = "it's-safe";

        var validation = _validator.Validate(value);
        var outputs = _formatter.Format(key, value, validation);

        Assert.Equal("export ENVSAFE_SECRET='it'\"'\"'s-safe'", outputs.Bash);
        Assert.Equal("$env:ENVSAFE_SECRET = 'it''s-safe'", outputs.PowerShell);
    }
}
