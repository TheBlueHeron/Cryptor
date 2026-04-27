using System.Security;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptorApp.ViewModels;

/// <summary>
/// Represents a view model that manages cryptographic settings, including the initialization vector and secret key.
/// </summary>
/// <remarks>This view model stores sensitive information as SecureString instances to help protect them in memory.</remarks>
public partial class SettingsViewModel : ObservableObject, IDisposable
{
    #region Properties

    /// <summary>
    /// Gets or sets the Initialization vector.
    /// </summary>
    public SecureString Iv { get; set; } = new SecureString();

    /// <summary>
    /// Gets or sets the secret key.
    /// </summary>
    public SecureString Key { get; set; } = new SecureString();

    /// <summary>
    /// Gets or sets a <see langword="bool"/>, signifying whether to show the Key setting.
    /// </summary>
    public bool ShowKey { get; set; }

    /// <summary>
    /// Gets or sets a <see langword="bool"/>, signifying whether to show the IV setting.
    /// </summary>
    public bool ShowIv { get; set; }

    /// <summary>
    /// Gets or sets a <see langword="bool"/>, signifying whether to use unicode when Base64 encoding input and output strings.
    /// </summary>
    public bool UseUnicode { get; set; }

    #endregion

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        Key?.Dispose();
        Iv?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}