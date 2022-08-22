namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;

internal class PropertyClassCodeGenerator : ApiDocCodeGenerator
{
  public GenType Type { get; set; }

  /// <inheritdoc />
  protected override async Task Generate(CancellationToken cancellationToken)
  {
    if (Document is null)
      throw new ArgumentNullException(nameof(Document));

    switch (Type) {
      case GenType.Request:
        await GenerateRequests(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.Response:
        await GenerateResponses(cancellationToken).ConfigureAwait(false);
        break;

      default:
        throw new ArgumentOutOfRangeException(nameof(Type));
    }
  }

  private async Task GenerateRequests(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Methods.Concat(Document.Subscriptions).Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Request == null)
        continue;

      var requestTypeInfo = function.GetRequestTypeInfo();

      if (requestTypeInfo is null or {IsImported: true})
        continue;

      var className = requestTypeInfo.TypeName;

      var path = DefinePathCallback?.Invoke(className);

      if (string.IsNullOrEmpty(path))
        continue;

      AddProperty(className, function.Request);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task GenerateResponses(CancellationToken cancellationToken)
  {
    var allFunctions = Document!.Methods.Concat(Document.Subscriptions).Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Response == null)
        continue;

      var responseTypeInfo = function.GetResponseTypeInfo();

      if (responseTypeInfo is null or {IsImported: true})
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
      foreach (var xmlDocParagraph in property.Description.ToXmlDocParagraphs())
        objClass.Comments.Add(new CodeCommentStatement($"<para>{xmlDocParagraph}</para>", true));

    objClass.Comments.Add(new CodeCommentStatement("</summary>", true));

    foreach (var (_, value) in property.Properties)
      objClass.Members.Add(CreateProperty(value));

    AddType(objClass);
  }

  internal enum GenType
  {
    Request,
    Response
  }
}
