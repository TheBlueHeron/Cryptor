using CryptorApp.Resources;
using System.Security.Cryptography;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for AES-CCM (Counter with CBC-MAC) encryption and decryption.
/// AES-CCM is an AEAD cipher: it provides both confidentiality and integrity.
/// </summary>
internal abstract class AesCcmCryptor : CryptorBase
{
    #region Objects and variables

    private const int _NONCE_SIZE = 12; // 96-bit nonce for AES-CCM — generated randomly per call
    private const int _TAG_SIZE   = 16; // 128-bit authentication tag
    private const string _VALIDATION = "Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).";

    protected static int NonceSize => _NONCE_SIZE;
    protected static int TagSize   => _TAG_SIZE;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string Name => "Aes (CCM)";

    /// <inheritdoc/>
    protected override bool ShowKey => true;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).
    /// The nonce is generated randomly per operation and is not user-supplied.
    /// </summary>
    /// <param name="msg">Will contain a validation message if validation failed</param>
    public override bool IsValid(ref string? msg)
    {
        var settings = GetSettings();

        if (settings is null)
        {
            return false;
        }

        var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
        var valid = keyBytes.Length is 16 or 24 or 32;
        Array.Clear(keyBytes);

        if (!valid)
        {
            msg = _VALIDATION;
        }
        return valid;
    }

    #endregion
}

/// <summary>
/// Handles AES-CCM decryption.
/// Expects Base64-encoded input with layout: [12-byte nonce][16-byte tag][ciphertext].
/// </summary>
internal sealed class AesCcmDecryptor : AesCcmCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decrypts and authenticates the AES-CCM encoded input string.
    /// </summary>
    /// <param name="input">The Base64-encoded ciphertext (nonce + tag + ciphertext) to decrypt</param>
    /// <returns>A <see cref="CryptResult"/> containing the decrypted text</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        var plainText = Array.Empty<byte>();

        try
        {
            var settings = GetSettings();

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

                    using var aesCcm = new AesCcm(keyBytes);
                    aesCcm.Decrypt(nonce, cipherText, tag, plainText);
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
        return Task.FromResult(new CryptResult { Output = output, Error = msg });
    }

    #endregion
}

/// <summary>
/// Handles AES-CCM encryption.
/// Output layout (Base64-encoded): [12-byte nonce][16-byte tag][ciphertext].
/// A fresh random nonce is generated for each call.
/// </summary>
internal sealed class AesCcmEncryptor : AesCcmCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// AES-CCM encrypts and authenticates the input string. A fresh random nonce is generated for each call.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded output (nonce + tag + ciphertext)</returns>
    public Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        var plainText = Array.Empty<byte>();

        try
        {
            var settings = GetSettings();

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    plainText      = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var nonce      = RandomNumberGenerator.GetBytes(NonceSize);
                    var cipherText = new byte[plainText.Length];
                    var tag        = new byte[TagSize];

                    using var aesCcm = new AesCcm(keyBytes);
                    aesCcm.Encrypt(nonce, plainText, cipherText, tag);

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
        return Task.FromResult(new CryptResult { Output = output, Error = msg });
    }

    #endregion
}