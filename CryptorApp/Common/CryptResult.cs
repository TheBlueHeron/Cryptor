namespace CryptorApp;

/// <summary>
/// The result of a crypt operation.
/// </summary>
public struct CryptResult
{
    /// <summary>
    /// Any error message. May be null.
    /// </summary>
    public string? Error;
    /// <summary>
    /// The result of the operation. May be null.
    /// </summary>
    public string? Output;
}