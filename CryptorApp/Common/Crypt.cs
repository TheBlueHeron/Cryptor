using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CryptorApp;

/// <summary>
/// Helper functions.
/// </summary>
internal sealed class Crypt
{
    #region Objects and variables

    private static readonly UnicodeEncoding encodingUni = new();
    private static readonly UTF8Encoding encodingUtf8 = new();

    #endregion

    #region Methods and functions

    /// <summary>
    /// Converts a byte array to its equivalent string representation using unicode encoding.
    /// </summary>
    /// <remarks>Ensure that the byte array was originally encoded with the same encoding to avoid data corruption or unexpected results.</remarks>
    /// <param name="bytes">The array of bytes to convert to a string.</param>
    /// <param name="useUnicode">If <see langword="true", Unicode encoding is used; else UTF8 encoding is used/></param>
    /// <returns>A string representation of the specified byte array.</returns>
    [DebuggerStepThrough]
	public static string BytesToString(byte[] bytes, bool useUnicode)
    {
        return useUnicode ? encodingUni.GetString(bytes) : encodingUtf8.GetString(bytes);
    }

    /// <summary>
    /// Converts the specified string to a byte array using unicode encoding.
    /// </summary>
    /// <param name="input">The string to convert to a byte array. If null or empty, an empty array is returned</param>
    /// <param name="useUnicode">If <see langword="true", Unicode encoding is used; else UTF8 encoding is used/></param>
    /// <returns>A byte array containing the encoded representation of the input string, or an empty array if the input is null or empty.</returns>
    [DebuggerStepThrough]
    public static byte[] StringToBytes(string input, bool useUnicode)
    {
        if (string.IsNullOrEmpty(input))
        {
            return [];
        }
        return useUnicode ? encodingUni.GetBytes(input) : encodingUtf8.GetBytes(input);
    }

    /// <summary>
    /// Converts the specified <see cref="SecureString"/> to a byte array using unicode encoding.
    /// The unmanaged BSTR and the intermediate character buffer are both zeroed immediately after conversion.
    /// </summary>
    /// <param name="secureString">The <see cref="SecureString"/> to convert. If null or empty, an empty array is returned</param>
    /// <returns>A byte array containing the encoded representation, or an empty array if null or empty.</returns>
    [DebuggerStepThrough]
    public static byte[] SecureStringToBytes(SecureString secureString)
    {
        if (secureString is null || secureString.Length == 0)
        {
            return [];
        }
        var ptr = Marshal.SecureStringToBSTR(secureString);
        var chars = new char[secureString.Length];
        try
        {
            Marshal.Copy(ptr, chars, 0, secureString.Length);
            return encodingUni.GetBytes(chars);
        }
        finally
        {
            Array.Clear(chars);
            Marshal.ZeroFreeBSTR(ptr);
        }
    }

    #endregion
}