namespace DeriSock.DevTools.ApiDoc.CodeGeneration;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;
using DeriSock.JsonRpc;

internal abstract class ApiDocCodeGenerator : IDisposable, IAsyncDisposable
{
  private readonly ICodeGenerator _generator = new CSharpCodeGenerator();

  private readonly CodeGeneratorOptions _codeGeneratorOptions = new()
  {
    IndentString = "  ",
    BlankLinesBetweenMembers = false,
    BracingStyle = "C",
    VerbatimOrder = true
  };

  private CodeNamespace? _namespace;

  private readonly StringWriter _writer;
  public string FileExtension => ".g.cs";

  public ApiDocDocument? Document { get; set; }
  public ApiDocEnumMap? EnumMap { get; set; }
  public ApiDocObjectMap? ObjectMap { get; set; }

  public string Namespace { get; set; } = string.Empty;

  public Func<object?, string>? DefinePathCallback { get; set; }

  protected ApiDocCodeGenerator()
  {
    _writer = new StringWriter();
  }

  public async Task GenerateToAsync(string path, CancellationToken cancellationToken = default)
  {
    _writer.GetStringBuilder().Clear();
    _namespace = new CodeNamespace(Namespace);
    GenerateHeader();

    await Generate(cancellationToken).ConfigureAwait(false);

    CheckImports(_namespace);
    GenerateCodeFromNamespace(_namespace);

    await _writer.FlushAsync();

    var directoryPath = Path.GetDirectoryName(path);
    Debug.Assert(directoryPath != null);

    if (!Directory.Exists(directoryPath))
      Directory.CreateDirectory(directoryPath);

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(_writer.ToString()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    _writer.GetStringBuilder().Clear();
    _namespace = null;
  }

  public async Task GenerateToCustomAsync(CancellationToken cancellationToken = default)
  {
    _namespace = new CodeNamespace(Namespace);
    GenerateHeader();
    await Generate(cancellationToken).ConfigureAwait(false);
    _namespace = null;
  }

  protected abstract Task Generate(CancellationToken cancellationToken);

  public async Task WriteToAsync(string path, CancellationToken cancellationToken = default)
  {
    if (_namespace is null)
      throw new InvalidOperationException("Can not write when nothing was generated.");

    CheckImports(_namespace);
    GenerateCodeFromNamespace(_namespace);

    await _writer.FlushAsync();

    var directoryPath = Path.GetDirectoryName(path);
    Debug.Assert(directoryPath != null);

    if (!Directory.Exists(directoryPath))
      Directory.CreateDirectory(directoryPath);

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(_writer.ToString()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    _writer.GetStringBuilder().Clear();
    _namespace = new CodeNamespace(_namespace.Name);
    GenerateHeader();
  }

  protected void GenerateCodeFromStatement(CodeStatement e)
  {
    _generator.GenerateCodeFromStatement(e, _writer, _codeGeneratorOptions);
  }

  protected void GenerateCodeFromNamespace(CodeNamespace e)
  {
    _generator.GenerateCodeFromNamespace(e, _writer, _codeGeneratorOptions);
  }

  protected void AddImport(string import)
  {
    _namespace?.Imports.Add(new CodeNamespaceImport(import));
  }

  protected void AddType(CodeTypeDeclaration type)
  {
    _namespace?.Types.Add(type);
  }

  protected CodeMemberProperty CreateProperty(ApiDocProperty property)
  {
    var dataTypeInfo = property.GetDataTypeInfo();
    var propertyTypeReference = new CodeTypeReference(dataTypeInfo.ToFullTypeName());

    var memberProperty = new CodeMemberProperty
    {
      Name = property.Name.ToPublicCodeName(),
      Type = propertyTypeReference,

      // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
      Attributes = MemberAttributes.Public | MemberAttributes.Final
    };

    memberProperty.HasGet = true;
    memberProperty.HasSet = true;

    memberProperty.UserData.Add("AutoImpl", true);
    memberProperty.UserData.Add("InitExpression", CreateInitExpression(propertyTypeReference));

    // Add Attributes
    memberProperty.CustomAttributes.Add(CodeDomConst.DebuggerNonUserCodeAttribute);

    if (property.Deprecated)
      memberProperty.CustomAttributes.Add(CodeDomConst.ObsoleteAttribute);

    memberProperty.CustomAttributes.Add(new CodeAttributeDeclaration(CodeDomConst.JsonPropertyAttributeType, new CodeAttributeArgument(new CodePrimitiveExpression(property.Name))));

    // Add Comments to the property
    memberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));

    if (!string.IsNullOrEmpty(property.Description))
      foreach (var xmlDocParagraph in property.Description.ToXmlDocParagraphs())
        memberProperty.Comments.Add(new CodeCommentStatement($"<para>{xmlDocParagraph}</para>", true));

    memberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));

    // Add Converters
    if (property is { Converters.Length: > 0 }) {
      AddImport(CodeDomConst.ImportSystem);
      AddImport(CodeDomConst.ImportDeriSockConverter);

      foreach (var converterTypeName in property.Converters) {
        var jsonConverterAttribute = new CodeAttributeDeclaration(
          CodeDomConst.JsonConverterAttributeType,
          new CodeAttributeArgument(new CodeTypeOfExpression(converterTypeName)));

        memberProperty.CustomAttributes.Add(jsonConverterAttribute);
      }
    }

    return memberProperty;
  }

  private void GenerateHeader()
  {
    Array.ForEach(
      CodeDomConst.GeneratedCodeComment,
      line =>
        GenerateCodeFromStatement(new CodeCommentStatement(line)));

    GenerateCodeFromStatement(new CodeSnippetStatement("#pragma warning disable CS1591"));
    GenerateCodeFromStatement(new CodeSnippetStatement("#nullable enable"));
  }

  protected static CodeTypeReference? CreateApiMethodTypeReference(ApiDocFunction function)
  {
    var resultDataType = function.GetResponseTypeInfo();
    var isVoid = resultDataType is null;

    if (isVoid && function.IsSynchronous)
      return null;

    var returnType = isVoid ? new CodeTypeReference(typeof(JsonRpcResponse)) : new CodeTypeReference(typeof(JsonRpcResponse<>));

    if (!isVoid)
      returnType.TypeArguments.Add(new CodeTypeReference(resultDataType!.ToFullTypeName()));

    var asyncReturnType = new CodeTypeReference(typeof(Task<>));
    asyncReturnType.TypeArguments.Add(returnType);
    return asyncReturnType;
  }

  private static CodeExpression? CreateInitExpression(CodeTypeReference type)
  {
    var isArray = type.ArrayRank > 0;
    var isNullable = type.BaseType.EndsWith("?");

    if (isNullable)
      return null;

    if (isArray)
      return new CodeFieldReferenceExpression(CodeDomConst.ArrayCodeTypeRefExpr, $"Empty<{type.BaseType}>()");

    var clrType = Type.GetType(type.BaseType);

    if (clrType is null) {
      if (type.BaseType == nameof(DateTime))
        return null;

      return CodeDomConst.NotNullValueRefExpr;
    }

    if (clrType == typeof(string))
      return CodeDomConst.StringEmptyRefExpr;

    if (clrType.IsClass)
      return CodeDomConst.NotNullValueRefExpr;

    return null;
  }

  private static void CheckImports(CodeNamespace space)
  {
    var typemap = new Dictionary<string, string>
    {
      { "DateTime", CodeDomConst.ImportSystem },
      { "JToken", CodeDomConst.ImportNewtonsoftJsonLinq },
      { "JObject", CodeDomConst.ImportNewtonsoftJsonLinq }
    };

    var fnCheck = new Action<string>(
      s =>
      {
        if (string.IsNullOrEmpty(s))
          return;

        foreach (var (key, value) in typemap) {
          if (!s.Contains(key))
            continue;

          space.Imports.Add(new CodeNamespaceImport(value));
          break;
        }
      });

    foreach (CodeTypeDeclaration domType in space.Types) {
      foreach (CodeTypeReference domTypeBaseType in domType.BaseTypes)
        WalkTypeReferenceRecursive(domTypeBaseType, fnCheck);

      foreach (CodeTypeMember domTypeMember in domType.Members) {
        switch (domTypeMember) {
          case CodeMemberField field:
            WalkTypeReferenceRecursive(field.Type, fnCheck);
            break;

          case CodeMemberProperty prop:
            WalkTypeReferenceRecursive(prop.Type, fnCheck);
            break;

          case CodeMemberMethod method:
          {
            WalkTypeReferenceRecursive(method.ReturnType, fnCheck);

            foreach (CodeParameterDeclarationExpression methodParameter in method.Parameters)
              WalkTypeReferenceRecursive(methodParameter.Type, fnCheck);

            break;
          }
        }
      }
    }
  }

  private static void WalkTypeReferenceRecursive(CodeTypeReference typeRef, Action<string> fn)
  {
    fn(typeRef.BaseType);

    foreach (CodeTypeReference typeRefTypeArgument in typeRef.TypeArguments)
      WalkTypeReferenceRecursive(typeRefTypeArgument, fn);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    _writer.Dispose();
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync()
  {
    await _writer.DisposeAsync();
  }
}
