using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptorApp.Cryptors;

namespace CryptorApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Objects and variables

    private readonly ObservableCollection<ICryptor> mConverters = [new Base64Decryptor(), new Base64Encryptor()];

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

    [ObservableProperty]
    public partial ICryptor? CurrentCryptor { get; set; } = null;

    [ObservableProperty]
    public partial UserControl? CurrentSettings { get; set; } = null;

    [ObservableProperty]
    public partial string Input { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Output { get; set; } = string.Empty;

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

    #endregion

    #region Private methods and functions

    /// <summary>
    /// Converts the input text, if possible.
    /// </summary>
    private async Task HandleConvert()
    {
        if (CurrentCryptor is ICryptor cryptor && cryptor.IsValid && !string.IsNullOrEmpty(Input))
        {
            Output = await cryptor.ConvertAsync(Input);
        }
    }

    /// <summary>
    /// Sets the current settings to the selected <see cref="ICryptor"/>'s settings, if available.
    /// </summary>
    private async Task HandleCryptorChange()
    {
        CurrentSettings = null;
        if (CurrentCryptor is not null)
        {
            CurrentSettings = await CurrentCryptor.GetSettingsAsync();
        }
    }

    #endregion
}