# DeriSock
This is a Deribit API v2.0.0 C# .Net Core Client Library

## What is this for?
This client library let's you connect with the Deribit API using a WebSocket connection.

It is entirely made with asynchronous methods in mind.

It supports all methods and subscriptions found on https://docs.deribit.com

## How do I use it?
Usage is quite simple. To obtain the current best bid price you simply do the following:
```csharp
var client = new DeribitV2Client(DeribitEndpointType.Testnet);
await client.Connect();

var response = await client.PublicGetOrderBook("BTC-PERPETUAL");
var bestBidPrice = response.ResultData.BestBidPrice;

await client.Disconnect();
```

### Authentication
The library supports authentication via credentials and signature

#### Authentication using Credentials
```csharp
var authInfo = await client.PublicAuth(new AuthParams
{
    GrantType = GrantType.Credentials,
    ClientId = "<client id>",
    ClientSecret = "<client secret>"
});
```

#### Authentication using Signature
```csharp
var signatureData = CryptoHelper.CreateSignature("<client secret>", "<optional data>");
var authInfo = await client.PublicAuth(new AuthParams
{
    GrantType = GrantType.Signature,
    ClientId = "<client id>",
    Signature = signatureData
});
```

### Subscriptions
The library supports all subscriptions.

Here is an example of a subscription:
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