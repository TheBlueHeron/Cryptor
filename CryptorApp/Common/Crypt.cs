using System.Diagnostics;
using System.Text;

namespace CryptorApp;

/// <summary>
/// Helper functions.
/// </summary>
internal class Crypt
{
    #region Objects and variables

    private static readonly UnicodeEncoding encoding = new();

    #endregion

    #region Methods and functions

    /// <summary>
    /// Converts a byte array to its equivalent string representation using unicode encoding.
    /// </summary>
    /// <remarks>Ensure that the byte array was originally encoded with the same encoding to avoid data corruption or unexpected results.</remarks>
    /// <param name="bytes">The array of bytes to convert to a string.</param>
    /// <returns>A string representation of the specified byte array.</returns>
    [DebuggerStepThrough]
	public static string BytesToString(byte[] bytes)
    {
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Converts the specified string to a byte array using unicode encoding.
    /// </summary>
    /// <param name="input">The string to convert to a byte array. If null or empty, an empty array is returned</param>
    /// <returns>A byte array containing the encoded representation of the input string, or an empty array if the input is null or empty.</returns>
    [DebuggerStepThrough]
    public static byte[] StringToBytes(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return [];
        }
        return encoding.GetBytes(input);
    }

    #endregion
}