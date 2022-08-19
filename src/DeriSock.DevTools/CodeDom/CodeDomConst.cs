namespace DeriSock.DevTools.CodeDom;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;

using Newtonsoft.Json;

internal static class CodeDomConst
{
  public static readonly string[] GeneratedCodeComment =
  {
    "--------------------------------------------------------------------------",
    "<auto-generated>",
    "     This code was generated by a tool.",
    "",
    "     Changes to this file may cause incorrect behavior and will be lost if",
    "     the code is regenerated.",
    "</auto-generated>",
    "--------------------------------------------------------------------------"
  };

  public const string ImportSystem = "System";
  public const string ImportSystemThreading = "System.Threading";
  public const string ImportSystemThreadingTasks = "System.Threading.Tasks";
  public const string ImportDeriSockApi = "DeriSock.Api";
  public const string ImportDeriSockConverter = "DeriSock.Converter";
  public const string ImportDeriSockNetJsonRpc = "DeriSock.Net.JsonRpc";
  public const string ImportDeriSockModel = "DeriSock.Model";
  public const string ImportNewtonsoftJsonLinq = "Newtonsoft.Json.Linq";

  public static readonly string AssemblyName = typeof(CodeDomConst).Assembly.GetName().Name!;
  public static readonly string AssemblyVersion = typeof(CodeDomConst).Assembly.GetName().Version!.ToString(3);

  public static readonly CodeAttributeDeclaration GeneratedCodeAttribute =
    new(
      new CodeTypeReference(typeof(GeneratedCodeAttribute)),
      new CodeAttributeArgument(new CodePrimitiveExpression(AssemblyName)),
      new CodeAttributeArgument(new CodePrimitiveExpression(AssemblyVersion)));

  public static readonly CodeAttributeDeclaration DebuggerNonUserCodeAttribute =
    new(new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute)));

  public static readonly CodeAttributeDeclaration DebuggerBrowsableAttribute =
    new(
      new CodeTypeReference(typeof(DebuggerBrowsableAttribute)),
      new CodeAttributeArgument(
        new CodeFieldReferenceExpression(
          new CodeTypeReferenceExpression(new CodeTypeReference(typeof(DebuggerBrowsableState))),
          nameof(DebuggerBrowsableState.Never))));

  public static readonly CodeAttributeDeclaration ObsoleteAttribute = new(new CodeTypeReference(typeof(ObsoleteAttribute)));

  public static readonly CodeTypeReference JsonPropertyAttributeType = new(typeof(JsonPropertyAttribute));
  public static readonly CodeTypeReference JsonConverterAttributeType = new(typeof(JsonConverter));

  public static readonly CodeTypeReferenceExpression ArrayCodeTypeRefExpr = new(typeof(Array));

  public static readonly CodeFieldReferenceExpression StringEmptyRefExpr = new(new CodeTypeReferenceExpression(typeof(string)), nameof(string.Empty));

  public static readonly CodeSnippetExpression NotNullValueRefExpr = new("null!");
}
