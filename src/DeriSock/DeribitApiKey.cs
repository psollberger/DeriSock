namespace DeriSock;

/// <summary>
///   Represents a Deribit API Key
/// </summary>
public sealed class DeribitApiKey
{
  /// <summary>
  /// The Client ID of the Api Key
  /// </summary>
  public string ClientId { get; set; } = string.Empty;

  /// <summary>
  /// The Client Secret of the Api Key
  /// </summary>
  public string ClientSecret { get; set; } = string.Empty;
}
