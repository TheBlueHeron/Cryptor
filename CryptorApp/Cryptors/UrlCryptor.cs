using System.Web;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Url encoding and decoding.
/// </summary>
internal abstract class UrlCryptor : IDisposable
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

    #region Public methods and functions

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync()
    {
        mSettings ??= new CryptSettings(false);
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
/// Handles url decoding.
/// </summary>
internal sealed class UrlDecryptor : UrlCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Url Decoding";

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

    #endregion
}

/// <summary>
/// Handles url encoding.
/// </summary>
internal sealed class UrlEncryptor : UrlCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Url Encoding";

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

    #endregion
}
