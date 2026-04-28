using CryptorApp.Resources;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for ChaCha20-Poly1305 encryption and decryption.
/// </summary>
internal abstract class ChaCha20Cryptor : IDisposable
{
    #region Objects and variables

    private const int _KEY_SIZE   = 32; // 256-bit key
    protected const int _NONCE_SIZE = 12; // 96-bit nonce — generated randomly per call
    protected const int _TAG_SIZE   = 16; // 128-bit authentication tag

    private const string _VALIDATION = "Requires a Key of 16 Unicode characters (32 bytes).";

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
    /// Requires a Key of exactly 32 bytes (16 Unicode characters).
    /// The nonce is generated randomly per operation and is not user-supplied.
    /// </summary>
    /// <param name="msg">Will contain a validation message if validation failed</param>
    public bool IsValid(ref string? msg)
    {
        if (mSettings is null)
        {
            return false;
        }

        var keyBytes = Crypt.SecureStringToBytes(mSettings.SettingsViewModel.Key);
        var valid = keyBytes.Length == _KEY_SIZE;
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
}

/// <summary>
/// Handles ChaCha20-Poly1305 decryption.
/// Expects Base64-encoded input with layout: [12-byte nonce][16-byte tag][ciphertext].
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
    /// <param name="input">The Base64-encoded encrypted text (nonce + tag + ciphertext) to decrypt</param>
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
                    var nonce      = inputBytes[.._NONCE_SIZE];
                    var tag        = inputBytes[_NONCE_SIZE..(_NONCE_SIZE + _TAG_SIZE)];
                    var cipherText = inputBytes[(_NONCE_SIZE + _TAG_SIZE)..];
                    plainText = new byte[cipherText.Length];

                    using var chacha = new ChaCha20Poly1305(keyBytes);
                    chacha.Decrypt(nonce, cipherText, tag, plainText);
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
/// Handles ChaCha20-Poly1305 encryption.
/// Output layout (Base64-encoded): [12-byte nonce][16-byte tag][ciphertext].
/// A fresh random nonce is generated for each call.
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
    /// A fresh random nonce is generated for each call.
    /// Output layout: nonce + tag + ciphertext, Base64-encoded.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/> containing the Base64-encoded encrypted output</returns>
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
                    var nonce      = RandomNumberGenerator.GetBytes(_NONCE_SIZE);
                    var cipherText = new byte[plainText.Length];
                    var tag        = new byte[_TAG_SIZE];

                    using var chacha = new ChaCha20Poly1305(keyBytes);
                    chacha.Encrypt(nonce, plainText, cipherText, tag);

                    // Layout: nonce + tag + ciphertext
                    var combined = new byte[_NONCE_SIZE + _TAG_SIZE + cipherText.Length];
                    nonce.CopyTo(combined, 0);
                    tag.CopyTo(combined, _NONCE_SIZE);
                    cipherText.CopyTo(combined, _NONCE_SIZE + _TAG_SIZE);
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

