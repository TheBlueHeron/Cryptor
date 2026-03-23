using System.Collections.ObjectModel;
using System.Windows.Controls;
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

    private readonly ObservableCollection<ICryptor> mConverters = [new Base64Decryptor(), new Base64Encryptor(), new HtmlDecryptor(), new HtmlEncryptor(), new UrlDecryptor(), new UrlEncryptor(), new AesDecryptor(), new AesEncryptor()];

    private IAsyncRelayCommand mConvertCommand = null!;
    private IAsyncRelayCommand mSelectCommand = null!;

    #endregion

    #region Properties

    /// <summary>
    /// Returns a <see cref="IAsyncRelayCommand"/> that calls <see cref="HandleConvert"/>.
    /// </summary>
    public IAsyncRelayCommand Convert
    {
        get
        {
            mConvertCommand ??= new AsyncRelayCommand(HandleConvert);
            return mConvertCommand;
        }
    }

    /// <summary>
    /// Gets the available <see cref="ICryptor"/>s.
    /// </summary>
    public ObservableCollection<ICryptor> Converters => mConverters;

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
    /// Gets or sets the output string.
    /// </summary>
    [ObservableProperty]
    public partial string? Output { get; set; } = string.Empty;

    /// <summary>
    /// Returns a <see cref="IAsyncRelayCommand"/> that calls <see cref="HandleCryptorChange"/>.
    /// </summary>
    public IAsyncRelayCommand Select
    {
        get
        {
            mSelectCommand ??= new AsyncRelayCommand(HandleCryptorChange);
            return mSelectCommand;
        }
    }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    [ObservableProperty]
    public partial string Status { get; set; } = _ready;

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Converts the input text, if possible.
    /// </summary>
    private async Task HandleConvert()
    {
        if (CurrentCryptor is ICryptor cryptor && cryptor.IsValid && !string.IsNullOrEmpty(Input))
        {
            var rst = await cryptor.ConvertAsync(Input);
            Output = rst.Output;
            Status = rst.Error ?? _ready;
        }
        else
        {
            Output = string.Empty;
            Status = _ready;
        }
    }

    /// <summary>
    /// Sets the current settings to the selected <see cref="ICryptor"/>'s settings, if available.
    /// </summary>
    private async Task HandleCryptorChange()
    {
        CurrentSettings = null;
        Output = string.Empty;
        if (CurrentCryptor is not null)
        {
            CurrentSettings = await CurrentCryptor.GetSettingsAsync();
        }
    }

    #endregion
}