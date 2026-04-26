# Cryptor

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-14.0-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D4?logo=windows)](https://www.microsoft.com/windows)
[![WPF](https://img.shields.io/badge/UI-WPF-0078D4)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Stars](https://img.shields.io/github/stars/TheBlueHeron/Cryptor?style=flat)](https://github.com/TheBlueHeron/Cryptor/stargazers)
[![Forks](https://img.shields.io/github/forks/TheBlueHeron/Cryptor?style=flat)](https://github.com/TheBlueHeron/Cryptor/network)
[![Issues](https://img.shields.io/github/issues/TheBlueHeron/Cryptor)](https://github.com/TheBlueHeron/Cryptor/issues)
[![Last Commit](https://img.shields.io/github/last-commit/TheBlueHeron/Cryptor/master)](https://github.com/TheBlueHeron/Cryptor/commits/master)
[![License](https://img.shields.io/github/license/TheBlueHeron/Cryptor)](https://github.com/TheBlueHeron/Cryptor/blob/master/LICENSE.txt)

A lightweight WPF desktop application for encoding, decoding, encrypting, and decrypting text using a variety of algorithms — all from a single, clean interface.

---

## ✨ Features

| Algorithm          | Encode / Encrypt | Decode / Decrypt |
|--------------------|:----------------:|:----------------:|
| Base64             | ✅               | ✅               |
| Hex                | ✅               | ✅               |
| HTML               | ✅               | ✅               |
| URL                | ✅               | ✅               |
| AES                | ✅               | ✅               |
| ChaCha20-Poly1305  | ✅               | ✅               |
| Triple DES         | ✅               | ✅               |
| SHA-256            | ✅               | —                |
| SHA-512            | ✅               | —                |

- **MVVM architecture** — built with [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- **Secure key input** — AES, Triple DES, and ChaCha20-Poly1305 keys and IVs/nonces are entered via `PasswordBox` and stored as `SecureString`; intermediate byte arrays are zeroed immediately after use
- **Windows 11 UI theme** — custom implicit styles modelled on the Windows 11 design language (rounded controls, accent blue, Segoe UI Variable typography, slim scrollbars)
- **Automatic dark / light mode** — reads `AppsUseLightTheme` from the Windows registry on startup and switches live whenever the user changes the system preference in Settings; the native title bar follows via the DWM `DWMWA_USE_IMMERSIVE_DARK_MODE` attribute
- **Single-file publish** — releases as a self-contained-free, single `win-x64` executable

---

## 🖥️ Requirements

| Requirement | Version |
|-------------|---------|
| OS          | Windows 10 / 11 (x64) |
| Runtime     | [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) |

---

## 🚀 Getting Started

### Run from release

1. Download the latest executable from the [Releases](https://github.com/TheBlueHeron/Cryptor/releases) page.
2. Ensure the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) is installed.
3. Run `CryptorApp.exe`.

### Build from source

```bash
git clone https://github.com/TheBlueHeron/Cryptor.git
cd Cryptor
dotnet build CryptorApp/CryptorApp.csproj
```

To produce a single-file release build:

```bash
dotnet publish CryptorApp/CryptorApp.csproj -c Release
```

---

## 🔐 Usage

1. Select an algorithm from the dropdown list.
2. For **AES**, **Triple DES**, or **ChaCha20-Poly1305**, enter the key and IV/nonce in the settings panel:
   - *AES*: key must be 16, 24, or 32 bytes; IV must be 16 bytes.
   - *Triple DES*: key must be 16 or 24 bytes (non-weak); IV must be 8 bytes.
   - *ChaCha20-Poly1305*: key must be 16 Unicode characters (32 bytes); nonce (IV) must be 6 Unicode characters (12 bytes).
3. Paste or type the input text.
4. Click **Convert** to see the result.

---

## 🏗️ Project Structure

```
CryptorApp/
├── Common/
│   ├── Crypt.cs              # Encoding helpers (StringToBytes, SecureStringToBytes, …)
│   ├── CryptResult.cs        # Result struct returned by every ICryptor
│   ├── DwmHelper.cs          # P/Invoke helper to apply dark/light mode to the native title bar
│   ├── ICryptor.cs           # Shared interface for all encode/decode operations
│   └── PasswordBoxHelper.cs  # Attached behavior for SecureString ↔ PasswordBox binding
├── Cryptors/
│   ├── AesCryptor.cs         # AES encrypt / decrypt
│   ├── Base64Cryptor.cs      # Base64 encode / decode
│   ├── ChaCha20Cryptor.cs    # ChaCha20-Poly1305 encrypt / decrypt
│   ├── HexCryptor.cs         # Hex encode / decode
│   ├── HtmlCryptor.cs        # HTML encode / decode
│   ├── ShaCryptor.cs         # SHA-256 / SHA-512 hashing (encode only)
│   ├── TripleDesCryptor.cs   # Triple DES encrypt / decrypt
│   └── UrlCryptor.cs         # URL encode / decode
├── Themes/
│   ├── Win11DarkColors.xaml  # Dark palette colour tokens and brushes
│   ├── Win11LightColors.xaml # Light palette colour tokens and brushes
│   └── Win11Theme.xaml       # Implicit control styles and templates (DynamicResource)
├── ViewModels/
│   ├── MainViewModel.cs      # Main window view model
│   └── SettingsViewModel.cs  # Key / IV settings view model
└── Views/
    ├── CryptSettings.xaml    # Key / IV settings control
    └── MainWindow.xaml       # Main application window
```

---

## 🎨 Theming

| File | Role |
|------|------|
| `Win11LightColors.xaml` | Light palette: colour tokens + named brushes |
| `Win11DarkColors.xaml`  | Dark palette: colour tokens + named brushes |
| `Win11Theme.xaml`       | Implicit styles for all controls; all brush references use `DynamicResource` |

`App.xaml.cs` reads the Windows registry key `AppsUseLightTheme` at startup and subscribes to `SystemEvents.UserPreferenceChanged` to swap `MergedDictionaries[0]` at runtime — no restart needed. The native title bar is synchronised via `DwmHelper`, which calls `DwmSetWindowAttribute` with `DWMWA_USE_IMMERSIVE_DARK_MODE`.

---

## 📦 Dependencies

| Package | Version |
|---------|---------|
| [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) | 8.4.2 |
| [Microsoft.Xaml.Behaviors.Wpf](https://www.nuget.org/packages/Microsoft.Xaml.Behaviors.Wpf) | 1.1.142 |

---

## 🤝 Contributing

Contributions are welcome! Please open an [issue](https://github.com/TheBlueHeron/Cryptor/issues) first to discuss what you would like to change, then submit a pull request.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m 'Add my feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for details.
