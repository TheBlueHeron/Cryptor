namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Url encoding and decoding.
/// </summary>
internal abstract class UrlCryptor : CryptorBase
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Url";

    #endregion
}

/// <summary>
/// Handles url decoding.
/// </summary>
internal sealed class UrlDecryptor : UrlCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the url-encoded input string.
    /// </summary>
    /// <param name="input">The url-encoded text to decode</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public Task<CryptResult> ConvertAsync(string input) => Task.FromResult(new CryptResult { Output = Uri.UnescapeDataString(input) });

    #endregion
}

/// <summary>
/// Handles url encoding.
/// </summary>
internal sealed class UrlEncryptor : UrlCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Url encodes the input string (RFC 3986).
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the url-encoded text</returns>
    public Task<CryptResult> ConvertAsync(string input) => Task.FromResult(new CryptResult { Output = Uri.EscapeDataString(input) });

    #endregion
}