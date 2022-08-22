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
namespace DeriSock.Api
{
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial interface IComboBooksApi
  {
    /// <summary>
    /// <para>Retrieves information about a combo</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Combo>> PublicGetComboDetails(PublicGetComboDetailsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves available combos. This method can be used to get the list of all combos, or only the list of combos in the given state.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string[]>> PublicGetComboIds(PublicGetComboIdsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves information about active combos</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Combo[]>> PublicGetCombos(PublicGetCombosRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Verifies and creates a combo book or returns an existing combo matching given trades</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Combo>> PrivateCreateCombo(PrivateCreateComboRequest args, CancellationToken cancellationToken = default(CancellationToken));
  }
}