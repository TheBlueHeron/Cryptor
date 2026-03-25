# Cryptor

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
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

| Algorithm | Encode / Encrypt | Decode / Decrypt |
|-----------|:----------------:|:----------------:|
| Base64    | ✅               | ✅               |
| HTML      | ✅               | ✅               |
| URL       | ✅               | ✅               |
| AES       | ✅               | ✅               |
| Triple DES| ✅               | ✅               |

- **MVVM architecture** — built with [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- **Secure key input** — AES and Triple DES keys and IVs are entered via `PasswordBox` and stored as `SecureString`; intermediate byte arrays are zeroed immediately after use
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
2. For **AES** or **Triple DES**, enter the key and IV in the settings panel:
   - *AES*: key must be 16, 24, or 32 bytes; IV must be 16 bytes.
   - *Triple DES*: key must be 16 or 24 bytes (non-weak); IV must be 8 bytes.
3. Paste or type the input text.
4. Click **Convert** to see the result.

---

## 🏗️ Project Structure

```
CryptorApp/
├── Common/
│   ├── Crypt.cs              # Encoding helpers (StringToBytes, SecureStringToBytes, …)
│   ├── CryptResult.cs        # Result struct returned by every ICryptor
│   └── ICryptor.cs           # Shared interface for all encode/decode operations
├── Cryptors/
│   ├── AesCryptor.cs         # AES encrypt / decrypt
│   ├── Base64Cryptor.cs      # Base64 encode / decode
│   ├── HtmlCryptor.cs        # HTML encode / decode
│   ├── TripleDesCryptor.cs   # Triple DES encrypt / decrypt
│   └── UrlCryptor.cs         # URL encode / decode
├── ViewModels/
│   ├── MainViewModel.cs      # Main window view model
│   └── SettingsViewModel.cs  # Key / IV settings view model
└── Views/
    ├── CryptSettings.xaml    # Key / IV settings control
    ├── MainWindow.xaml       # Main application window
    └── PasswordBoxHelper.cs  # Attached behavior for SecureString ↔ PasswordBox binding
```

---

## 📦 Dependencies

| Package | Version |
|---------|---------|
| [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) | 8.4.1 |
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
