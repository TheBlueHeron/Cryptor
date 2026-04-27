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

    #region Events

    /// <summary>
    /// Disposes the <see cref="MainViewModel"/> and all cryptors when the window closes.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        mMainViewModel?.Dispose();
    }

    /// <summary>
    /// Applies the Desktop Window Manager theme based on the current system theme and excludes this window from capture.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/></param>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        NativeMethods.ApplyTheme(this, App.IsSystemDark());
        NativeMethods.SetWindowIncludeInCapture(this, false);
    }

    #endregion
}