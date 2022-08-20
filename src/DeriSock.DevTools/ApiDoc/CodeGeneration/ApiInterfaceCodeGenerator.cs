// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;

internal class ApiInterfaceCodeGenerator : ApiDocCodeGenerator
{
  private CodeTypeDeclaration? _objSummary;

  public GenType Type { get; set; }

  /// <inheritdoc />
  protected override async Task Generate(CancellationToken cancellationToken)
  {
    switch (Type) {
      case GenType.Categories:
        await GenerateCategories(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.Scopes:
        await GenerateScopes(cancellationToken).ConfigureAwait(false);
        break;

      case GenType.Summary:
        await GenerateSummary(cancellationToken).ConfigureAwait(false);
        break;

      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private async Task GenerateCategories(CancellationToken cancellationToken)
  {
    var functionsPerCategory = Document!.Methods.Where(x => !x.Value.ExcludeInInterface).GroupBy(x => x.Value.Category);

    foreach (var category in functionsPerCategory) {
      if (cancellationToken.IsCancellationRequested)
        break;

      var path = DefinePathCallback?.Invoke(category.Key);

      if (string.IsNullOrEmpty(path))
        continue;

      AddCategoryFunctions($"I{category.Key!.ToPublicCodeName()}Api", category.Select(x => x.Value), false);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task GenerateScopes(CancellationToken cancellationToken)
  {
    var functionsPerScope = Document!.Methods.Where(x => !x.Value.ExcludeInInterface).GroupBy(x => x.Value.IsPrivate);

    foreach (var scope in functionsPerScope) {
      if (cancellationToken.IsCancellationRequested)
        break;

      var path = DefinePathCallback?.Invoke(scope.Key);

      if (string.IsNullOrEmpty(path))
        continue;

      AddCategoryFunctions($"I{(scope.Key ? "Private" : "Public")}Api", scope.Select(x => x.Value), true);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private void AddCategoryFunctions(string typeName, IEnumerable<ApiDocFunction> functions, bool removeScope)
  {
    AddImport(CodeDomConst.ImportSystemThreading);
    AddImport(CodeDomConst.ImportSystemThreadingTasks);
    AddImport(CodeDomConst.ImportDeriSockJsonRpc);
    AddImport(CodeDomConst.ImportDeriSockModel);

    var domType = new CodeTypeDeclaration(typeName)
    {
      Attributes = MemberAttributes.Public,
      IsInterface = true,
      IsPartial = true
    };

    domType.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    foreach (var function in functions) {
      var objMethod = new CodeMemberMethod
      {
        Attributes = MemberAttributes.Public | MemberAttributes.Final,
        Name = function.ToInterfaceMethodName(removeScope)
      };

      objMethod.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

      objMethod.Comments.Add(new CodeCommentStatement("<summary>", true));

      if (!string.IsNullOrEmpty(function.Description))
        foreach (var xmlDocParagraph in function.Description.ToXmlDocParagraphs())
          objMethod.Comments.Add(new CodeCommentStatement($"<para>{xmlDocParagraph}</para>", true));

      objMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
      objMethod.ReturnType = CreateApiMethodTypeReference(function);

      var requestTypeInfo = function.GetRequestTypeInfo();

      if (requestTypeInfo is not null) {
        var argsParamExpr = new CodeParameterDeclarationExpression($"{requestTypeInfo.ToFullTypeName()}", "args");

        if (requestTypeInfo.IsNullable)
          argsParamExpr.UserData.Add("InitExpression", new CodeSnippetExpression("null"));

        objMethod.Parameters.Add(argsParamExpr);
        objMethod.Comments.Add(new CodeCommentStatement("<param name=\"args\"></param>", true));
      }

      if (!function.IsSynchronous) {
        var cancellationTokenParamExpr = new CodeParameterDeclarationExpression(nameof(CancellationToken), "cancellationToken");
        cancellationTokenParamExpr.UserData.Add("InitExpression", new CodeDefaultValueExpression(new CodeTypeReference(nameof(CancellationToken))));
        objMethod.Parameters.Add(cancellationTokenParamExpr);
        objMethod.Comments.Add(new CodeCommentStatement("<param name=\"cancellationToken\"></param>", true));
      }

      domType.Members.Add(objMethod);
    }

    AddType(domType);
  }

  private Task GenerateSummary(CancellationToken cancellationToken)
  {
    var categoryNames = Document!.Methods.Where(x => !x.Value.ExcludeInInterface).GroupBy(x => x.Value.Category).Select(x => x.Key!);

    BeginSummary("ICategoriesApi");

    AddSummaryProperty("Public");
    AddSummaryProperty("Private");

    foreach (var categoryName in categoryNames) {
      if (cancellationToken.IsCancellationRequested)
        break;

      AddSummaryProperty(categoryName);
    }

    EndSummary();

    return Task.CompletedTask;
  }

  private void BeginSummary(string typeName)
  {
    AddImport(CodeDomConst.ImportSystemThreadingTasks);
    AddImport(CodeDomConst.ImportDeriSockJsonRpc);
    AddImport(CodeDomConst.ImportDeriSockModel);

    _objSummary = new CodeTypeDeclaration(typeName)
    {
      Attributes = MemberAttributes.Public,
      IsInterface = true,
      IsPartial = true
    };

    _objSummary.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);
  }

  private void AddSummaryProperty(string name)
  {
    if (_objSummary is null)
      return;

    var objProperty = new CodeMemberProperty
    {
      Attributes = MemberAttributes.Public | MemberAttributes.Final,
      Name = name.ToPublicCodeName()
    };

    var returnTypeName = $"I{objProperty.Name}Api";
    objProperty.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);
    objProperty.Comments.Add(new CodeCommentStatement($"<inheritdoc cref=\"{returnTypeName}\" />", true));
    objProperty.Type = new CodeTypeReference(returnTypeName);

    objProperty.GetStatements.Add(new CodeExpression());

    _objSummary.Members.Add(objProperty);
  }

  private void EndSummary()
  {
    if (_objSummary is null)
      return;

    AddType(_objSummary);
    _objSummary = null;
  }

  internal enum GenType
  {
    Categories,
    Scopes,
    Summary
  }
}
