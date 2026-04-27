using System.Windows;
using Microsoft.Win32;

namespace CryptorApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Objects and variables

    private const string mKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string mKeyValue = "AppsUseLightTheme";
    private const string mLightColors = "Themes/Win11LightColors.xaml";
    private const string mDarkColors  = "Themes/Win11DarkColors.xaml";

    #endregion

    #region Events

    /// <summary>
    /// Applies the current system theme and subscribes to system preference changes.
    /// </summary>
    /// <param name="e">The <see cref="StartupEventArgs"/></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ApplyTheme(IsSystemDark());
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    /// <summary>
    /// Unsubscribes from system preference changes and exits.
    /// </summary>
    /// <param name="e">The <see cref="ExitEventArgs"/></param>
    protected override void OnExit(ExitEventArgs e)
    {
        SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        base.OnExit(e);
    }

    /// <summary>
    /// Applies the appropriate theme when general preferences have been modified.
    /// </summary>
    /// <param name="e">The <see cref="UserPreferenceChangedEventArgs"/></param>
    private void OnUserPreferenceChanged(object _, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            Dispatcher.Invoke(() => ApplyTheme(IsSystemDark()));
        }
    }

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Applies the specified theme by loading the appropriate resource dictionary and applying it to the instantiated application windows.
    /// </summary>
    /// <param name="dark"><see langword="true"/> to apply the dark theme; <see langword="false"/> to apply the light theme.</param>
    private void ApplyTheme(bool dark)
    {
        var uri = new Uri(dark ? mDarkColors : mLightColors, UriKind.Relative);
        Resources.MergedDictionaries[0] = new ResourceDictionary { Source = uri };

        foreach (Window window in Windows)
        {
            NativeMethods.ApplyTheme(window, dark);
        }
    }

    /// <summary>
    /// Determines whether the system is using the dark theme by reading the Windows registry.
    /// </summary>
    /// <returns><see langword="true"/> if dark theme is enabled; otherwise, <see langword="false"/>.</returns>
    internal static bool IsSystemDark()
    {        
        using var key = Registry.CurrentUser.OpenSubKey(mKeyPath);
        return key?.GetValue(mKeyValue) is int value && value == 0; // AppsUseLightTheme: 0 = dark, 1 = light (missing key → assume light)
    }

    #endregion
}