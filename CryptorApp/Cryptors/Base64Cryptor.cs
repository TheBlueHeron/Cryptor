using CryptorApp.Resources;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for Base64 encryption and decryption.
/// </summary>
internal abstract class Base64Cryptor : IDisposable
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
/// Handles Base64 decoding.
/// </summary>
internal sealed class Base64Decryptor : Base64Cryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Base64 Decryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decodes the input string.
    /// </summary>
    /// <param name="input">The Base64 encoded text to decode</param>
    /// <returns>A <see langword="string"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        byte[] bytes;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

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
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

/// <summary>
/// Handles Base64 encoding.
/// </summary>
internal sealed class Base64Encryptor : Base64Cryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Base64 Encryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Base64 encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output and possibly an error message.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        byte[] bytes;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

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
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

