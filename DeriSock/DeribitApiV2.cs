namespace DeriSock
{
  using Converter;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Model;

  public class DeribitApiV2 : JsonRpcWebSocketClient
  {
    public string SessionName { get; }

    private readonly List<Tuple<string, object, object>> _listeners;

    public string AccessToken { get; set; }

    public DeribitApiV2(string hostname, string sessionName) : base($"wss://{hostname}/ws/api/v2")
    {
      SessionName = sessionName;
      _listeners = new List<Tuple<string, object, object>>();
    }

    #region Helpers

    private async Task<bool> SubscribePublicAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, false, null, myCallback)) return false;
      lock (_listeners)
      {
        _listeners.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }
      return true;
    }

    private async Task<bool> UnsubscribePublicAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_listeners)
      {
        entry = _listeners.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }
      if (entry == null) return false;
      if (!await ManagedUnsubscribeAsync(channelName, false, null, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    private async Task<bool> SubscribePrivateAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, true, AccessToken, myCallback)) return false;
      lock (_listeners)
      {
        _listeners.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }
      return true;
    }

    private async Task<bool> UnsubscribePrivateAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_listeners)
      {
        entry = _listeners.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }
      if (entry == null) return false;
      if (!await ManagedUnsubscribeAsync(channelName, true, AccessToken, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    #endregion

    public Task<AuthResponse> PublicAuthAsync(string accessKey, string accessSecret, string? sessionName)
    {
      var scope = "connection";
      if (!string.IsNullOrEmpty(sessionName))
      {
        scope = $"session:{sessionName}";
      }
      return SendAsync("public/auth", new
      {
        grant_type = "client_credentials",
        client_id = accessKey,
        client_secret = accessSecret,
        scope = scope
      }, new ObjectJsonConverter<AuthResponse>());
    }

    public Task<object> PublicSetHeartbeatAsync(int intervalSeconds)
    {
      return SendAsync("public/set_heartbeat", new { interval = intervalSeconds }, new ObjectJsonConverter<object>());
    }

    public Task<object> PublicDisableHeartbeatAsync()
    {
      return SendAsync("public/disable_heartbeat", new { }, new ObjectJsonConverter<object>());
    }

    public Task<object> PrivateDisableCancelOnDisconnectAsync()
    {
      return SendAsync("private/disable_cancel_on_disconnect", new { access_token = AccessToken }, new ObjectJsonConverter<object>());
    }

    public Task<AccountSummaryResponse> PrivateGetAccountSummaryAsync()
    {
      return SendAsync("private/get_account_summary", new { currency = "BTC", extended = true, access_token = AccessToken }, new ObjectJsonConverter<AccountSummaryResponse>());
    }

    public Task<bool> SubscribeBookAsync(string instrument, int group, int depth, Action<BookResponse> callback)
    {
      return SubscribePublicAsync("book." + instrument + "." + (group == 0 ? "none" : group.ToString()) + "." + depth + ".100ms", callback, response =>
      {
        callback(response.@params.data.ToObject<BookResponse>());
      });
    }

    public Task<bool> SubscribeOrdersAsync(string instrument, Action<OrderResponse> callback)
    {
      return SubscribePrivateAsync("user.orders." + instrument + ".raw", callback, response =>
      {
        var orderResponse = response.@params.data.ToObject<OrderResponse>();
        orderResponse.timestamp = response.@params.timestamp;
        callback(orderResponse);
      });
    }

    public Task<bool> SubscribePortfolioAsync(string currency, Action<PortfolioResponse> callback)
    {
      return SubscribePrivateAsync($"user.portfolio.{currency.ToLower()}", callback, response =>
      {
        callback(response.@params.data.ToObject<PortfolioResponse>());
      });
    }

    public Task<bool> SubscribeTickerAsync(string instrument, string interval, Action<TickerResponse> callback)
    {
      return SubscribePublicAsync($"ticker.{instrument}.{interval}", callback, response =>
      {
        callback(response.@params.data.ToObject<TickerResponse>());
      });
    }

    public Task<BookResponse> GetOrderBook(string instrument, int depth)
    {
      return SendAsync("public/get_order_book", new
      {
        instrument_name = instrument,
        depth
      }, new ObjectJsonConverter<BookResponse>());
    }

    public Task<OrderItem[]> GetOpenOrders(string instrument)
    {
      return SendAsync("private/get_open_orders_by_instrument", new
      {
        instrument_name = instrument,
        access_token = AccessToken
      }, new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<BuySellResponse> BuyLimitAsync(string instrument, double amount, double price)
    {
      return SendAsync("private/buy", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = SessionName,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public Task<BuySellResponse> SellLimitAsync(string instrument, double amount, double price)
    {
      return SendAsync("private/sell", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = SessionName,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public async Task<OrderResponse> GetOrderStateAsnyc(string orderId)
    {
      try
      {
        var result = await SendAsync(
                                               "private/get_order_state", new
                                               {
                                                 order_id = orderId,
                                                 access_token = AccessToken
                                               }, new ObjectJsonConverter<OrderResponse>());
        return result;
      }
      catch
      {
        return null;
      }
    }

    public Task<object> CancelOrderAsync(string orderId)
    {
      return SendAsync("private/cancel", new
      {
        order_id = orderId,
        access_token = AccessToken
      }, new ObjectJsonConverter<object>());
    }

    //public Task<object> CancelAllOrdersAsync()
    //{
    //  return SendAsync("private/cancel_all", new
    //  {
    //    access_token = AccessToken
    //  }, new ObjectJsonConverter<object>());
    //}

    public Task<object> CancelAllOrdersByInstrument(string instrument)
    {
      return SendAsync("private/cancel_all_by_instrument", new
      {
        instrument_name = instrument,
        access_token = AccessToken
      }, new ObjectJsonConverter<object>());
    }

    public Task<SettlementResponse> GetSettlementHistoryByInstrument(string instrument, int count)
    {
      return SendAsync(
                              "private/get_settlement_history_by_instrument", new
                              {
                                instrument_name = instrument,
                                type = "settlement",
                                count = count,
                                access_token = AccessToken
                              }, new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<SettlementResponse> GetSettlementHistoryByCurrency(string currency, int count)
    {
      return SendAsync(
                              "private/get_settlement_history_by_currency", new
                              {
                                currency = currency,
                                type = "settlement",
                                count = count,
                                access_token = AccessToken
                              }, new ObjectJsonConverter<SettlementResponse>());
    }
  }
}
