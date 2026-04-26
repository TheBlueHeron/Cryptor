using System.IO;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for triple des encryption and decryption.
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal abstract class TripleDesCryptor
{
    #region Objects and variables

    private const string _VALIDATION = "Requires a non-weak Key of 8 or 12 characters and an IV of 4 characters.";

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
        mSettings ??= new CryptSettings(true);
        return mSettings;
    }

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// Requires a Key of 16 or 24 bytes, an IV of 8 bytes, and a non-weak key.
    /// </summary>
    /// <param name="msg">Will contain a validation message if validation failed</param>
    public bool IsValid(ref string? msg)
    {
        if (mSettings is null)
        {
            return false;
        }
        var keyBytes = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Key);
        var ivBytes = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Iv);
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
        var valid = keyBytes.Length is 16 or 24 && ivBytes.Length == 8 && !TripleDES.IsWeakKey(keyBytes);
#pragma warning restore CA5350
        Array.Clear(keyBytes);
        Array.Clear(ivBytes);

        if (!valid)
        {
            msg = _VALIDATION;
        }
        return valid;
    }

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}
#pragma warning restore CA5350

/// <summary>
/// Handles Triple DES decoding.
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal sealed class TripleDesDecryptor : TripleDesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Triple DES Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the input string.
    /// </summary>
    /// <param name="input">The triple des-encoded text to decode</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        try
        {
            using var tripleDes = TripleDES.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var ivBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var decryptor = tripleDes.CreateDecryptor(keyBytes, ivBytes);
                    using var memStream = new MemoryStream(inputBytes);
                    using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                    using var memOutput = new MemoryStream();
                    await cryptoStream.CopyToAsync(memOutput);
                    output = Crypt.BytesToString(memOutput.ToArray(), settings.SettingsViewModel.UseUnicode);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(ivBytes);
                }
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}
#pragma warning restore CA5350

/// <summary>
/// Handles Triple DES encoding.
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal sealed class TripleDesEncryptor : TripleDesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Triple DES Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Triple DES encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the result output.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        try
        {
            using var tripleDes = TripleDES.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var ivBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var inputBytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var encryptor = tripleDes.CreateEncryptor(keyBytes, ivBytes);
                    using var memOutput = new MemoryStream();
                    using var cryptoStream = new CryptoStream(memOutput, encryptor, CryptoStreamMode.Write);
                    await cryptoStream.WriteAsync(inputBytes);
                    await cryptoStream.FlushFinalBlockAsync();
                    output = Convert.ToBase64String(memOutput.ToArray());
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(ivBytes);
                }
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}
#pragma warning restore CA5350