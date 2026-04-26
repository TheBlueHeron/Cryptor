using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptorApp.Cryptors;

namespace CryptorApp.ViewModels;

/// <summary>
/// The viewmodel for the main application window (<see cref="MainWindow"/>).
/// </summary>
public partial class MainViewModel : ObservableObject
{
    #region Objects and variables

    private const string _ready = "Ready";
    private static readonly Brush mFallbackNormal = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
    private static readonly Brush mFallbackError  = new SolidColorBrush(Color.FromRgb(0xC4, 0x2B, 0x1C));

    private readonly ObservableCollection<ICryptor> mAllConverters;

    private IAsyncRelayCommand mConvertCommand = null!;
    private IAsyncRelayCommand<CryptMode> mModeCommand = null!;
    private IAsyncRelayCommand mSelectCommand = null!;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes the <see cref="ICryptor"/> collections.
    /// </summary>
    public MainViewModel()
    {
        mAllConverters = [new Base64Decryptor(), new Base64Encryptor(), new HexDecryptor(), new HexEncryptor(), new HtmlDecryptor(), new HtmlEncryptor(), new UrlDecryptor(), new UrlEncryptor(), new AesDecryptor(), new AesEncryptor(), new ChaCha20Decryptor(), new ChaCha20Encryptor(), new TripleDesDecryptor(), new TripleDesEncryptor(), new Sha256Encryptor(), new Sha512Encryptor()];
        Converters = new ObservableCollection<ICryptor>(mAllConverters.Where(c => c.Mode == CryptMode.Decode));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns a <see cref="IAsyncRelayCommand"/> that calls <see cref="Convert"/>.
    /// </summary>
    public IAsyncRelayCommand ConvertCommand
    {
        get
        {
            mConvertCommand ??= new AsyncRelayCommand(Convert);
            return mConvertCommand;
        }
    }

    /// <summary>
    /// Gets the available <see cref="ICryptor"/>s.
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<ICryptor> Converters { get; set; }

    /// <summary>
    /// Gets or sets the currently selected <see cref="ICryptor"/> implementation.
    /// </summary>
    [ObservableProperty]
    public partial ICryptor? CurrentCryptor { get; set; } = null;

    /// <summary>
    /// Gets or sets the current settings <see cref="UserControl"/>, which is the result of <see cref="CurrentCryptor"/>'s <see cref="ICryptor.GetSettingsAsync()"/>.
    /// </summary>
    [ObservableProperty]
    public partial UserControl? CurrentSettings { get; set; } = null;

    /// <summary>
    /// Gets or sets the input string.
    /// </summary>
    [ObservableProperty]
    public partial string Input { get; set; } = string.Empty;

    /// <summary>
    /// Returns a <see cref="IAsyncRelayCommand"/> that calls <see cref="HandleModeChange"/>.
    /// </summary>
    public IAsyncRelayCommand<CryptMode> ModeCommand
    {
        get
        {
            mModeCommand ??= new AsyncRelayCommand<CryptMode>(HandleModeChange);
            return mModeCommand;
        }
    }

    /// <summary>
    /// Gets or sets the output string.
    /// </summary>
    [ObservableProperty]
    public partial string? Output { get; set; } = string.Empty;

    /// <summary>
    /// Returns a <see cref="IAsyncRelayCommand"/> that calls <see cref="HandleTypeChange"/>.
    /// </summary>
    public IAsyncRelayCommand SelectCommand
    {
        get
        {
            mSelectCommand ??= new AsyncRelayCommand(HandleTypeChange);
            return mSelectCommand;
        }
    }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    [ObservableProperty]
    public partial string Status { get; set; } = _ready;

    /// <summary>
    /// Gets the brush to use for the status text — error colour when <see cref="Status"/> contains an error, secondary foreground otherwise.
    /// </summary>
    [ObservableProperty]
    public partial Brush StatusColor { get; set; } = ResolveStatusBrush(false);

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Converts the input text, if possible.
    /// </summary>
    private async Task Convert()
    {
        if (CurrentCryptor is ICryptor cryptor && !string.IsNullOrEmpty(Input))
        {
            var msg = string.Empty;
            if (cryptor.IsValid(ref msg))
            {
                var rst = await cryptor.ConvertAsync(Input);
                Output = rst.Output;
                SetStatus(rst.Error, isError: rst.Error is not null);
            }
            else
            {
                Output = string.Empty;
                SetStatus(msg, isError: true);
            }
        }
        else
        {
            Output = string.Empty;
            SetStatus(null);
        }
    }

    /// <summary>
    /// Sets the current settings to the selected <see cref="ICryptor"/>'s settings, if available.
    /// </summary>
    private async Task HandleModeChange(CryptMode mode)
    {
        var mPrevIndex = -1;

        if (CurrentCryptor is not null) // remember index to be able to select its equivalent after changing the source collection
        {
            mPrevIndex = Converters.IndexOf(CurrentCryptor);
        }
        CurrentCryptor = null;
        CurrentSettings = null;
        Input = string.Empty;
        Output = string.Empty;
        Converters = new ObservableCollection<ICryptor>(mAllConverters.Where(c => c.Mode == mode));
        if (mPrevIndex >= 0 && Converters.Count > mPrevIndex)
        {
            CurrentCryptor = Converters.ElementAt(mPrevIndex);
        }
    }

    /// <summary>
    /// Sets the current settings to the selected <see cref="ICryptor"/>'s settings, if available.
    /// </summary>
    private async Task HandleTypeChange()
    {
        CurrentSettings = null;
        Output = string.Empty;
        if (CurrentCryptor is not null)
        {
            CurrentSettings = await CurrentCryptor.GetSettingsAsync();
        }
    }

    /// <summary>
    /// Resolves the appropriate status brush from the active theme resources.
    /// </summary>
    private static Brush ResolveStatusBrush(bool isError) =>
        isError
            ? (Application.Current.Resources["ErrorBrush"] as Brush ?? mFallbackError)
            : (Application.Current.Resources["SecondaryForegroundBrush"] as Brush ?? mFallbackNormal);

    /// <summary>
    /// Updates the status message and color.
    /// </summary>
    /// <param name="message">The status message to display. If <see langword="null"/>, uses the default ready message</param>
    /// <param name="isError"><see langword="true"/> if the status represents an error; otherwise, <see langword="false"/></param>
    private void SetStatus(string? message, bool isError = false)
    {
        Status = message ?? _ready;
        StatusColor = ResolveStatusBrush(isError);
    }

    #endregion
}