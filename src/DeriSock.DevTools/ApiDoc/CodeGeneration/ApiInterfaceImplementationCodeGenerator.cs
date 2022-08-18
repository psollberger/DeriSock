// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;

internal class ApiInterfaceImplementationCodeGenerator : ApiDocCodeGenerator
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

      CreateImplementation($"{category.Key!.ToPublicCodeName()}ApiImpl", $"I{category.Key!.ToPublicCodeName()}Api", category.Select(x => x.Value), false);

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

      CreateImplementation($"{(scope.Key ? "Private" : "Public")}ApiImpl", $"I{(scope.Key ? "Private" : "Public")}Api", scope.Select(x => x.Value), true);

      await WriteToAsync(path, cancellationToken).ConfigureAwait(false);
    }
  }

  private void CreateImplementation(string typeName, string interfaceName, IEnumerable<ApiDocFunction> functions, bool removeScope)
  {
    AddImport(CodeDomConst.ImportSystem);
    AddImport(CodeDomConst.ImportSystemThreading);
    AddImport(CodeDomConst.ImportSystemThreadingTasks);
    AddImport(CodeDomConst.ImportDeriSockApi);
    AddImport(CodeDomConst.ImportDeriSockJsonRpc);
    AddImport(CodeDomConst.ImportDeriSockModel);
    AddImport(CodeDomConst.ImportNewtonsoftJsonLinq);

    // Adding the implementation class
    var domType = new CodeTypeDeclaration(typeName)
    {
      TypeAttributes = TypeAttributes.NestedPrivate | TypeAttributes.Sealed,
      IsPartial = true
    };

    domType.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    // Implementing from the interface
    domType.BaseTypes.Add(interfaceName);

    // Adding Field _client
    var clientField = new CodeMemberField("DeribitClient", "_client");
    clientField.Attributes = MemberAttributes.Private | MemberAttributes.Final;
    domType.Members.Add(clientField);

    // Adding Constructor
    var ctor = new CodeConstructor
    {
      Attributes = MemberAttributes.Public | MemberAttributes.Final
    };

    ctor.Parameters.Add(new CodeParameterDeclarationExpression("DeribitClient", "client"));
    ctor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(null, "_client"), new CodeArgumentReferenceExpression("client")));
    domType.Members.Add(ctor);

    // Adding implementation of methods

    foreach (var function in functions) {
      var objMethod = new CodeMemberMethod
      {
        Attributes = MemberAttributes.Public | MemberAttributes.Final,
        Name = function.ToInterfaceMethodName(removeScope)
      };

      objMethod.PrivateImplementationType = new CodeTypeReference(interfaceName);
      objMethod.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

      // Adding method comment
      objMethod.Comments.Add(new CodeCommentStatement($"<inheritdoc cref=\"{interfaceName}.{objMethod.Name}\" />", true));

      // Defining return type
      objMethod.ReturnType = CreateApiMethodTypeReference(function);

      // Adding Parameters (and collecting call arguments for method body)
      var callArgs = new List<CodeExpression>(2);

      var requestTypeInfo = function.GetRequestTypeInfo();

      if (requestTypeInfo is not null) {
        var argsParamExpr = new CodeParameterDeclarationExpression($"{requestTypeInfo.ToFullTypeName()}", "args");

        // In the implementation context, a default value isn't allowed
        //if (requestTypeInfo.IsNullable)
        //  argsParamExpr.UserData.Add("InitExpression", new CodeSnippetExpression("null"));

        objMethod.Parameters.Add(argsParamExpr);
        callArgs.Add(new CodeArgumentReferenceExpression("args"));
      }

      if (!function.IsSynchronous) {
        var cancellationTokenParamExpr = new CodeParameterDeclarationExpression(nameof(CancellationToken), "cancellationToken");

        // In the implementation context, a default value isn't allowed
        //cancellationTokenParamExpr.UserData.Add("InitExpression", new CodeDefaultValueExpression(new CodeTypeReference(nameof(CancellationToken))));
        objMethod.Parameters.Add(cancellationTokenParamExpr);
        callArgs.Add(new CodeArgumentReferenceExpression("cancellationToken"));
      }

      // Adding method body
      var methodInvoke = new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(null, "_client"), $"Internal{function.ToInterfaceMethodName(false)}", callArgs.ToArray());

      CodeStatement bodyStatement = objMethod.ReturnType!.BaseType switch
      {
        "System.Void" => new CodeExpressionStatement(methodInvoke),
        _             => new CodeMethodReturnStatement(methodInvoke)
      };

      objMethod.Statements.Add(bodyStatement);

      domType.Members.Add(objMethod);
    }

    // Adding the class into DeribitClient

    var clientClass = new CodeTypeDeclaration("DeribitClient")
    {
      Attributes = MemberAttributes.Public,
      IsPartial = true
    };

    clientClass.Members.Add(domType);

    AddType(clientClass);
  }

  private Task GenerateSummary(CancellationToken cancellationToken)
  {
    var categoryNames = Document!.Methods.Where(x => !x.Value.ExcludeInInterface).GroupBy(x => x.Value.Category).Select(x => x.Key!).ToArray();

    BeginSummary("ICategoriesApi");

    AddSummaryField("Public");
    AddSummaryField("Private");

    foreach (var categoryName in categoryNames) {
      if (cancellationToken.IsCancellationRequested)
        break;

      AddSummaryField(categoryName);
    }

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

  private void BeginSummary(string interfaceName)
  {
    AddImport(CodeDomConst.ImportDeriSockApi);

    _objSummary = new CodeTypeDeclaration("DeribitClient")
    {
      TypeAttributes = TypeAttributes.Public,
      IsPartial = true
    };

    _objSummary.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);

    // Implementing from the interface
    _objSummary.BaseTypes.Add(interfaceName);
  }

  private void AddSummaryField(string name)
  {
    if (_objSummary is null)
      return;

    var clientField = new CodeMemberField($"I{name.ToPublicCodeName()}Api?", $"_{name.ToPrivateCodeName()}");
    clientField.Attributes = MemberAttributes.Private;
    _objSummary.Members.Add(clientField);
  }

  private void AddSummaryProperty(string name)
  {
    if (_objSummary is null)
      return;

    var propName = name.ToPublicCodeName();
    var fieldName = $"_{name.ToPrivateCodeName()}";
    var implTypeName = $"{propName}ApiImpl";

    var objProperty = new CodeMemberProperty
    {
      Attributes = MemberAttributes.Public | MemberAttributes.Final,
      Name = propName
    };

    var returnTypeName = $"I{propName}Api";
    objProperty.CustomAttributes.Add(CodeDomConst.GeneratedCodeAttribute);
    objProperty.Comments.Add(new CodeCommentStatement($"<inheritdoc cref=\"{returnTypeName}\" />", true));
    objProperty.Type = new CodeTypeReference(returnTypeName);

    objProperty.GetStatements.Add(new CodeSnippetExpression($"{fieldName} ??= new {implTypeName}(this)"));
    objProperty.GetStatements.Add(new CodeSnippetExpression($"return {fieldName}"));

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
