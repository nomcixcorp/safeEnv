using System.Security.Cryptography;
using System.Text;
using envSafe.Api.Models;

namespace envSafe.Api.Services;

public interface IRandomValueGenerator
{
    string Generate(GenerationOptions options);
}

public sealed class RandomValueGenerator : IRandomValueGenerator
{
    private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string BalancedSymbols = "-_.~";
    private const string MaxEntropySymbols = "!@%^*()-_=+[]{}|,./?:~";
    private const string UrlSafe = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
    private const string ConnectionSafe = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.";
    private const string SimilarCharacters = "O0Il1";
    private const string AmbiguousCharacters = "O0Il1|`'\"";

    public string Generate(GenerationOptions options)
    {
        var charset = GetCharacterSet(options.Mode, options.ValueType);
        charset = ApplyAdvancedFiltering(charset, options);
        if (string.IsNullOrEmpty(charset))
        {
            charset = Alphanumeric;
        }

        return GenerateFromCharset(charset, options.Length);
    }

    private static string GetCharacterSet(GenerationMode mode, SecretValueType valueType)
    {
        var baseCharset = valueType switch
        {
            SecretValueType.UrlSafeString => UrlSafe,
            SecretValueType.ConnectionStringSafeValue => ConnectionSafe,
            _ => mode switch
            {
                GenerationMode.StrictSafe => Alphanumeric,
                GenerationMode.Balanced => Alphanumeric + BalancedSymbols,
                GenerationMode.MaxEntropy => Alphanumeric + BalancedSymbols + MaxEntropySymbols,
                _ => Alphanumeric
            }
        };

        return baseCharset;
    }

    private static string ApplyAdvancedFiltering(string charset, GenerationOptions options)
    {
        var working = charset;

        if (options.ExcludeSimilarCharacters)
        {
            working = new string(working.Where(ch => !SimilarCharacters.Contains(ch)).ToArray());
        }

        if (options.ExcludeAmbiguousCharacters)
        {
            working = new string(working.Where(ch => !AmbiguousCharacters.Contains(ch)).ToArray());
        }

        switch (options.CharacterCaseMode)
        {
            case CharacterCaseMode.LowercaseOnly:
                working = new string(working.Where(ch => !char.IsLetter(ch) || char.IsLower(ch)).ToArray());
                break;
            case CharacterCaseMode.UppercaseOnly:
                working = new string(working.Where(ch => !char.IsLetter(ch) || char.IsUpper(ch)).ToArray());
                break;
        }

        return working;
    }

    private static string GenerateFromCharset(string charset, int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);

        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            sb.Append(charset[bytes[i] % charset.Length]);
        }

        return sb.ToString();
    }
}
