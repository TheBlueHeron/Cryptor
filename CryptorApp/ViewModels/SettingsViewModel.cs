using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptorApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    #region Properties

    /// <summary>
    /// Gets or sets the Initialization vector.
    /// </summary>
    [ObservableProperty]
    public partial string Iv { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret key.
    /// </summary>
    [ObservableProperty]
    public partial string Key { get; set; } = string.Empty;

    #endregion
}