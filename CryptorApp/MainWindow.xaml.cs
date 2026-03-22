using System.Windows;
using CryptorApp.ViewModels;

namespace CryptorApp;

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
}