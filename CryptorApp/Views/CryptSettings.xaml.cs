using System.Windows.Controls;
using CryptorApp.ViewModels;

namespace CryptorApp.Views;

/// <summary>
/// Interaction logic for CryptSettings.xaml
/// </summary>
public partial class CryptSettings : UserControl
{
    #region Objects and variables

    private SettingsViewModel mSettingsViewModel = null!;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="CryptSettings"/> control.
    /// </summary>
    public CryptSettings()
    {
        InitializeComponent();
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