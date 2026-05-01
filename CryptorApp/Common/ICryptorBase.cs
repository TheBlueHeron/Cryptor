using System.Windows.Controls;

namespace CryptorApp.Cryptors;

/// <summary>
/// Interface definition for base cryptor objects.
/// </summary>
public interface ICryptorBase : IDisposable
{
    #region Properties

    /// <summary>
    /// The <see cref="CryptMode"/> of the <see cref="ICryptor"/>.
    /// </summary>
    CryptMode Mode { get; }

    /// <summary>
    /// Gets the name of the object.
    /// </summary>
    string Name { get; }

    #endregion

    #region Methods and functions

    /// <summary>
    /// Gets the settings <see cref="UserControl"/>.
    /// </summary>
    Task<UserControl?> GetSettingsAsync();

    /// <summary>
    /// Determines whether the settings are valid.
    /// </summary>
    bool IsValid(ref string? msg);

    #endregion
}