using System.Text.RegularExpressions;
using envSafe.Api.Models;
using envSafe.Api.Services;

namespace envSafe.Api.Tests;

public class RandomValueGeneratorTests
{
    private readonly RandomValueGenerator _generator = new();

    [Fact]
    public void StrictSafe_ProducesOnlyAlphanumericCharacters()
    {
        var value = _generator.Generate(new GenerationOptions(SecretValueType.Password, GenerationMode.StrictSafe, 64));
        Assert.Matches("^[A-Za-z0-9]+$", value);
    }

    [Fact]
    public void UrlSafeType_ProducesOnlyUrlSafeCharacters()
    {
        var value = _generator.Generate(new GenerationOptions(SecretValueType.UrlSafeString, GenerationMode.MaxEntropy, 80));
        Assert.Matches("^[A-Za-z0-9\\-._~]+$", value);
    }

    [Fact]
    public void GeneratedValues_DoNotContainControlCharacters()
    {
        var options = new GenerationOptions(SecretValueType.Token, GenerationMode.MaxEntropy, 128);
        var value = _generator.Generate(options);

        Assert.DoesNotContain(value, ch => char.IsControl(ch));
    }
}
