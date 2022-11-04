namespace DeriSock.Tests.WebSocketConsole;

using StreamJsonRpc;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class MyJsonRpcMethodAttribute : JsonRpcMethodAttribute
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="MyJsonRpcMethodAttribute" /> class.
  /// </summary>
  public MyJsonRpcMethodAttribute()
  {
    SetDefaults();
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MyJsonRpcMethodAttribute" /> class.
  /// </summary>
  public MyJsonRpcMethodAttribute(string? name) : base(name)
  {
    SetDefaults();
  }

  private void SetDefaults()
  {
    ClientRequiresNamedArguments = true;
    UseSingleObjectParameterDeserialization = true;
  }
}