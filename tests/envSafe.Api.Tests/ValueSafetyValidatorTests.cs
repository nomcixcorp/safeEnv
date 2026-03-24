using envSafe.Api.Services;

namespace envSafe.Api.Tests;

public class ValueSafetyValidatorTests
{
    private readonly ValueSafetyValidator _validator = new();

    [Fact]
    public void Validate_FlagsControlCharactersAsSevere()
    {
        var result = _validator.Validate("abc\n123");

        Assert.True(result.ContainsControlCharacters);
        Assert.True(result.ContainsSevereRisks);
    }

    [Fact]
    public void Validate_ReturnsAlphanumericOnly_WhenApplicable()
    {
        var result = _validator.Validate("Alpha123");

        Assert.True(result.IsAlphanumericOnly);
        Assert.DoesNotContain(result.Warnings, warning => warning.Contains("control", StringComparison.OrdinalIgnoreCase));
    }
}
