using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CryptorApp;

/// <summary>
/// Applies the dark/light title-bar (DWM immersive dark mode) to a <see cref="Window"/>.
/// </summary>
internal static partial class DwmHelper
{
    #region Objects and variables

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    #endregion

    #region Methods and functions

    /// <summary>
    /// Sets the title-bar colour mode for the given <paramref name="window"/> to match <paramref name="dark"/>.
    /// Must be called after the window handle has been created (i.e. after <see cref="Window.SourceInitialized"/>).
    /// </summary>
    internal static void Apply(Window window, bool dark)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            return;
        }

        var value = dark ? 1 : 0;
        _ = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
    }

    #endregion
}