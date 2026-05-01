using CryptorApp.Resources;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for hex encoding and decoding.
/// </summary>
internal abstract class HexCryptor : CryptorBase
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Hex";

    #endregion
}

/// <summary>
/// Handles hex decoding.
/// </summary>
internal sealed class HexDecryptor : HexCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the hex-encoded input string.
    /// </summary>
    /// <param name="input">The hex-encoded text to decode</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                var bytes = Convert.FromHexString(input);
                output = Crypt.BytesToString(bytes, settings.SettingsViewModel.UseUnicode);
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

/// <summary>
/// Handles hex encoding.
/// </summary>
internal sealed class HexEncryptor : HexCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Hex encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the lowercase hex-encoded output</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                var bytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                output = Convert.ToHexStringLower(bytes);
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