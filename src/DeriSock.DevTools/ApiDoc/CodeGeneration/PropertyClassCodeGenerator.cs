namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;
using DeriSock.Model;

internal class PropertyClassCodeGenerator : ApiDocCodeGenerator
{
  public GenType Type { get; set; }

  /// <inheritdoc />
  protected override async Task Generate(CancellationToken cancellationToken)
  {
    if (Document is null)
      throw new ArgumentNullException(nameof(Document));

    switch (Type) {
      case GenType.MethodRequest:
        await GenerateMethodRequests(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.MethodResponse:
        await GenerateMethodResponses(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.SubscriptionChannel:
        await GenerateSubscriptionChannels(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.SubscriptionNotification:
        await GenerateSubscriptionNotifications(cancellationToken).ConfigureAwait(false);
        break;

      default:
        throw new ArgumentOutOfRangeException(nameof(Type));
    }
  }

  private async Task GenerateMethodRequests(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Methods.Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Request == null)
        continue;

      var requestTypeInfo = function.GetRequestTypeInfo();

      if (requestTypeInfo is null)
        continue;

      var className = requestTypeInfo.TypeName;

      var path = DefinePathCallback?.Invoke(className);

      if (string.IsNullOrEmpty(path))
        continue;

      AddProperty(className, function.Request);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task GenerateMethodResponses(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Methods.Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Response == null)
        continue;

      var responseTypeInfo = function.GetResponseTypeInfo();

      if (responseTypeInfo is null or { IsImported: true })
        continue;

      var className = responseTypeInfo.TypeName;

      var path = DefinePathCallback?.Invoke(className);

      if (string.IsNullOrEmpty(path))
        continue;

      AddProperty(className, function.Response);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task GenerateSubscriptionChannels(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Subscriptions.Select(x => x.Value);

    foreach (var function in allFunctions) {
      var requestTypeInfo = function.GetRequestTypeInfo();

      if (requestTypeInfo is null or { IsImported: true })
        continue;

      var className = requestTypeInfo.TypeName;

      var path = DefinePathCallback?.Invoke(className);

      if (string.IsNullOrEmpty(path))
        continue;

      var objClass = new CodeTypeDeclaration
      {
        Name = className,
        Attributes = MemberAttributes.Public,
        IsPartial = true
      };

      objClass.BaseTypes.Add(typeof(ISubscriptionChannel));

      objClass.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

      if (function.Request is not null) {
        objClass.Comments.Add(new CodeCommentStatement("<summary>", true));

        if (!string.IsNullOrEmpty(function.Request.Description))
          objClass.Comments.Add(function.Request.Description.CreateXmlDocumentationPara());

        objClass.Comments.Add(new CodeCommentStatement("</summary>", true));

        if (function.Request is { Properties.Count: > 0 })
          foreach (var (_, value) in function.Request.Properties)
            objClass.Members.Add(CreateProperty(value));
      }

      // Creating the ToChannelName Method (ISubscriptionChannel implementation)
      var toChannelNameMethod = new CodeMemberMethod
      {
        Name = nameof(ISubscriptionChannel.ToChannelName),
        Attributes = MemberAttributes.Public | MemberAttributes.Final,
        ReturnType = new CodeTypeReference(typeof(string))
      };

      var returnValueBuilder = new StringBuilder(function.Name);

      if (function.Request is { Properties.Count: > 0 })
        foreach (var (_, value) in function.Request.Properties)
          returnValueBuilder.Replace($"{{{value.Name}}}", $"{{{value.Name.ToPublicCodeName()}}}");

      toChannelNameMethod.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression($"$\"{returnValueBuilder}\"")));
      objClass.Members.Add(toChannelNameMethod);

      // Adding the new type to the namespace
      AddType(objClass);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task GenerateSubscriptionNotifications(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Subscriptions.Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Response == null)
        continue;

      var responseTypeInfo = function.GetResponseTypeInfo();

      if (responseTypeInfo is null or { IsImported: true })
        continue;

      var className = responseTypeInfo.TypeName;

      var path = DefinePathCallback?.Invoke(className);

      if (string.IsNullOrEmpty(path))
        continue;

      AddProperty(className, function.Response);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private void AddProperty(string typeName, ApiDocProperty property)
  {
    if (property.Properties == null)
      return;

    var objClass = new CodeTypeDeclaration
    {
      Name = typeName,
      Attributes = MemberAttributes.Public,
      IsPartial = true
    };

    objClass.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    objClass.Comments.Add(new CodeCommentStatement("<summary>", true));

    if (!string.IsNullOrEmpty(property.Description))
      objClass.Comments.Add(property.Description.CreateXmlDocumentationPara());

    objClass.Comments.Add(new CodeCommentStatement("</summary>", true));

    foreach (var (_, value) in property.Properties)
      objClass.Members.Add(CreateProperty(value));

    AddType(objClass);
  }

  internal enum GenType
  {
    MethodRequest,
    MethodResponse,
    SubscriptionChannel,
    SubscriptionNotification
  }
}
