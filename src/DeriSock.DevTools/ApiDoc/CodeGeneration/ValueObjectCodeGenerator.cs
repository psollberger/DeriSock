namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;

internal class ValueObjectCodeGenerator : ApiDocCodeGenerator
{
  /// <inheritdoc />
  protected override async Task Generate(CancellationToken cancellationToken)
  {
    if (ObjectMap is null)
      throw new ArgumentNullException(nameof(ObjectMap));

    foreach (var (typeName, mapEntry) in ObjectMap) {
      var path = DefinePathCallback?.Invoke(typeName);

      if (string.IsNullOrEmpty(path))
        continue;

      AddMapEntry(typeName, mapEntry);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private void AddMapEntry(string typeName, ApiDocObjectMapEntry mapEntry)
  {
    if (mapEntry.Properties is null)
      return;

    var propertySourcePath = mapEntry.GetFirstSourcePath();

    if (propertySourcePath is null)
      return;

    var apiDocPropertySource = Document?.GetPropertyFromPath(propertySourcePath);

    if (apiDocPropertySource is null or {Properties: null})
      return;

    var objClass = new CodeTypeDeclaration
    {
      Name = typeName,
      Attributes = MemberAttributes.Public,
      IsPartial = true
    };

    objClass.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    objClass.Comments.Add(new CodeCommentStatement("<summary>", true));

    if (!string.IsNullOrEmpty(mapEntry.Description))
      foreach (var xmlDocParagraph in mapEntry.Description.ToXmlDocParagraphs())
        objClass.Comments.Add(new CodeCommentStatement($"<para>{xmlDocParagraph}</para>", true));

    objClass.Comments.Add(new CodeCommentStatement("</summary>", true));

    foreach (var (_, value) in apiDocPropertySource.Properties) {
      objClass.Members.Add(CreateProperty(value));
    }

    AddType(objClass);
  }
}
