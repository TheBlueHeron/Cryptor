using System.IO;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Aes encryption and decryption.
/// </summary>
internal abstract class AesCryptor
{
    #region Objects and variables

    private const string _VALIDATION = "Requires a Key of 8, 12, or 16 characters and an IV of 8 characters.";

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
    /// Requires a Key of 16, 24, or 32 bytes and an IV of 16 bytes.
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
        var valid = keyBytes.Length is 16 or 24 or 32 && ivBytes.Length == 16;
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

/// <summary>
/// Handles AES decoding.
/// </summary>
internal sealed class AesDecryptor : AesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Aes Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the input string.
    /// </summary>
    /// <param name="input">The aes-encoded text to decode</param>
    /// <returns>A <see langword="string"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        try
        {
            using var aes = Aes.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var ivBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var decryptor = aes.CreateDecryptor(keyBytes, ivBytes);
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

/// <summary>
/// Handles AES encoding.
/// </summary>
internal sealed class AesEncryptor : AesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Aes Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Aes encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        try
        {
            using var aes = Aes.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var ivBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var inputBytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var encryptor = aes.CreateEncryptor(keyBytes, ivBytes);
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