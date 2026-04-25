using System.Windows;
using CryptorApp.ViewModels;

namespace CryptorApp.Views;

/// <summary>
/// The main application window.
/// </summary>
public partial class MainWindow : Window
{
    #region Objects and variables

    private MainViewModel mMainViewModel = null!;

    #endregion

    #region Construction

    /// <summary>
    /// Creates a new <see cref="MainWindow"/>.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    /// <summary>
    /// Applies the Desktop Window Manager theme based on the current system theme.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/></param>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        DwmHelper.Apply(this, App.IsSystemDark());
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns the <see cref="CryptorApp.MainViewModel"/>.
    /// </summary>
    public MainViewModel MainViewModel
    {
        get
        {
            mMainViewModel ??= new();
            return mMainViewModel;
        }
    }

    #endregion
}