using CryptorApp.Resources;
using System.IO;
using System.Security.Cryptography;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for AES-CNG (CBC mode) encryption and decryption.
/// </summary>
internal abstract class AesCngCryptor : CryptorBase
{
    #region Objects and variables

    private const int _IV_SIZE = 16; // 128-bit IV for AES-CBC — generated randomly per call
    private const string _VALIDATION = "Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).";

    protected static int IvSize => _IV_SIZE;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string Name => "Aes (CNG) CBC Mode";

    /// <inheritdoc/>
    protected override bool ShowKey => true;

    #endregion

    #region Public methods and functions

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// Requires a Key of 16, 24, or 32 bytes (8, 12, or 16 Unicode characters).
    /// The IV is generated randomly per operation and is not user-supplied.
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
/// Handles AES-CNG (CBC mode) decryption.
/// Expects Base64-encoded input with layout: [16-byte IV][ciphertext].
/// </summary>
internal sealed class AesCngDecryptor : AesCngCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Decode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decrypts the AES-CNG (CBC) encoded input string.
    /// </summary>
    /// <param name="input">The Base64-encoded ciphertext (IV prepended) to decrypt</param>
    /// <returns>A <see cref="CryptResult"/> containing the decrypted text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        byte[]? plainText = null;

        try
        {
            using var aesCng = new AesCng();
            var settings = GetSettings();

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var ivBytes    = inputBytes[..IvSize];
                    var cipherText = inputBytes[IvSize..];

                    var decryptor = aesCng.CreateDecryptor(keyBytes, ivBytes);
                    using var memStream   = new MemoryStream(cipherText);
                    using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                    using var memOutput   = new MemoryStream();
                    await cryptoStream.CopyToAsync(memOutput);
                    plainText = memOutput.ToArray();
                    output = Crypt.BytesToString(plainText, settings.SettingsViewModel.UseUnicode);
                }
                finally
                {
                    Array.Clear(keyBytes);
                    if (plainText is not null)
                    {
                        Array.Clear(plainText);
                    }
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
/// Handles AES-CNG (CBC mode) encryption.
/// Output layout (Base64-encoded): [16-byte IV][ciphertext].
/// A fresh random IV is generated for each call.
/// </summary>
internal sealed class AesCngEncryptor : AesCngCryptor, ICryptor
{
    #region Properties

    /// <inheritdoc/>
    public override CryptMode Mode => CryptMode.Encode;

    #endregion

    #region Methods and functions

    /// <summary>
    /// AES-CNG (CBC) encrypts the input string. A fresh random IV is generated for each call.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded output (IV + ciphertext)</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        var plainText = Array.Empty<byte>();

        try
        {
            using var aesCng = new AesCng();
            var settings = GetSettings();

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    plainText     = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var ivBytes   = RandomNumberGenerator.GetBytes(IvSize);
                    var encryptor = aesCng.CreateEncryptor(keyBytes, ivBytes);

                    using var memOutput    = new MemoryStream();
                    using var cryptoStream = new CryptoStream(memOutput, encryptor, CryptoStreamMode.Write);
                    await cryptoStream.WriteAsync(plainText);
                    await cryptoStream.FlushFinalBlockAsync();

                    // Layout: IV + ciphertext
                    var cipherText = memOutput.ToArray();
                    var combined   = new byte[IvSize + cipherText.Length];
                    ivBytes.CopyTo(combined, 0);
                    cipherText.CopyTo(combined, IvSize);
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