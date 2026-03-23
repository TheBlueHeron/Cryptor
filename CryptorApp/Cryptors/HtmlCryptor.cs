using System.Web;
using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Handles html decoding.
/// </summary>
internal sealed class HtmlDecryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Html Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the input string.
    /// </summary>
    /// <param name="input">The html-encoded text to decode</param>
    /// <returns>A <see langword="string"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        return new CryptResult { Output = HttpUtility.HtmlDecode(input) };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}

/// <summary>
/// Handles html encoding.
/// </summary>
internal sealed class HtmlEncryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Html Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Html encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        return new CryptResult { Output = HttpUtility.HtmlEncode(input) };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}