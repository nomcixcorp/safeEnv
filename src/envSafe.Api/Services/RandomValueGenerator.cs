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

    public string Generate(GenerationOptions options)
    {
        var charset = GetCharacterSet(options.Mode, options.ValueType);
        return GenerateFromCharset(charset, options.Length);
    }

    private static string GetCharacterSet(GenerationMode mode, SecretValueType valueType)
    {
        return valueType switch
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
