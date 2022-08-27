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
namespace DeriSock.Model
{
  using System;
  using DeriSock.Converter;
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class Combo
  {
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("creation_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreationTimestamp { get; set; }
    /// <summary>
    /// <para>Unique combo identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("id")]
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// <para>Instrument ID</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_id")]
    public long InstrumentId { get; set; }
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("legs")]
    public ComboLeg[] Legs { get; set; } = System.Array.Empty<ComboLeg>();
    /// <summary>
    /// <para>Combo state: <c>&quot;rfq&quot;</c>, <c>&quot;active&quot;</c>, &quot;<c>inactive</c>&quot;</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("state")]
    public string State { get; set; } = string.Empty;
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("state_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime StateTimestamp { get; set; }
  }
}
