# DeriSock

This is a Deribit API C# .Net Core Client Library

## What is this for?

This client library let's you connect with the Deribit API using a WebSocket connection.  
It is entirely made with asynchronous methods in mind.  
All methods and subscriptions found on https://docs.deribit.com are supported.

## How do I use it?

Usage is quite simple. To obtain the current best bid price you simply do the following:

snippet: readme-how-to-use

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

snippet: readme-subscribtion-usage
