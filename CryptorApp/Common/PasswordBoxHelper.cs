using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace CryptorApp.Views;

/// <summary>
/// Attached behavior that enables binding of <see cref="PasswordBox.SecurePassword"/> to a ViewModel property without exposing the password as a plain string.
/// </summary>
internal static class PasswordBoxHelper
{
    #region Properties

    /// <summary>
    /// Attached property that holds the <see cref="SecureString"/> value, bound to a ViewModel property.
    /// </summary>
    public static readonly DependencyProperty SecurePasswordProperty = DependencyProperty.RegisterAttached("SecurePassword", typeof(SecureString), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Attached property to enable monitoring of <see cref="PasswordBox.PasswordChanged"/>.
    /// Set to <see langword="true"/> on a <see cref="PasswordBox"/> to activate the behavior.
    /// </summary>
    public static readonly DependencyProperty MonitorPasswordProperty = DependencyProperty.RegisterAttached("MonitorPassword", typeof(bool), typeof(PasswordBoxHelper), new UIPropertyMetadata(false, OnMonitorPasswordChanged));

    #endregion

    #region Methods and functions

    /// <summary>
    /// Gets the <see cref="SecurePasswordProperty"/> value.
    /// </summary>
    public static SecureString GetSecurePassword(PasswordBox element) => (SecureString)element.GetValue(SecurePasswordProperty);

    /// <summary>
    /// Sets the <see cref="SecurePasswordProperty"/> value.
    /// </summary>
    public static void SetSecurePassword(PasswordBox element, SecureString value) => element.SetValue(SecurePasswordProperty, value);

    /// <summary>
    /// Gets the <see cref="MonitorPasswordProperty"/> value.
    /// </summary>
    public static bool GetMonitorPassword(PasswordBox element) => (bool)element.GetValue(MonitorPasswordProperty);

    /// <summary>
    /// Sets the <see cref="MonitorPasswordProperty"/> value.
    /// </summary>
    public static void SetMonitorPassword(PasswordBox element, bool value) => element.SetValue(MonitorPasswordProperty, value);

    /// <summary>
    /// Handles changes to the MonitorPassword attached property by subscribing or unsubscribing to the PasswordChanged event of a PasswordBox.
    /// </summary>
    /// <param name="d">The dependency object on which the property value has changed. Expected to be a PasswordBox</param>
    /// <param name="e">The event data that contains information about the property change, including the old and new values</param>
    private static void OnMonitorPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox)
        {
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += OnPasswordChanged;
            }
            else
            {
                passwordBox.PasswordChanged -= OnPasswordChanged;
            }
        }
    }

    /// <summary>
    /// Handles the PasswordChanged event for a PasswordBox control and updates the associated secure password.
    /// </summary>
    /// <param name="sender">The source of the event, expected to be a PasswordBox instance</param>
    /// <param name="e">The event data associated with the password change</param>
    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            SetSecurePassword(passwordBox, passwordBox.SecurePassword);
        }
    }

    #endregion
}
