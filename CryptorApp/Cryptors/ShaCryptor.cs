using CryptorApp.Resources;
using System.Security.Cryptography;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for SHA hashing.
/// </summary>
internal abstract class ShaCryptor : CryptorBase { }

/// <summary>
/// Handles SHA-256 hashing.
/// </summary>
internal sealed class Sha256Encryptor : ShaCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "SHA-256 Hashing";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Computes the SHA-256 hash of the input string and returns it as a lowercase hex string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the hex-encoded hash</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                var inputBytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                var hashBytes = SHA256.HashData(inputBytes);
                output = Convert.ToHexStringLower(hashBytes);
            }
        }
        catch
        {
            msg = Strings.Status_ErrCrypt;
        }
        return Task.FromResult(new CryptResult { Output = output, Error = msg });
    }

    #endregion
}

/// <summary>
/// Handles SHA-512 hashing.
/// </summary>
internal sealed class Sha512Encryptor : ShaCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "SHA-512 Hashing";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Computes the SHA-512 hash of the input string and returns it as a lowercase hex string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the hex-encoded hash</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                var inputBytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                var hashBytes = SHA512.HashData(inputBytes);
                output = Convert.ToHexStringLower(hashBytes);
            }
        }
        catch
        {
            msg = Strings.Status_ErrCrypt;
        }
        return Task.FromResult(new CryptResult { Output = output, Error = msg });
    }

    #endregion
}