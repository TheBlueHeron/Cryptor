using System.Web;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Html encoding and decoding.
/// </summary>
internal abstract class HtmlCryptor : CryptorBase
{
    #region Properties

    /// <inheritdoc/>
    public override string Name => "Html";

    #endregion
}

/// <summary>
/// Handles html decoding.
/// </summary>
internal sealed class HtmlDecryptor : HtmlCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the html-encoded input string.
    /// </summary>
    /// <param name="input">The html-encoded text to decode</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public Task<CryptResult> ConvertAsync(string input) => Task.FromResult(new CryptResult { Output = HttpUtility.HtmlDecode(input) });

    #endregion
}

/// <summary>
/// Handles html encoding.
/// </summary>
internal sealed class HtmlEncryptor : HtmlCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Html encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the html-encoded text</returns>
    public Task<CryptResult> ConvertAsync(string input) => Task.FromResult(new CryptResult { Output = HttpUtility.HtmlEncode(input) });

    #endregion
}