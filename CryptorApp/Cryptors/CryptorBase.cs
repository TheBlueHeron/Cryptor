using System.Windows.Controls;
using CryptorApp.Views;

namespace CryptorApp.Cryptors;

/// <summary>
/// Abstract base class for all cryptor implementations.
/// Handles lazy-initialised settings, disposal, and validation boilerplate.
/// </summary>
internal abstract class CryptorBase : ICryptorBase
{
    #region Objects and variables

    private CryptSettings? mSettings;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public abstract CryptMode Mode { get; }

    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <summary>
    /// Whether the settings panel should show the Key input row. Default is <see langword="false"/>.
    /// </summary>
    protected virtual bool ShowKey => false;

    #endregion

    #region Public methods and functions

    /// <inheritdoc/>
    public Task<UserControl?> GetSettingsAsync()
    {
        mSettings ??= new CryptSettings(ShowKey);
        return Task.FromResult<UserControl?>(mSettings);
    }

    /// <inheritdoc/>
    public virtual bool IsValid(ref string? msg) => true;

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public void Dispose()
    {
        mSettings?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Protected methods and functions

    /// <summary>
    /// Returns the <see cref="CryptSettings"/> instance.
    /// </summary>
    protected CryptSettings? GetSettings()
    {
        mSettings ??= new CryptSettings(ShowKey);
        return mSettings;
    }

    #endregion
}