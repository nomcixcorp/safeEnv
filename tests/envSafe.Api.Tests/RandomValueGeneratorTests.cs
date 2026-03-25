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
        var value = _generator.Generate(new GenerationOptions(
            SecretValueType.Password,
            GenerationMode.StrictSafe,
            64,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: false,
            CharacterCaseMode.Mixed));
        Assert.Matches("^[A-Za-z0-9]+$", value);
    }

    [Fact]
    public void UrlSafeType_ProducesOnlyUrlSafeCharacters()
    {
        var value = _generator.Generate(new GenerationOptions(
            SecretValueType.UrlSafeString,
            GenerationMode.MaxEntropy,
            80,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: false,
            CharacterCaseMode.Mixed));
        Assert.Matches("^[A-Za-z0-9\\-._~]+$", value);
    }

    [Fact]
    public void GeneratedValues_DoNotContainControlCharacters()
    {
        var options = new GenerationOptions(
            SecretValueType.Token,
            GenerationMode.MaxEntropy,
            128,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: false,
            CharacterCaseMode.Mixed);
        var value = _generator.Generate(options);

        Assert.DoesNotContain(value, ch => char.IsControl(ch));
    }

    [Fact]
    public void Generate_WithExcludeAmbiguousCharacters_RemovesAmbiguousCharacters()
    {
        var options = new GenerationOptions(
            SecretValueType.Token,
            GenerationMode.MaxEntropy,
            120,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: true,
            CharacterCaseMode.Mixed);

        var value = _generator.Generate(options);

        Assert.DoesNotContain(value, ch => "O0l1I|".Contains(ch));
    }

    [Fact]
    public void Generate_WithLowercaseOnly_ProducesLowercaseAndDigits()
    {
        var options = new GenerationOptions(
            SecretValueType.Token,
            GenerationMode.Balanced,
            80,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: false,
            CharacterCaseMode.LowercaseOnly);

        var value = _generator.Generate(options);
        Assert.Matches("^[a-z0-9\\-_.~]+$", value);
    }

    [Fact]
    public void Generate_WithUppercaseOnly_ProducesUppercaseAndDigits()
    {
        var options = new GenerationOptions(
            SecretValueType.Token,
            GenerationMode.Balanced,
            80,
            ExcludeSimilarCharacters: false,
            ExcludeAmbiguousCharacters: false,
            CharacterCaseMode.UppercaseOnly);

        var value = _generator.Generate(options);
        Assert.Matches("^[A-Z0-9\\-_.~]+$", value);
    }
}
