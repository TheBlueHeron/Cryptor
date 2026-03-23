using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Handles AES decoding.
/// </summary>
internal sealed class AesDecryptor : ICryptor
{
    #region Objects and variables

    private CryptSettings? mSettings;

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Aes Decoding";

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
                var inputBytes = Convert.FromBase64String(input);
                var decryptor = aes.CreateDecryptor(Crypt.StringToBytes(settings.SettingsViewModel.Key), Crypt.StringToBytes(settings.SettingsViewModel.Iv));
                using var memStream = new MemoryStream(inputBytes);
                using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                using var memOutput = new MemoryStream();
                await cryptoStream.CopyToAsync(memOutput);
                output = Crypt.BytesToString(memOutput.ToArray());
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }

        return new CryptResult { Output = output, Error = msg };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync()
    {
        if (mSettings is null)
        {
            mSettings = new CryptSettings();
        }
        return mSettings;
    }

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}

/// <summary>
/// Handles AES encoding.
/// </summary>
internal sealed class AesEncryptor : ICryptor
{
    #region Objects and variables

    private CryptSettings? mSettings;

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Aes Encoding";

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
                var inputBytes = Crypt.StringToBytes(input);
                var encryptor = aes.CreateEncryptor(Crypt.StringToBytes(settings.SettingsViewModel.Key), Crypt.StringToBytes(settings.SettingsViewModel.Iv));
                using var memOutput = new MemoryStream();
                using var cryptoStream = new CryptoStream(memOutput, encryptor, CryptoStreamMode.Write);
                await cryptoStream.WriteAsync(inputBytes);
                await cryptoStream.FlushFinalBlockAsync();
                output = Convert.ToBase64String(memOutput.ToArray());
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }

        return new CryptResult { Output = output, Error = msg };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync()
    {
        if (mSettings is null)
        {
            mSettings = new CryptSettings();
        }
        return mSettings;
    }

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}