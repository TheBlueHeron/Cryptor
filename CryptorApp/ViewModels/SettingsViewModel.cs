using System.Security;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptorApp.ViewModels;

/// <summary>
/// Represents a view model that manages cryptographic settings, including the initialization vector and secret key.
/// </summary>
/// <remarks>This view model stores sensitive information as SecureString instances to help protect them in memory.</remarks>
public partial class SettingsViewModel : ObservableObject
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

    #endregion
}