using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Interface definition for objects that encode or decode text.
/// </summary>
public interface ICryptor : IDisposable
{
    /// <summary>
    /// Converts the input string.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output and possibly an error message.</returns>
    Task<CryptResult> ConvertAsync(string input);

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    Task<UserControl?> GetSettingsAsync();

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// </summary>
    bool IsValid(ref string? msg);

    /// <summary>
    /// The <see cref="CryptMode"/> of the <see cref="ICryptor">.
    /// </summary>
    CryptMode Mode { get; }

    /// <summary>
    /// Gets the name of the cryptor.
    /// </summary>
    string Name { get; }
}