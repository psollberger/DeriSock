namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Exceptions;
  using Serilog;
  using Serilog.Events;

  public abstract class ClientBase
  {
    protected readonly ILogger Logger = Log.Logger;
    private Thread _processReceiveThread;
    private CancellationTokenSource _receiveCancellationTokenSource;

    private ClientWebSocket _socket;

    protected bool SocketAvailable => _socket != null;

    public bool ClosedByError { get; protected set; }
    public bool ClosedByClient { get; protected set; }
    public bool ClosedByHost { get; protected set; }

    public Uri ServiceEndpoint { get; }

    protected ClientBase(Uri serviceEndpoint)
    {
      ServiceEndpoint = serviceEndpoint;
    }

    /// <summary>
    ///   Connects to the Endpoint using the ServiceEndpoint address and starts processing received messages
    /// </summary>
    public async Task ConnectAsync()
    {
      if (_socket != null)
      {
        throw new WebSocketAlreadyConnectedException();
      }

      ClosedByClient = false;
      ClosedByError = false;
      ClosedByHost = false;

      _receiveCancellationTokenSource = new CancellationTokenSource();

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Connecting to {Host}", ServiceEndpoint);
      }

      _socket = new ClientWebSocket();
      try
      {
        await _socket.ConnectAsync(ServiceEndpoint, CancellationToken.None).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Logger?.Error(ex, "Exception during ConnectAsync");
        _socket.Dispose();
        _socket = null;
        throw;
      }

      //Start processing Threads
      _processReceiveThread = new Thread(ProcessReceive) {Name = "ProcessReceive"};
      _processReceiveThread.Start();
    }

    /// <summary>
    ///   Stops processing of received messages and disconnects from the Endpoint
    /// </summary>
    public async Task DisconnectAsync()
    {
      if (_socket == null || _socket.State != WebSocketState.Open)
      {
        throw new WebSocketNotConnectedException();
      }

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Disconnecting from {Host}", ServiceEndpoint);
      }

      //Shutdown processing Threads
      _receiveCancellationTokenSource.Cancel();
      _processReceiveThread.Join();

      if (_socket.State == WebSocketState.Open)
      {
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Connection", CancellationToken.None);
      }

      _socket.Dispose();
      _socket = null;
    }

    /// <summary>
    ///   Sends a message to the endpoint
    /// </summary>
    /// <param name="message">The message to be sent to the endpoint</param>
    /// <returns>A Task object</returns>
    protected Task SendAsync(string message)
    {
      return _socket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None);
    }

    private void ProcessReceive()
    {
      var msgBuffer = new byte[0x1000];
      var msgSegment = new ArraySegment<byte>(msgBuffer);
      var msgBuilder = new StringBuilder();
      var msgReceiveStart = DateTime.MinValue;

      try
      {
        while (!_receiveCancellationTokenSource.IsCancellationRequested)
        {
          if (_socket == null || _socket.State != WebSocketState.Open)
          {
            continue;
          }

          try
          {
            var receiveResult = _socket.ReceiveAsync(msgSegment, _receiveCancellationTokenSource.Token)
              .GetAwaiter().GetResult();

            if (msgReceiveStart == DateTime.MinValue)
            {
              msgReceiveStart = DateTime.Now;
            }

            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
              ClosedByHost = true;
              if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
              {
                Logger.Debug("ProcessReceive: The host closed the connection");
              }

              break;
            }

            msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer, 0, receiveResult.Count));
            if (!receiveResult.EndOfMessage)
            {
              continue;
            }

            var message = new Message
            {
              ReceiveStart = msgReceiveStart, ReceiveEnd = DateTime.Now, Data = msgBuilder.ToString()
            };

            if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              Logger.Verbose("ProcessReceive: Received Message ({Size}): {@Message}", Encoding.UTF8.GetByteCount(message.Data), message);
            }

            if (message.IsValid)
            {
              Task.Factory.StartNew(mo =>
              {
                OnMessage(mo as Message);
              }, message);
            }

            msgBuilder.Clear();
            msgReceiveStart = DateTime.MinValue;
          }
          catch (OperationCanceledException) when (_receiveCancellationTokenSource.IsCancellationRequested)
          {
            //user cancelled
            ClosedByClient = true;
            Logger?.Verbose("ProcessReceive: Valid manual cancellation");

            break;
          }
          catch (Exception ex)
          {
            ClosedByError = true;
            Logger?.Error(ex, "ProcessReceive: Connection closed by unknown error");
            break;
          }
        }
      }
      finally
      {
        Logger?.Verbose("ProcessReceive: Leaving");
      }
    }

    protected abstract void OnMessage(Message message);
  }
}
