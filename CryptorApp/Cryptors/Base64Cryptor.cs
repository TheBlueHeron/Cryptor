using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Handles Base64 decoding.
/// </summary>
internal sealed class Base64Decryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Base64 Decryption";

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
        try
        {
            bytes = Convert.FromBase64String(input);
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            bytes = [];
        }
        return new CryptResult { Output = Crypt.BytesToString(bytes), Error = msg };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid(ref string? msg) => true;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}

/// <summary>
/// Handles Base64 encoding.
/// </summary>
internal sealed class Base64Encryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Base64 Encryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Base64 encodes the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output and possibly an error message.</returns>
    public async Task<CryptResult> ConvertAsync(string input)
    {
        return new CryptResult { Output = Convert.ToBase64String(Crypt.StringToBytes(input)) };
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid(ref string? msg) => true;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}