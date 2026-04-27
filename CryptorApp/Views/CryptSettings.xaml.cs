using System.Windows.Controls;
using CryptorApp.ViewModels;

namespace CryptorApp.Views;

/// <summary>
/// Interaction logic for CryptSettings.xaml
/// </summary>
public partial class CryptSettings : UserControl, IDisposable
{
    #region Objects and variables

    private SettingsViewModel mSettingsViewModel = null!;

    #endregion

    #region Construction and destruction

    /// <summary>
    /// Creates a new <see cref="CryptSettings"/> control.
    /// </summary>
    /// <param name="showKey">Whether to show the Key input row</param>
    /// <param name="showIv">Whether to show the IV input row</param>
    public CryptSettings(bool showKey, bool showIv = false)
    {
        SettingsViewModel.ShowKey = showKey;
        SettingsViewModel.ShowIv = showIv;
        InitializeComponent();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        mSettingsViewModel?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns the <see cref="CryptorApp.SettingsViewModel"/>.
    /// </summary>
    public SettingsViewModel SettingsViewModel
    {
        get
        {
            mSettingsViewModel ??= new();
            return mSettingsViewModel;
        }
    }

    #endregion
}