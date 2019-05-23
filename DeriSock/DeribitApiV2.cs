namespace DeriSock
{
  using Converter;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Data;
  using Model;

  public class DeribitApiV2
  {
    private readonly string _accessKey;
    private readonly string _accessSecret;

    public WebSocket Socket { get; }

    public string SessionName { get; }

    private readonly List<Tuple<string, object, object>> _listeners;

    public DeribitApiV2(string hostname, string accessKey, string accessSecret, string sessionName)
    {
      _accessKey = accessKey;
      _accessSecret = accessSecret;
      SessionName = sessionName;
      Socket = new WebSocket(hostname);
      _listeners = new List<Tuple<string, object, object>>();
    }

    #region Helpers

    private async Task<bool> SubscribePublicAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await Socket.ManagedSubscribeAsync(channelName, false, myCallback)) return false;
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
      if (!await Socket.ManagedUnsubscribeAsync(channelName, false, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    private async Task<bool> SubscribePrivateAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await Socket.ManagedSubscribeAsync(channelName, true, myCallback)) return false;
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
      if (!await Socket.ManagedUnsubscribeAsync(channelName, true, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    #endregion

    public Task<AuthResponse> LoginAsync()
    {
      return Socket.SendAsync("public/auth", new
      {
        grant_type = "client_credentials",
        client_id = _accessKey,
        client_secret = _accessSecret,
        scope = $"session:{SessionName}"
      }, new ObjectJsonConverter<AuthResponse>());
    }

    public Task<object> SetHeartbeatAsync(int intervalSeconds)
    {
      return Socket.SendAsync("public/set_heartbeat", new { interval = intervalSeconds }, new ObjectJsonConverter<object>());
    }

    public Task<object> DisableHeartbeatAsync()
    {
      return Socket.SendAsync("public/disable_heartbeat", new { }, new ObjectJsonConverter<object>());
    }

    public Task<object> DisableCancelOnDisconnectAsync()
    {
      return Socket.SendAsync("private/disable_cancel_on_disconnect", new { access_token = Socket.AccessToken }, new ObjectJsonConverter<object>());
    }

    public Task<AccountSummaryResponse> GetAccountSummaryAsync()
    {
      return Socket.SendAsync("private/get_account_summary", new { currency = "BTC", extended = true, access_token = Socket.AccessToken }, new ObjectJsonConverter<AccountSummaryResponse>());
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
      return Socket.SendAsync("public/get_order_book", new
      {
        instrument_name = instrument,
        depth
      }, new ObjectJsonConverter<BookResponse>());
    }

    public Task<OrderItem[]> GetOpenOrders(string instrument)
    {
      return Socket.SendAsync("private/get_open_orders_by_instrument", new
      {
        instrument_name = instrument,
        access_token = Socket.AccessToken
      }, new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<BuySellResponse> BuyLimitAsync(string instrument, double amount, double price)
    {
      return Socket.SendAsync("private/buy", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = SessionName,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = Socket.AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public Task<BuySellResponse> SellLimitAsync(string instrument, double amount, double price)
    {
      return Socket.SendAsync("private/sell", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = SessionName,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = Socket.AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public async Task<OrderResponse> GetOrderStateAsnyc(string orderId)
    {
      try
      {
        var result = await Socket.SendAsync(
                                               "private/get_order_state", new
                                               {
                                                 order_id = orderId,
                                                 access_token = Socket.AccessToken
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
      return Socket.SendAsync("private/cancel", new
      {
        order_id = orderId,
        access_token = Socket.AccessToken
      }, new ObjectJsonConverter<object>());
    }

    //public Task<object> CancelAllOrdersAsync()
    //{
    //  return Socket.SendAsync("private/cancel_all", new
    //  {
    //    access_token = Socket.AccessToken
    //  }, new ObjectJsonConverter<object>());
    //}

    public Task<object> CancelAllOrdersByInstrument(string instrument)
    {
      return Socket.SendAsync("private/cancel_all_by_instrument", new
      {
        instrument_name = instrument,
        access_token = Socket.AccessToken
      }, new ObjectJsonConverter<object>());
    }

    public Task<SettlementResponse> GetSettlementHistoryByInstrument(string instrument, int count)
    {
      return Socket.SendAsync(
                              "private/get_settlement_history_by_instrument", new
                              {
                                instrument_name = instrument,
                                type = "settlement",
                                count = count,
                                access_token = Socket.AccessToken
                              }, new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<SettlementResponse> GetSettlementHistoryByCurrency(string currency, int count)
    {
      return Socket.SendAsync(
                              "private/get_settlement_history_by_currency", new
                              {
                                currency = currency,
                                type = "settlement",
                                count = count,
                                access_token = Socket.AccessToken
                              }, new ObjectJsonConverter<SettlementResponse>());
    }
  }
}
