using CryptorApp.Resources;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for AES-GCM encryption and decryption.
/// </summary>
internal abstract class AesCryptor : IDisposable
{
    #region Objects and variables

    private const int _NONCE_SIZE = 12; // 96-bit nonce for AES-GCM
    private const int _TAG_SIZE   = 16; // 128-bit authentication tag
    private const string _VALIDATION = "Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).";

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
        mSettings ??= new CryptSettings(showKey: true);
        return mSettings;
    }

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).
    /// The IV is generated randomly per operation and is not user-supplied.
    /// </summary>
    /// <param name="msg">Will contain a validation message if validation failed</param>
    public bool IsValid(ref string? msg)
    {
        if (mSettings is null)
        {
            return false;
        }

        var keyBytes = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Key);
        var valid = keyBytes.Length is 16 or 24 or 32;
        Array.Clear(keyBytes);

        if (!valid)
        {
            msg = _VALIDATION;
        }
        return valid;
    }

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public void Dispose() => mSettings?.Dispose();

    #endregion

    #region Protected helpers

    protected static int NonceSize => _NONCE_SIZE;
    protected static int TagSize => _TAG_SIZE;

    #endregion
}

/// <summary>
/// Handles AES-GCM decryption.
/// Expects Base64-encoded input with layout: [12-byte nonce][16-byte tag][ciphertext].
/// </summary>
internal sealed class AesDecryptor : AesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Aes (GCM) Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decrypts the AES-GCM encoded input string.
    /// </summary>
    /// <param name="input">The Base64-encoded ciphertext (nonce + tag + ciphertext) to decrypt</param>
    /// <returns>A <see cref="CryptResult"/> containing the decrypted text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        var plainText = Array.Empty<byte>();
        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var nonce      = inputBytes[..NonceSize];
                    var tag        = inputBytes[NonceSize..(NonceSize + TagSize)];
                    var cipherText = inputBytes[(NonceSize + TagSize)..];
                    plainText = new byte[cipherText.Length];

                    using var aesGcm = new AesGcm(keyBytes, TagSize);
                    aesGcm.Decrypt(nonce, cipherText, tag, plainText);
                    output = Crypt.BytesToString(plainText, settings.SettingsViewModel.UseUnicode);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(plainText);
                }
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
/// Handles AES-GCM encryption.
/// Output layout (Base64-encoded): [12-byte nonce][16-byte tag][ciphertext].
/// </summary>
internal sealed class AesEncryptor : AesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Aes (GCM) Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// AES-GCM encrypts the input string. A fresh random nonce is generated for each call.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded output (nonce + tag + ciphertext)</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        var plainText = Array.Empty<byte>();
        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    plainText      = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var nonce      = RandomNumberGenerator.GetBytes(NonceSize);
                    var cipherText = new byte[plainText.Length];
                    var tag        = new byte[TagSize];

                    using var aesGcm = new AesGcm(keyBytes, TagSize);
                    aesGcm.Encrypt(nonce, plainText, cipherText, tag);

                    // Layout: nonce + tag + ciphertext
                    var combined = new byte[NonceSize + TagSize + cipherText.Length];
                    nonce.CopyTo(combined, 0);
                    tag.CopyTo(combined, NonceSize);
                    cipherText.CopyTo(combined, NonceSize + TagSize);
                    output = Convert.ToBase64String(combined);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(plainText);
                }
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

