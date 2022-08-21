namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

using DeriSock.DevTools.CodeDom;

public enum ApiDocFunctionType
{
  Undefined,
  Method,
  Subscription
}

/// <summary>
///   For the lack of a better name, this is called <see cref="ApiDocFunction" />. It represents a method or subscription definition
/// </summary>
public class ApiDocFunction : IApiDocPropertyNode
{
  [JsonIgnore]
  IApiDocPropertyNode? IApiDocPropertyNode.Parent => null;

  [JsonIgnore]
  public ApiDocFunctionType FunctionType { get; set; } = ApiDocFunctionType.Undefined;

  /// <summary>
  ///   Gets the category of a method. This is always <c>null</c> on a subscription
  /// </summary>
  [JsonPropertyName("category")]
  public string? Category { get; set; } = null;

  /// <summary>
  /// Gets a friendly channel name that can be used for <see cref="DeriSock.Request.ISubscriptionChannel"/>.
  /// </summary>
  [JsonPropertyName("friendlyChannelName")]
  public string? FriendlyChannelName { get; set; } = null;

  /// <summary>
  ///   Gets, if the method/channel is private and authentication is needed to use it. This is always false on a subscription
  /// </summary>
  [JsonIgnore]
  public bool IsPrivate =>
    (FunctionType == ApiDocFunctionType.Method && Name.StartsWith("private/")) ||
    (FunctionType == ApiDocFunctionType.Subscription && Name.StartsWith("user."));

  /// <summary>
  ///   <para>This is the methods or subscriptions name in the form of:</para>
  ///   <para>
  ///     Method: <c>public/name_of_method</c><br />
  ///     Subscription: <c>subscription_name.{param1}.{param2}</c>
  ///   </para>
  /// </summary>
  [JsonIgnore]
  public string Name { get; set; } = string.Empty;

  /// <summary>
  ///   This is the description of the method or subscription
  /// </summary>
  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;

  /// <summary>
  ///   Gets, if this entry should not be included in interface generation
  /// </summary>
  [JsonPropertyName("excludeInInterface")]
  public bool ExcludeInInterface { get; set; }

  /// <summary>
  ///   Gets, if this function is executed synchronously
  /// </summary>
  [JsonPropertyName("isSynchronous")]
  public bool IsSynchronous { get; set; }

  /// <summary>
  ///   Gets, if this entry
  /// </summary>
  [JsonPropertyName("deprecated")]
  public bool Deprecated { get; set; }

  /// <summary>
  ///   This contains the parameters for a method request or for subscribing to a subscription
  /// </summary>
  [JsonPropertyName("request")]
  public ApiDocProperty? Request { get; set; } = default;

  /// <summary>
  ///   This contains the content of the method responses result property or the subscription notifications data property
  /// </summary>
  [JsonPropertyName("response")]
  public ApiDocProperty? Response { get; set; } = default;

  [JsonIgnore]
  ApiDocPropertyCollection? IApiDocPropertyNode.Properties
  {
    get => ApiDocPropertyCollection.Empty;
    set { }
  }

  public DataTypeInfo? GetRequestTypeInfo()
  {
    if (FunctionType == ApiDocFunctionType.Subscription)
      return new DataTypeInfo($"{FriendlyChannelName ?? Name.ToPublicCodeName()}Channel", false, false);

    if (Request is null)
      return null;

    var dataTypeInfo = Request.GetDataTypeInfo();

    if (dataTypeInfo is null or { TypeName: null } or { TypeName: "" } or { TypeName: "object" })
      return new DataTypeInfo($"{Name.ToPublicCodeName()}Request", false, !Request.IsAnyPropertyRequired);

    dataTypeInfo.IsImported = true;
    dataTypeInfo.IsNullable = !Request.IsAnyPropertyRequired;
    return dataTypeInfo;
  }

  public DataTypeInfo? GetResponseTypeInfo()
  {
    if (Response is null)
      return null;

    var dataTypeInfo = Response.GetDataTypeInfo();

    if (dataTypeInfo is null or { TypeName: null } or { TypeName: "" } or { TypeName: "object" })
      return new DataTypeInfo($"{Name.ToPublicCodeName()}Response", false, !Response.IsAnyPropertyRequired);

    dataTypeInfo.IsImported = true;
    dataTypeInfo.IsNullable = !Response.IsAnyPropertyRequired;
    return dataTypeInfo;
  }

  public string ToInterfaceMethodName(bool removeScope)
  {
    if (FunctionType == ApiDocFunctionType.Subscription)
      return $"Subscribe{FriendlyChannelName ?? Name.ToPublicCodeName()}";

    if (removeScope)
      return Name[(Name.IndexOf('/') + 1)..].ToPublicCodeName();

    return Name.ToPublicCodeName();
  }

  public void Accept(IApiDocDocumentVisitor visitor)
  {
    visitor.VisitFunction(this);
    Request?.Accept(visitor);
    Response?.Accept(visitor);
  }

  public void UpdateRelations(IApiDocPropertyNode? parent)
  {
    if (Request != null) {
      Request.Name = "request";
      Request.UpdateRelations(this);
    }

    if (Response != null) {
      Response.Name = "response";
      Response.UpdateRelations(this);
    }
  }
}
