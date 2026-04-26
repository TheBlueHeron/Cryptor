using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for ChaCha20-Poly1305 encryption and decryption.
/// </summary>
internal abstract class ChaCha20Cryptor
{
    #region Objects and variables

    private const int _KEY_SIZE   = 32; // 256-bit key
    private const int _NONCE_SIZE = 12; // 96-bit nonce
    protected const int _TAG_SIZE   = 16; // 128-bit authentication tag

    private const string _VALIDATION = "Requires a Key of 16 characters and a Nonce (IV) of 6 characters.";

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
        mSettings ??= new CryptSettings(true);
        return mSettings;
    }

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// Requires a Key of exactly 32 bytes (16 Unicode characters) and a Nonce (IV) of exactly 12 bytes (6 Unicode characters).
    /// </summary>
    /// <param name="msg">Will contain a validation message if validation failed</param>
    public bool IsValid(ref string? msg)
    {
        if (mSettings is null)
        {
            return false;
        }

        var keyBytes   = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Key);
        var nonceBytes = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Iv);
        var valid = keyBytes.Length == _KEY_SIZE && nonceBytes.Length == _NONCE_SIZE;
        Array.Clear(keyBytes);
        Array.Clear(nonceBytes);

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

    #endregion
}

/// <summary>
/// Handles ChaCha20-Poly1305 decryption.
/// </summary>
internal sealed class ChaCha20Decryptor : ChaCha20Cryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "ChaCha20 Decryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decrypts the ChaCha20-Poly1305 encrypted input string.
    /// </summary>
    /// <param name="input">The Base64-encoded encrypted text (tag prepended) to decrypt</param>
    /// <returns>A <see cref="CryptResult"/> containing the decrypted text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var nonceBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var tag = inputBytes[.._TAG_SIZE];
                    var cipherText = inputBytes[_TAG_SIZE..];
                    var plainText = new byte[cipherText.Length];

                    using var chacha = new ChaCha20Poly1305(keyBytes);
                    chacha.Decrypt(nonceBytes, cipherText, tag, plainText);
                    output = Crypt.BytesToString(plainText, settings.SettingsViewModel.UseUnicode);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(nonceBytes);
                }
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}

/// <summary>
/// Handles ChaCha20-Poly1305 encryption.
/// </summary>
internal sealed class ChaCha20Encryptor : ChaCha20Cryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "ChaCha20 Encryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Encrypts the input string using ChaCha20-Poly1305.
    /// The 16-byte authentication tag is prepended to the ciphertext before Base64 encoding.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded encrypted output</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;

        try
        {
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                var nonceBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Iv);
                try
                {
                    var plainText = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var cipherText = new byte[plainText.Length];
                    var tag = new byte[_TAG_SIZE];

                    using var chacha = new ChaCha20Poly1305(keyBytes);
                    chacha.Encrypt(nonceBytes, plainText, cipherText, tag);

                    // Prepend the tag to the ciphertext so the decryptor can recover it
                    var combined = new byte[tag.Length + cipherText.Length];
                    tag.CopyTo(combined, 0);
                    cipherText.CopyTo(combined, tag.Length);
                    output = Convert.ToBase64String(combined);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    Array.Clear(nonceBytes);
                }
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        return new CryptResult { Output = output, Error = msg };
    }

    #endregion
}