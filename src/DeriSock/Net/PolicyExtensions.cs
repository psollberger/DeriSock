using System;
using System.Net.Http;
using Polly;
using Polly.RateLimit;
using Serilog;

namespace DeriSock.Net;

public static class PolicyExtensions
{
  public static IAsyncPolicy ReconnectPolicy(
    int maxAttempts,
    double delayIncreaseFactor,
    int initialDelayMilliseconds,
    ILogger? logger = null)
  {
    return Policy
      .Handle<Exception>()
      .WaitAndRetryAsync(
        maxAttempts,
        retryAttempt => TimeSpan.FromTicks((long) (TimeSpan.FromMilliseconds(initialDelayMilliseconds).Ticks *
                                                   Math.Pow(delayIncreaseFactor, retryAttempt - 1))),
        (exception, calculatedWaitDuration) =>
        {
          logger?.Error("Error during reconnect attempt: {@Exception}", exception);
        });
  }

  public static AsyncRateLimitPolicy GetRateLimitPolicy(int tier = 4)
  {
    int requestsPerSecond;
    int burst;

    switch (tier)
    {
      case 1:
        requestsPerSecond = 30;
        burst = 100;
        break;
      case 2:
        requestsPerSecond = 20;
        burst = 50;
        break;
      case 3:
        requestsPerSecond = 10;
        burst = 30;
        break;
      case 4:
        requestsPerSecond = 5;
        burst = 20;
        break;
      default:
        requestsPerSecond = 2;
        burst = 10;
        break;
    }

    return Policy
      .RateLimitAsync(requestsPerSecond, TimeSpan.FromSeconds(1), burst);
  }

  public static IAsyncPolicy<HttpResponseMessage> CreateRateLimitPolicy(this IAsyncPolicy<HttpResponseMessage> policy,
    int tier = 4)
  {
    int requestsPerSecond;
    int burst;

    switch (tier)
    {
      case 1:
        requestsPerSecond = 30;
        burst = 100;
        break;
      case 2:
        requestsPerSecond = 20;
        burst = 50;
        break;
      case 3:
        requestsPerSecond = 10;
        burst = 30;
        break;
      case 4:
        requestsPerSecond = 5;
        burst = 20;
        break;
      default:
        requestsPerSecond = 2;
        burst = 10;
        break;
    }

    return Policy
      .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
      .WaitAndRetryAsync(burst, retryAttempt => TimeSpan.FromSeconds(1.0 / requestsPerSecond));
  }
}
