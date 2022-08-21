// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;
using DeriSock.Model;

internal class ValueEnumerationCodeGenerator : ApiDocCodeGenerator
{
  /// <inheritdoc />
  protected override async Task Generate(CancellationToken cancellationToken)
  {
    if (EnumMap is null)
      throw new ArgumentNullException(nameof(EnumMap));

    foreach (var (typeName, mapEntry) in EnumMap) {
      var path = DefinePathCallback?.Invoke(typeName);

      if (string.IsNullOrEmpty(path))
        continue;

      AddMapEntry(typeName, mapEntry);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private void AddMapEntry(string typeName, ApiDocEnumMapEntry mapEntry)
  {
    var enumClass = new CodeTypeDeclaration(typeName)
    {
      Attributes = MemberAttributes.Public,
      IsPartial = true
    };

    enumClass.BaseTypes.Add(new CodeTypeReference(typeof(EnumValue)));

    enumClass.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    enumClass.Comments.Add(new CodeCommentStatement("<summary>", true));

    if (!string.IsNullOrEmpty(mapEntry.Description))
      enumClass.Comments.Add(mapEntry.Description.CreateXmlDocumentationPara());

    enumClass.Comments.Add(new CodeCommentStatement("</summary>", true));

    var thisEnumRef = new CodeTypeReference(typeName);

    foreach (var enumValue in mapEntry.EnumValues) {
      var fieldName = string.IsNullOrEmpty(enumValue) ? "None" : enumValue.ToPublicCodeName();

      var enumField = new CodeMemberField(thisEnumRef, fieldName);
      enumField.Attributes = (enumField.Attributes & ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask) | MemberAttributes.Public | MemberAttributes.Static;
      enumField.InitExpression = new CodeObjectCreateExpression(thisEnumRef, new CodePrimitiveExpression(enumValue));
      enumClass.Members.Add(enumField);
    }

    var ctor = new CodeConstructor
    {
      Attributes = MemberAttributes.Private
    };

    ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "value"));
    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("value"));
    enumClass.Members.Add(ctor);

    AddType(enumClass);
  }
}
