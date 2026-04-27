using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for hex encoding and decoding.
/// </summary>
internal abstract class HexCryptor : IDisposable
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

    #region Methods and functions

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
/// Handles hex decoding.
/// </summary>
internal sealed class HexDecryptor : HexCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Hex Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the hex-encoded input string.
    /// </summary>
    /// <param name="input">The hex-encoded text to decode</param>
    /// <returns>A <see langword="string"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var bytes = Convert.FromHexString(input);
                output = Crypt.BytesToString(bytes, settings.SettingsViewModel.UseUnicode);
            }
        }
        catch
        {
            msg = Constants.errConvert;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

/// <summary>
/// Handles hex encoding.
/// </summary>
internal sealed class HexEncryptor : HexCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Hex Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Hex encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the lowercase hex-encoded output.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var bytes = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                output = Convert.ToHexStringLower(bytes);
            }
        }
        catch
        {
            msg = Constants.errConvert;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}
