using System.Web;
using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Handles url decoding.
/// </summary>
internal sealed class UrlDecryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Url Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the input string.
    /// </summary>
    /// <param name="input">The url-encoded text to decode</param>
    /// <returns>A <see langword="string"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        return new CryptResult { Output = HttpUtility.UrlDecode(input) };
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
/// Handles url encoding.
/// </summary>
internal sealed class UrlEncryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Url Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Url encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        return new CryptResult { Output = HttpUtility.UrlEncode(input) };
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