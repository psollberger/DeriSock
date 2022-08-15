// --------------------------------------------------------------------------
// <auto-generated>
//      This code was generated by a tool.
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------
#pragma warning disable CS1591
#nullable enable
namespace DeriSock.Api {
  using System.Threading.Tasks;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial interface ISessionManagementApi {
    
    /// <summary>
    /// <para>Signals the Websocket connection to send and request heartbeats. Heartbeats can be used to detect stale connections. When heartbeats have been set up, the API server will send <c>heartbeat</c> messages and <c>test_request</c> messages. Your software should respond to <c>test_request</c> messages by sending a <c>/api/v2/public/test</c> request. If your software fails to do so, the API server will immediately close the connection. If your account is configured to cancel on disconnect, any orders opened over the connection will be cancelled.</para>
    /// </summary>
    /// <param name="args"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PublicSetHeartbeat(PublicSetHeartbeatRequest args);
    
    /// <summary>
    /// <para>Stop sending heartbeat messages.</para>
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PublicDisableHeartbeat();
    
    /// <summary>
    /// <para>Enable Cancel On Disconnect for the connection. After enabling Cancel On Disconnect all orders created by the connection will be removed when connection is closed.</para>
    /// <para><b>NOTICE</b> It does not affect orders created by other connections - they will remain active !</para>
    /// <para>When change is applied for the account, then every newly opened connection will start with <b>active</b> Cancel on Disconnect.</para>
    /// </summary>
    /// <param name="args"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateEnableCancelOnDisconnect(PrivateEnableCancelOnDisconnectRequest? args);
    
    /// <summary>
    /// <para>Disable Cancel On Disconnect for the connection.</para>
    /// <para>When change is applied for the account, then every newly opened connection will start with <b>inactive</b> Cancel on Disconnect.</para>
    /// </summary>
    /// <param name="args"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateDisableCancelOnDisconnect(PrivateDisableCancelOnDisconnectRequest? args);
    
    /// <summary>
    /// <para>Read current Cancel On Disconnect configuration for the account.</para>
    /// </summary>
    /// <param name="args"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<PrivateGetCancelOnDisconnectResponse>> PrivateGetCancelOnDisconnect(PrivateGetCancelOnDisconnectRequest? args);
  }
}
