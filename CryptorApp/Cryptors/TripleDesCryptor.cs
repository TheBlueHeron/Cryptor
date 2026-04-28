using CryptorApp.Resources;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Base class for triple des encryption and decryption.
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal abstract class TripleDesCryptor : IDisposable
{
    #region Objects and variables

    private const int _IV_SIZE = 8; // 64-bit IV for TripleDES — generated randomly per call
    private const string _VALIDATION = "Requires a non-weak Key of 8 or 12 characters.";

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
    /// Requires a Key of 16 or 24 bytes, a non-weak key.
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
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
        var valid = keyBytes.Length is 16 or 24 && !TripleDES.IsWeakKey(keyBytes);
#pragma warning restore CA5350
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

    protected static int IvSize => _IV_SIZE;

    #endregion
}
#pragma warning restore CA5350

/// <summary>
/// Handles Triple DES decryption.
/// Expects Base64-encoded input with layout: [8-byte IV][ciphertext].
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal sealed class TripleDesDecryptor : TripleDesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Decode;

    /// <inheritdoc/>
    public override string Name => "Triple DES Decoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Decrypts the Triple DES encoded input string.
    /// </summary>
    /// <param name="input">The Base64-encoded ciphertext (IV prepended) to decrypt</param>
    /// <returns>A <see cref="CryptResult"/> containing the decoded text</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        string? msg = null;
        string? output = null;
        byte[]? plainText = null;
        try
        {
            using var tripleDes = TripleDES.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    var inputBytes = Convert.FromBase64String(input);
                    var ivBytes    = inputBytes[..IvSize];
                    var cipherText = inputBytes[IvSize..];

                    var decryptor = tripleDes.CreateDecryptor(keyBytes, ivBytes);
                    using var memStream = new MemoryStream(cipherText);
                    using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                    using var memOutput = new MemoryStream();
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
#pragma warning restore CA5350

/// <summary>
/// Handles Triple DES encryption.
/// Output layout (Base64-encoded): [8-byte IV][ciphertext].
/// A fresh random IV is generated for each call.
/// </summary>
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms - TripleDES support is intentional for compatibility
internal sealed class TripleDesEncryptor : TripleDesCryptor, ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the <see cref="CryptMode"/>.
    /// </summary>
    public CryptMode Mode => CryptMode.Encode;

    /// <inheritdoc/>
    public override string Name => "Triple DES Encoding";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Triple DES encrypts the input string. A fresh random IV is generated for each call.
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
            using var tripleDes = TripleDES.Create();
            var settings = (await GetSettingsAsync()) as CryptSettings;

            if (settings is not null)
            {
                var keyBytes = Crypt.SecureStringToBytes(settings.SettingsViewModel.Key);
                try
                {
                    plainText      = Crypt.StringToBytes(input, settings.SettingsViewModel.UseUnicode);
                    var ivBytes    = RandomNumberGenerator.GetBytes(IvSize);
                    var encryptor  = tripleDes.CreateEncryptor(keyBytes, ivBytes);

                    using var memOutput = new MemoryStream();
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
#pragma warning restore CA5350

