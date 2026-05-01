using CryptorApp.Resources;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Base64 encoding and decoding.
/// </summary>
internal abstract class Base64Cryptor : CryptorBase
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Base64";

    #endregion
}

/// <summary>
/// Handles Base64 decoding.
/// </summary>
internal sealed class Base64Decryptor : Base64Cryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the Base64-encoded input string.
    /// </summary>
    /// <param name="input">The Base64 encoded text to decode</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        byte[] bytes;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                bytes = Convert.FromBase64String(input);
                output = Crypt.BytesToString(bytes, settings.SettingsViewModel.UseUnicode);
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
/// Handles Base64 encoding.
/// </summary>
internal sealed class Base64Encryptor : Base64Cryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Base64 encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded output and possibly an error message</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        byte[] bytes;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                bytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                output = Convert.ToBase64String(bytes);
            }
        }
        catch
        {
            msg = Strings.Status_ErrConvert;
        }
        return Task.FromResult(new CryptResult { Output = output, Error = msg });
    }

    #endregion
}