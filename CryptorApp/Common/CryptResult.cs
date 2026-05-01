namespace CryptorApp;

/// <summary>
/// The immutable result of a crypt operation.
/// </summary>
public readonly record struct CryptResult
{
    /// <summary>
    /// The result of the operation. May be null.
    /// </summary>
    public string? Output { get; init; }

    /// <summary>
    /// Any error message. May be null.
    /// </summary>
    public string? Error { get; init; }
}