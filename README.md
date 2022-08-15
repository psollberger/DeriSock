# DeriSock

This is a Deribit API v2.0.0 C# .Net Core Client Library

## What is this for?

This client library let's you connect with the Deribit API using a WebSocket connection.  
It is entirely made with asynchronous methods in mind.  
All methods and subscriptions found on https://docs.deribit.com are supported.

## How do I use it?

Usage is quite simple. To obtain the current best bid price you simply do the following:

```csharp
var client = new DeribitClient(EndpointType.Testnet);
await client.Connect();

var response = await client.PublicGetOrderBook("BTC-PERPETUAL");
var bestBidPrice = response.ResultData.BestBidPrice;

await client.Disconnect();
```

## Authentication

The library supports authentication via credentials and signature

### Authentication using Credentials

```csharp
var authInfo = await client.PublicLogin()
                 .WithClientCredentials("<client id", "<client secret>",
                                        "<optional state>", "<optional scope>");
```

### Authentication using Signature

```csharp
var authInfo = await client.PublicLogin()
                 .WithClientSignature("<client id", "<client secret>", "<optional data>",
                                      "<optional state>", "<optional scope>");
```

### Logout

When authenticated, you can logout like this (this is the only synchroneous method):

```csharp
client.PrivateLogout();
```

**Note:** The server will automatically close the connection when you logout

## Subscriptions

```csharp
var success = await client.SubscribeBookChange(new BookChangeSubscriptionParams
{
    InstrumentName = "BTC-PERPETUAL",
    Interval = "100ms"
}, notification =>
{
    //Here you can do something with the received information.
    //This callback method is executed everytime a notification for
    //this subscription is received.
});
```