namespace CryptorApp.Cryptors;

/// <summary>
/// Interface definition for objects that encode or decode text.
/// </summary>
public interface ICryptor : ICryptorBase
{
    /// <summary>
    /// Converts the input string based on the <see cref="ICryptorBase.Mode"/>.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>A <see cref="CryptResult"/>, containing the result output and possibly an error message.</returns>
    Task<CryptResult> ConvertAsync(string input);
}