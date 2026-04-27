using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CryptorApp;

/// <summary>
/// Contains P/Invoke declarations.
/// </summary>
internal sealed partial class NativeMethods
{
    #region Objects and variables

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const uint WDA_NONE = 0x00000000;
    private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

    #endregion

    #region API

    /// <summary>
    /// Sets the title-bar colour mode for the given <paramref name="window"/> to match <paramref name="dark"/>.
    /// Must be called after the window handle has been created (i.e. after <see cref="Window.SourceInitialized"/>).
    /// </summary>
    public static void ApplyTheme(Window window, bool dark)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            return;
        }
        var value = dark ? 1 : 0;
        _ = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
    }

    /// <summary>
    /// Sets whether the specified <paramref name="window"/> is included in screen capture operations.
    /// </summary>
    /// <remarks>This method can be used to prevent a window from appearing in screen captures.
    /// The effect may depend on the operating system version and user permissions.</remarks>
    /// <param name="handle">The window</param>
    /// <param name="include"><see langword="true"/> to include the window in screen capture operations; <see langword="false"/> to exclude it</param>
    /// <returns><see langword="true"/> if the operation succeeds; otherwise, <see langword="false"/>.</returns>
    public static bool SetWindowIncludeInCapture(Window window, bool include)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
        {
            return false;
        }
        return SetWindowDisplayAffinity(hwnd, include ? WDA_NONE : WDA_EXCLUDEFROMCAPTURE);
    }

    #endregion
}