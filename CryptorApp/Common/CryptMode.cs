namespace CryptorApp;

/// <summary>
/// Enumeration of possible <see cref="Cryptors.ICryptor"> modes.
/// </summary>
public enum CryptMode
{
    /// <summary>
    /// The <see cref="Cryptors.ICryptor"> decrypts or decodes encrypted or encoded text.
    /// </summary>
    Decode = 0,
    /// <summary>
    /// The <see cref="Cryptors.ICryptor"> encrypts or encodes plain text.
    /// </summary>
    Encode = 1
}