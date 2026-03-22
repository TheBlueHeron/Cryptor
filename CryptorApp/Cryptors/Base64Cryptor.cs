using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Handles Base64 decoding.
/// </summary>
internal sealed class Base64Decryptor : ICryptor
{
    #region Properties

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

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
    public async Task<string> ConvertAsync(string input)
    {
        //
        return input;
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

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
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid. Always <see langword="true"/>.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the name of the <see cref="ICryptor"/>.
    /// </summary>
    public string Name => "Base64 Encryption";

    #endregion

    #region Methods and functions

    /// <summary>
    /// Encodes the input string.
    /// </summary>
    /// <param name="input">The plain text to encode</param>
    /// <returns>A <see langword="string"/> containing the encoded text</returns>
    public async Task<string> ConvertAsync(string input)
    {
        //
        return input;
    }

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>. Returns <see langword="null"/>.
    /// </summary>
    public async Task<UserControl?> GetSettingsAsync() => null;

    /// <summary>
    /// Returns the <see cref="Name"/> value.
    /// </summary>
    public override string ToString() => Name;

    #endregion
}