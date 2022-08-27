namespace DeriSock.Constants;

using System;

/// <summary>
///   Holds the URIs to the available Deribit endpoints.
/// </summary>
public static class Endpoint
{
  /// <summary>
  ///   The URI to the productive endpoint.
  /// </summary>
  public static readonly Uri Productive = new("wss://www.deribit.com/ws/api/v2");

  /// <summary>
  ///   The URI to the test endpoint.
  /// </summary>
  public static readonly Uri TestNet = new("wss://test.deribit.com/ws/api/v2");
}
