using CryptorApp.Resources;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for SHA hashing.
/// </summary>
internal abstract class ShaCryptor : IDisposable
{
    #region Objects and variables

    private CryptSettings? mSettings;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public abstract string Name { get; }

    #endregion

    #region Methods and functions

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync()
    {
        mSettings ??= new CryptSettings(showKey: false);
        return mSettings;
    }

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Is interface implementation for inheritors")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Is interface implementation for inheritors")]
    public bool IsValid(ref string? msg) => true;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public void Dispose() => mSettings?.Dispose();

    #endregion
}

/// <summary>
/// Handles SHA-256 hashing.
/// </summary>
internal sealed class Sha256Encryptor : ShaCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "SHA-256 Hashing";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Computes the SHA-256 hash of the input string and returns it as a lowercase hex string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the hex-encoded hash.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

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
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

/// <summary>
/// Handles SHA-512 hashing.
/// </summary>
internal sealed class Sha512Encryptor : ShaCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "SHA-512 Hashing";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Computes the SHA-512 hash of the input string and returns it as a lowercase hex string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the hex-encoded hash.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

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
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

