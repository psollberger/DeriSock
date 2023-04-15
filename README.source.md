# DeriSock ![Build Status](https://github.com/psollberger/DeriSock/actions/workflows/DeriSock_BuildTest.yml/badge.svg)  [![NuGet Version](http://img.shields.io/nuget/v/DeriSock.svg?style=flat)](https://www.nuget.org/packages/DeriSock/) [![NuGet Downloads](https://img.shields.io/nuget/dt/DeriSock.svg)](https://www.nuget.org/packages/DeriSock/)

DeriSock is a client library that connects to the Deribit API via WebSocket.  
All methods and subscriptions found on https://docs.deribit.com are supported.

## Getting Started

To connect to the Deribit Network just instantiate a new instance of the `DeribitClient` class and call the `Connect` method to connect and the `Disconnect` method to disconnect.

snippet: readme-connect-disconnect

To use a proxy, you can assign an `IWebProxy` instance to the default `ITextMessageClient` implementation (`TextMessageMessageWebSocketClient`) and pass it to the `DeribitClient` constructor.

snippet: readme-webproxy

The various methods are organized in categories (Authentication, Supporting, Market Data, ...) and scopes (Private, Public).

**Example:** Calling `GetOrderBook` from the `Public` scope.

snippet: readme-req-bbp-1

**Example:** Calling `GetOrderBook` from the `MarketData` category.

snippet: readme-req-bbp-2

## Authentication

The library supports authentication via credentials and signature

### Authentication using Credentials

snippet: readme-auth-credentials

### Authentication using Signature

snippet: readme-auth-signature

### Logout

When authenticated, you can logout like this (this is the only synchroneous method):

snippet: readme-auth-logout

**Note:** The server will automatically close the connection when you logout

## Subscriptions

The subscription system will choose `public/subscribe` or `private/subscribe` automatically.
If the client is authenticated it will use `private/subscribe`, if the client is not authenticated it will use `public/subscribe`.
This is also the reason why the subscription methods are not present in the `Public` or `Private` scopes.

snippet: readme-subscribtion-usage
