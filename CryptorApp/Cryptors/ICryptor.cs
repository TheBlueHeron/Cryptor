using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Interface definition for objects that encode or decode text.
/// </summary>
public interface ICryptor
{
    /// <summary>
    /// Converts the input string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>A <see langword="string"/></returns>
    Task<string> ConvertAsync(string input);

    /// <summary>
    /// Gets the settings <see cref="UserControl"/> for this <see cref="ICryptor"/>.
    /// </summary>
    Task<UserControl?> GetSettingsAsync();

    /// <summary>
    /// Determines whether the <see cref="ICryptor"/>'s settings are valid.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets the name of the cryptor.
    /// </summary>
    string Name { get; }
}