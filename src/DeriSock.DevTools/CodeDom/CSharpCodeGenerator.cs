// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copied and adjusted by Patrick Sollberger

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

#nullable disable
namespace DeriSock.DevTools.CodeDom;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

internal sealed class CSharpCodeGenerator : ICodeGenerator
{
  private const string CodeGenOutputWriter = "The output writer for code generation and the writer supplied don't match and cannot be used. This is generally caused by a bad implementation of a CodeGenerator derived class.";

  private static readonly char[] s_periodArray = { '.' };

  private const int ParameterMultilineThreshold = 15;
  private const int MaxLineLength = 80;

  private const GeneratorSupport LanguageSupport = GeneratorSupport.ArraysOfArrays |
                                                   GeneratorSupport.EntryPointMethod |
                                                   GeneratorSupport.GotoStatements |
                                                   GeneratorSupport.MultidimensionalArrays |
                                                   GeneratorSupport.StaticConstructors |
                                                   GeneratorSupport.TryCatchStatements |
                                                   GeneratorSupport.ReturnTypeAttributes |
                                                   GeneratorSupport.AssemblyAttributes |
                                                   GeneratorSupport.DeclareValueTypes |
                                                   GeneratorSupport.DeclareEnums |
                                                   GeneratorSupport.DeclareEvents |
                                                   GeneratorSupport.DeclareDelegates |
                                                   GeneratorSupport.DeclareInterfaces |
                                                   GeneratorSupport.ParameterAttributes |
                                                   GeneratorSupport.ReferenceParameters |
                                                   GeneratorSupport.ChainedConstructorArguments |
                                                   GeneratorSupport.NestedTypes |
                                                   GeneratorSupport.MultipleInterfaceMembers |
                                                   GeneratorSupport.PublicStaticMembers |
                                                   GeneratorSupport.ComplexExpressions |
                                                   GeneratorSupport.Win32Resources |
                                                   GeneratorSupport.Resources |
                                                   GeneratorSupport.PartialTypes |
                                                   GeneratorSupport.GenericTypeReference |
                                                   GeneratorSupport.GenericTypeDeclaration |
                                                   GeneratorSupport.DeclareIndexerProperties;

  private ExposedTabStringIndentedTextWriter _output;
  private CodeGeneratorOptions _options;
  private CodeTypeDeclaration _currentClass;
  private CodeTypeMember _currentMember;
  private bool _inNestedBinary;

  private bool _generatingForLoop;

  private string CurrentTypeName => _currentClass != null ? _currentClass.Name : "<% unknown %>";

  private int Indent
  {
    get => _output.Indent;
    set => _output.Indent = value;
  }

  private bool IsCurrentInterface => _currentClass is { } and not CodeTypeDelegate && _currentClass.IsInterface;

  private bool IsCurrentClass => _currentClass is { } and not CodeTypeDelegate && _currentClass.IsClass;

  private bool IsCurrentStruct => _currentClass is { } and not CodeTypeDelegate && _currentClass.IsStruct;

  private bool IsCurrentEnum => _currentClass is { } and not CodeTypeDelegate && _currentClass.IsEnum;

  private bool IsCurrentDelegate => _currentClass is CodeTypeDelegate;

  private string NullToken => "null";

  private TextWriter Output => _output;

  private string QuoteSnippetStringCStyle(string value)
  {
    var b = new StringBuilder(value.Length + 5);

    var indentObj = new Indentation(_output, Indent + 1);

    b.Append('\"');

    var i = 0;

    while (i < value.Length) {
      switch (value[i]) {
        case '\r':
          b.Append("\\r");
          break;

        case '\t':
          b.Append("\\t");
          break;

        case '\"':
          b.Append("\\\"");
          break;

        case '\'':
          b.Append("\\\'");
          break;

        case '\\':
          b.Append("\\\\");
          break;

        case '\0':
          b.Append("\\0");
          break;

        case '\n':
          b.Append("\\n");
          break;

        case '\u2028':
        case '\u2029':
        case '\u0085':
          AppendEscapedChar(b, value[i]);
          break;

        default:
          b.Append(value[i]);
          break;
      }

      if (i > 0 && i % MaxLineLength == 0) {
        // If current character is a high surrogate and the following
        // character is a low surrogate, don't break them.
        // Otherwise when we write the string to a file, we might lose
        // the characters.
        if (char.IsHighSurrogate(value[i]) && i < value.Length - 1 && char.IsLowSurrogate(value[i + 1]))
          b.Append(value[++i]);

        b.Append("\" +");
        b.Append(Environment.NewLine);
        b.Append(indentObj.IndentationString);
        b.Append('\"');
      }

      ++i;
    }

    b.Append('\"');

    return b.ToString();
  }

  private string QuoteSnippetStringVerbatimStyle(string value)
  {
    var b = new StringBuilder(value.Length + 5);

    b.Append("@\"");

    for (var i = 0; i < value.Length; i++) {
      if (value[i] == '\"')
        b.Append("\"\"");
      else
        b.Append(value[i]);
    }

    b.Append('\"');

    return b.ToString();
  }

  private string QuoteSnippetString(string value)
  {
    // If the string is short, use C style quoting (e.g "\r\n")
    // Also do it if it is too long to fit in one line
    // If the string contains '\0', verbatim style won't work.
    if (value.Length is < 256 or > 1500 || value.IndexOf('\0') != -1) // string.Contains(char) is .NetCore2.1+ specific
      return QuoteSnippetStringCStyle(value);

    // Otherwise, use 'verbatim' style quoting (e.g. @"foo")
    return QuoteSnippetStringVerbatimStyle(value);
  }

  private void ContinueOnNewLine(string st)
    => Output.WriteLine(st);

  private void OutputIdentifier(string ident)
    => Output.Write(CreateEscapedIdentifier(ident));

  private void OutputType(CodeTypeReference typeRef)
    => Output.Write(GetTypeOutput(typeRef));

  private void GenerateArrayCreateExpression(CodeArrayCreateExpression e)
  {
    Output.Write("new ");

    var init = e.Initializers;

    if (init.Count > 0) {
      OutputType(e.CreateType);

      if (e.CreateType.ArrayRank == 0)

        // Unfortunately, many clients are already calling this without array
        // types. This will allow new clients to correctly use the array type and
        // not break existing clients. For VNext, stop doing this.
        Output.Write("[]");

      Output.WriteLine(" {");
      Indent++;
      OutputExpressionList(init, true);
      Indent--;
      Output.Write('}');
    }
    else {
      Output.Write(GetBaseTypeOutput(e.CreateType));

      Output.Write('[');

      if (e.SizeExpression != null)
        GenerateExpression(e.SizeExpression);
      else
        Output.Write(e.Size);

      Output.Write(']');

      var nestedArrayDepth = e.CreateType.GetNestedArrayDepth();

      for (var i = 0; i < nestedArrayDepth - 1; i++)
        Output.Write("[]");
    }
  }

  private void GenerateBaseReferenceExpression(CodeBaseReferenceExpression e)
    => Output.Write("base");

  private void GenerateBinaryOperatorExpression(CodeBinaryOperatorExpression e)
  {
    var indentedExpression = false;
    Output.Write('(');

    GenerateExpression(e.Left);
    Output.Write(' ');

    if (e.Left is CodeBinaryOperatorExpression || e.Right is CodeBinaryOperatorExpression) {
      // In case the line gets too long with nested binary operators, we need to output them on
      // different lines. However we want to indent them to maintain readability, but this needs
      // to be done only once;
      if (!_inNestedBinary) {
        indentedExpression = true;
        _inNestedBinary = true;
        Indent += 3;
      }

      ContinueOnNewLine("");
    }

    OutputOperator(e.Operator);

    Output.Write(' ');
    GenerateExpression(e.Right);

    Output.Write(')');

    if (indentedExpression) {
      Indent -= 3;
      _inNestedBinary = false;
    }
  }

  private void GenerateCastExpression(CodeCastExpression e)
  {
    Output.Write("((");
    OutputType(e.TargetType);
    Output.Write(")(");
    GenerateExpression(e.Expression);
    Output.Write("))");
  }

  public void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
  {
    if (_output != null)
      throw new InvalidOperationException("This code generation API cannot be called while the generator is being used to generate something else.");

    _options = options ?? new CodeGeneratorOptions();
    _output = new ExposedTabStringIndentedTextWriter(writer, _options.IndentString);

    try {
      var dummyClass = new CodeTypeDeclaration();
      _currentClass = dummyClass;
      GenerateTypeMember(member, dummyClass);
    }
    finally {
      _currentClass = null;
      _output = null;
      _options = null;
    }
  }

  private void GenerateDefaultValueExpression(CodeDefaultValueExpression e)
  {
    Output.Write("default(");
    OutputType(e.Type);
    Output.Write(')');
  }

  private void GenerateDelegateCreateExpression(CodeDelegateCreateExpression e)
  {
    Output.Write("new ");
    OutputType(e.DelegateType);
    Output.Write('(');
    GenerateExpression(e.TargetObject);
    Output.Write('.');
    OutputIdentifier(e.MethodName);
    Output.Write(')');
  }

  private void GenerateEvents(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeMemberEvent) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeMemberEvent)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        GenerateEvent(imp, e);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateFields(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeMemberField) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeMemberField)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        GenerateField(imp);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateFieldReferenceExpression(CodeFieldReferenceExpression e)
  {
    if (e.TargetObject != null) {
      GenerateExpression(e.TargetObject);
      Output.Write('.');
    }

    OutputIdentifier(e.FieldName);
  }

  private void GenerateArgumentReferenceExpression(CodeArgumentReferenceExpression e)
    => OutputIdentifier(e.ParameterName);

  private void GenerateVariableReferenceExpression(CodeVariableReferenceExpression e)
    => OutputIdentifier(e.VariableName);

  private void GenerateIndexerExpression(CodeIndexerExpression e)
  {
    GenerateExpression(e.TargetObject);
    Output.Write('[');
    var first = true;

    foreach (CodeExpression exp in e.Indices) {
      if (first)
        first = false;
      else
        Output.Write(", ");

      GenerateExpression(exp);
    }

    Output.Write(']');
  }

  private void GenerateArrayIndexerExpression(CodeArrayIndexerExpression e)
  {
    GenerateExpression(e.TargetObject);
    Output.Write('[');
    var first = true;

    foreach (CodeExpression exp in e.Indices) {
      if (first)
        first = false;
      else
        Output.Write(", ");

      GenerateExpression(exp);
    }

    Output.Write(']');
  }

  private void GenerateSnippetCompileUnit(CodeSnippetCompileUnit e)
  {
    GenerateDirectives(e.StartDirectives);

    if (e.LinePragma != null)
      GenerateLinePragmaStart(e.LinePragma);

    Output.WriteLine(e.Value);

    if (e.LinePragma != null)
      GenerateLinePragmaEnd(e.LinePragma);

    if (e.EndDirectives.Count > 0)
      GenerateDirectives(e.EndDirectives);
  }

  private void GenerateSnippetExpression(CodeSnippetExpression e)
  {
    Output.Write(e.Value);
  }

  private void GenerateMethodInvokeExpression(CodeMethodInvokeExpression e)
  {
    GenerateMethodReferenceExpression(e.Method);
    Output.Write('(');
    OutputExpressionList(e.Parameters);
    Output.Write(')');
  }

  private void GenerateMethodReferenceExpression(CodeMethodReferenceExpression e)
  {
    if (e.TargetObject != null) {
      if (e.TargetObject is CodeBinaryOperatorExpression) {
        Output.Write('(');
        GenerateExpression(e.TargetObject);
        Output.Write(')');
      }
      else {
        GenerateExpression(e.TargetObject);
      }

      Output.Write('.');
    }

    OutputIdentifier(e.MethodName);

    if (e.TypeArguments.Count > 0)
      Output.Write(GetTypeArgumentsOutput(e.TypeArguments));
  }

  private bool GetUserData(CodeObject e, string property, bool defaultValue)
  {
    var o = e.UserData[property];

    if (o is bool)
      return (bool)o;

    return defaultValue;
  }

  private void GenerateNamespace(CodeNamespace e)
  {
    GenerateCommentStatements(e.Comments);
    GenerateNamespaceStart(e);

    if (GetUserData(e, "GenerateImports", true))
      GenerateNamespaceImports(e);

    Output.WriteLine();

    GenerateTypes(e);
    GenerateNamespaceEnd(e);
  }

  private void GenerateStatement(CodeStatement e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof(e));

    if (e.StartDirectives.Count > 0)
      GenerateDirectives(e.StartDirectives);

    if (e.LinePragma != null)
      GenerateLinePragmaStart(e.LinePragma);

    if (e is CodeCommentStatement) {
      GenerateCommentStatement((CodeCommentStatement)e);
    }
    else if (e is CodeMethodReturnStatement) {
      GenerateMethodReturnStatement((CodeMethodReturnStatement)e);
    }
    else if (e is CodeConditionStatement) {
      GenerateConditionStatement((CodeConditionStatement)e);
    }
    else if (e is CodeTryCatchFinallyStatement) {
      GenerateTryCatchFinallyStatement((CodeTryCatchFinallyStatement)e);
    }
    else if (e is CodeAssignStatement) {
      GenerateAssignStatement((CodeAssignStatement)e);
    }
    else if (e is CodeExpressionStatement) {
      GenerateExpressionStatement((CodeExpressionStatement)e);
    }
    else if (e is CodeIterationStatement) {
      GenerateIterationStatement((CodeIterationStatement)e);
    }
    else if (e is CodeThrowExceptionStatement) {
      GenerateThrowExceptionStatement((CodeThrowExceptionStatement)e);
    }
    else if (e is CodeSnippetStatement) {
      // Don't indent snippet statements, in order to preserve the column
      // information from the original code.  This improves the debugging
      // experience.
      var savedIndent = Indent;
      Indent = 0;

      GenerateSnippetStatement((CodeSnippetStatement)e);

      // Restore the indent
      Indent = savedIndent;
    }
    else if (e is CodeVariableDeclarationStatement) {
      GenerateVariableDeclarationStatement((CodeVariableDeclarationStatement)e);
    }
    else if (e is CodeAttachEventStatement) {
      GenerateAttachEventStatement((CodeAttachEventStatement)e);
    }
    else if (e is CodeRemoveEventStatement) {
      GenerateRemoveEventStatement((CodeRemoveEventStatement)e);
    }
    else if (e is CodeGotoStatement) {
      GenerateGotoStatement((CodeGotoStatement)e);
    }
    else if (e is CodeLabeledStatement) {
      GenerateLabeledStatement((CodeLabeledStatement)e);
    }
    else {
      throw new ArgumentException($"Element type {e.GetType().FullName} is not supported.", nameof(e));
    }

    if (e.LinePragma != null)
      GenerateLinePragmaEnd(e.LinePragma);

    if (e.EndDirectives.Count > 0)
      GenerateDirectives(e.EndDirectives);
  }

  private void GenerateStatements(CodeStatementCollection stmts)
  {
    foreach (CodeStatement stmt in stmts)
      ((ICodeGenerator)this).GenerateCodeFromStatement(stmt, _output.InnerWriter, _options);
  }

  private void GenerateNamespaceImports(CodeNamespace e)
  {
    foreach (CodeNamespaceImport imp in e.Imports) {
      if (imp.LinePragma != null)
        GenerateLinePragmaStart(imp.LinePragma);

      GenerateNamespaceImport(imp);

      if (imp.LinePragma != null)
        GenerateLinePragmaEnd(imp.LinePragma);
    }
  }

  private void GenerateEventReferenceExpression(CodeEventReferenceExpression e)
  {
    if (e.TargetObject != null) {
      GenerateExpression(e.TargetObject);
      Output.Write('.');
    }

    OutputIdentifier(e.EventName);
  }

  private void GenerateDelegateInvokeExpression(CodeDelegateInvokeExpression e)
  {
    if (e.TargetObject != null)
      GenerateExpression(e.TargetObject);

    Output.Write('(');
    OutputExpressionList(e.Parameters);
    Output.Write(')');
  }

  private void GenerateObjectCreateExpression(CodeObjectCreateExpression e)
  {
    Output.Write("new ");
    OutputType(e.CreateType);
    Output.Write('(');
    OutputExpressionList(e.Parameters);
    Output.Write(')');
  }

  private void GeneratePrimitiveExpression(CodePrimitiveExpression e)
  {
    if (e.Value is char value) {
      GeneratePrimitiveChar(value);
    }
    else if (e.Value is sbyte eValue) {
      // C# has no literal marker for types smaller than Int32
      Output.Write(eValue.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is ushort value1) {
      // C# has no literal marker for types smaller than Int32, and you will
      // get a conversion error if you use "u" here.
      Output.Write(value1.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is uint u) {
      Output.Write(u.ToString(CultureInfo.InvariantCulture));
      Output.Write('u');
    }
    else if (e.Value is ulong eValue1) {
      Output.Write(eValue1.ToString(CultureInfo.InvariantCulture));
      Output.Write("ul");
    }
    else if (e.Value == null) {
      Output.Write(NullToken);
    }
    else if (e.Value is string s) {
      Output.Write(QuoteSnippetString(s));
    }
    else if (e.Value is byte b) {
      Output.Write(b.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is short s1) {
      Output.Write(s1.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is int i) {
      Output.Write(i.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is long l) {
      Output.Write(l.ToString(CultureInfo.InvariantCulture));
    }
    else if (e.Value is float f) {
      GenerateSingleFloatValue(f);
    }
    else if (e.Value is double d) {
      GenerateDoubleValue(d);
    }
    else if (e.Value is decimal value2) {
      GenerateDecimalValue(value2);
    }
    else if (e.Value is bool b1) {
      Output.Write(b1 ? "true" : "false");
    }
    else {
      throw new ArgumentException($"Invalid Primitive Type: {e.Value.GetType()}. Consider using CodeObjectCreateExpression.", nameof(e));

      //throw new ArgumentException(SR.Format(SR.InvalidPrimitiveType, e.Value.GetType()), nameof(e));
    }
  }

  private void GeneratePrimitiveChar(char c)
  {
    Output.Write('\'');

    switch (c) {
      case '\r':
        Output.Write("\\r");
        break;

      case '\t':
        Output.Write("\\t");
        break;

      case '\"':
        Output.Write("\\\"");
        break;

      case '\'':
        Output.Write("\\\'");
        break;

      case '\\':
        Output.Write("\\\\");
        break;

      case '\0':
        Output.Write("\\0");
        break;

      case '\n':
        Output.Write("\\n");
        break;

      case '\u2028':
      case '\u2029':
      case '\u0084':
      case '\u0085':
        AppendEscapedChar(null, c);
        break;

      default:
        if (char.IsSurrogate(c))
          AppendEscapedChar(null, c);
        else
          Output.Write(c);

        break;
    }

    Output.Write('\'');
  }

  private void AppendEscapedChar(StringBuilder b, char value)
  {
    if (b == null) {
      Output.Write("\\u");
      Output.Write(((int)value).ToString("X4"));
    }
    else {
      b.Append("\\u");
      b.Append(((int)value).ToString("X4"));
    }
  }

  private void GeneratePropertySetValueReferenceExpression(CodePropertySetValueReferenceExpression e)
    => Output.Write("value");

  private void GenerateThisReferenceExpression(CodeThisReferenceExpression e)
    => Output.Write("this");

  private void GenerateExpressionStatement(CodeExpressionStatement e)
  {
    GenerateExpression(e.Expression);

    if (!_generatingForLoop)
      Output.WriteLine(';');
  }

  private void GenerateIterationStatement(CodeIterationStatement e)
  {
    _generatingForLoop = true;
    Output.Write("for (");
    GenerateStatement(e.InitStatement);
    Output.Write("; ");
    GenerateExpression(e.TestExpression);
    Output.Write("; ");
    GenerateStatement(e.IncrementStatement);
    Output.Write(')');
    OutputStartingBrace();
    _generatingForLoop = false;
    Indent++;
    GenerateStatements(e.Statements);
    Indent--;
    Output.WriteLine('}');
  }

  private void GenerateThrowExceptionStatement(CodeThrowExceptionStatement e)
  {
    Output.Write("throw");

    if (e.ToThrow != null) {
      Output.Write(' ');
      GenerateExpression(e.ToThrow);
    }

    Output.WriteLine(';');
  }

  private void GenerateComment(CodeComment e)
  {
    var commentLineStart = e.DocComment ? "///" : "//";
    Output.Write(commentLineStart);
    Output.Write(' ');
    var isAfterCommentLineStart = false;

    var value = e.Text;

    for (var i = 0; i < value.Length; i++) {
      if (isAfterCommentLineStart) {
        if (value[i] == '/' && (e.DocComment || !value.HasCharAt(i + 1, '/')))
          Output.Write(' ');

        isAfterCommentLineStart = false;
      }

      if (value[i] == '\u0000')
        continue;

      Output.Write(value[i]);

      if (value[i] == '\r') {
        if (value.HasCharAt(i + 1, '\n')) { // if next char is '\n', skip it
          Output.Write('\n');
          i++;
        }

        _output.InternalOutputTabs();
        Output.Write(commentLineStart);
        isAfterCommentLineStart = true;
      }
      else if (value[i] == '\n') {
        _output.InternalOutputTabs();
        Output.Write(commentLineStart);
        isAfterCommentLineStart = true;
      }
      else if (value[i] == '\u2028' || value[i] == '\u2029' || value[i] == '\u0085') {
        Output.Write(commentLineStart);
        isAfterCommentLineStart = true;
      }
    }

    Output.WriteLine();
  }

  private void GenerateCommentStatement(CodeCommentStatement e)
  {
    if (e.Comment == null)
      throw new ArgumentException($"The 'Comment' property of the CodeCommentStatement '{nameof(e)}' cannot be null.", nameof(e));

    GenerateComment(e.Comment);
  }

  private void GenerateCommentStatements(CodeCommentStatementCollection e)
  {
    foreach (CodeCommentStatement comment in e)
      GenerateCommentStatement(comment);
  }

  private void GenerateMethodReturnStatement(CodeMethodReturnStatement e)
  {
    Output.Write("return");

    if (e.Expression != null) {
      Output.Write(' ');
      GenerateExpression(e.Expression);
    }

    Output.WriteLine(';');
  }

  private void GenerateConditionStatement(CodeConditionStatement e)
  {
    Output.Write("if (");
    GenerateExpression(e.Condition);
    Output.Write(')');
    OutputStartingBrace();
    Indent++;
    GenerateStatements(e.TrueStatements);
    Indent--;

    var falseStatements = e.FalseStatements;

    if (falseStatements.Count > 0) {
      Output.Write('}');

      if (_options.ElseOnClosing)
        Output.Write(' ');
      else
        Output.WriteLine();

      Output.Write("else");
      OutputStartingBrace();
      Indent++;
      GenerateStatements(e.FalseStatements);
      Indent--;
    }

    Output.WriteLine('}');
  }

  private void GenerateTryCatchFinallyStatement(CodeTryCatchFinallyStatement e)
  {
    Output.Write("try");
    OutputStartingBrace();
    Indent++;
    GenerateStatements(e.TryStatements);
    Indent--;
    var catches = e.CatchClauses;

    if (catches.Count > 0)
      foreach (CodeCatchClause current in catches) {
        Output.Write('}');

        if (_options.ElseOnClosing)
          Output.Write(' ');
        else
          Output.WriteLine();

        Output.Write("catch (");
        OutputType(current.CatchExceptionType);
        Output.Write(' ');
        OutputIdentifier(current.LocalName);
        Output.Write(')');
        OutputStartingBrace();
        Indent++;
        GenerateStatements(current.Statements);
        Indent--;
      }

    var finallyStatements = e.FinallyStatements;

    if (finallyStatements.Count > 0) {
      Output.Write('}');

      if (_options.ElseOnClosing)
        Output.Write(' ');
      else
        Output.WriteLine();

      Output.Write("finally");
      OutputStartingBrace();
      Indent++;
      GenerateStatements(finallyStatements);
      Indent--;
    }

    Output.WriteLine('}');
  }

  private void GenerateAssignStatement(CodeAssignStatement e)
  {
    GenerateExpression(e.Left);
    Output.Write(" = ");
    GenerateExpression(e.Right);

    if (!_generatingForLoop)
      Output.WriteLine(';');
  }

  private void GenerateAttachEventStatement(CodeAttachEventStatement e)
  {
    GenerateEventReferenceExpression(e.Event);
    Output.Write(" += ");
    GenerateExpression(e.Listener);
    Output.WriteLine(';');
  }

  private void GenerateRemoveEventStatement(CodeRemoveEventStatement e)
  {
    GenerateEventReferenceExpression(e.Event);
    Output.Write(" -= ");
    GenerateExpression(e.Listener);
    Output.WriteLine(';');
  }

  private void GenerateSnippetStatement(CodeSnippetStatement e)
  {
    Output.WriteLine(e.Value);
  }

  private void GenerateGotoStatement(CodeGotoStatement e)
  {
    Output.Write("goto ");
    Output.Write(e.Label);
    Output.WriteLine(';');
  }

  private void GenerateLabeledStatement(CodeLabeledStatement e)
  {
    Indent--;
    Output.Write(e.Label);
    Output.WriteLine(':');
    Indent++;

    if (e.Statement != null)
      GenerateStatement(e.Statement);
  }

  private void GenerateVariableDeclarationStatement(CodeVariableDeclarationStatement e)
  {
    OutputTypeNamePair(e.Type, e.Name);

    if (e.InitExpression != null) {
      Output.Write(" = ");
      GenerateExpression(e.InitExpression);
    }

    if (!_generatingForLoop)
      Output.WriteLine(';');
  }

  private void GenerateLinePragmaStart(CodeLinePragma e)
  {
    Output.WriteLine();
    Output.Write("#line ");
    Output.Write(e.LineNumber);
    Output.Write(" \"");
    Output.Write(e.FileName);
    Output.Write('\"');
    Output.WriteLine();
  }

  private void GenerateLinePragmaEnd(CodeLinePragma e)
  {
    Output.WriteLine();
    Output.WriteLine("#line default");
    Output.WriteLine("#line hidden");
  }

  private void GenerateEvent(CodeMemberEvent e, CodeTypeDeclaration c)
  {
    if (IsCurrentDelegate || IsCurrentEnum)
      return;

    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    if (e.PrivateImplementationType == null)
      OutputMemberAccessModifier(e.Attributes);

    Output.Write("event ");
    var name = e.Name;

    if (e.PrivateImplementationType != null)
      name = GetBaseTypeOutput(e.PrivateImplementationType, false) + "." + name;

    OutputTypeNamePair(e.Type, name);
    Output.WriteLine(';');
  }

  private void GenerateExpression(CodeExpression e)
  {
    if (e is CodeArrayCreateExpression) {
      GenerateArrayCreateExpression((CodeArrayCreateExpression)e);
    }
    else if (e is CodeBaseReferenceExpression) {
      GenerateBaseReferenceExpression((CodeBaseReferenceExpression)e);
    }
    else if (e is CodeBinaryOperatorExpression) {
      GenerateBinaryOperatorExpression((CodeBinaryOperatorExpression)e);
    }
    else if (e is CodeCastExpression) {
      GenerateCastExpression((CodeCastExpression)e);
    }
    else if (e is CodeDelegateCreateExpression) {
      GenerateDelegateCreateExpression((CodeDelegateCreateExpression)e);
    }
    else if (e is CodeFieldReferenceExpression) {
      GenerateFieldReferenceExpression((CodeFieldReferenceExpression)e);
    }
    else if (e is CodeArgumentReferenceExpression) {
      GenerateArgumentReferenceExpression((CodeArgumentReferenceExpression)e);
    }
    else if (e is CodeVariableReferenceExpression) {
      GenerateVariableReferenceExpression((CodeVariableReferenceExpression)e);
    }
    else if (e is CodeIndexerExpression) {
      GenerateIndexerExpression((CodeIndexerExpression)e);
    }
    else if (e is CodeArrayIndexerExpression) {
      GenerateArrayIndexerExpression((CodeArrayIndexerExpression)e);
    }
    else if (e is CodeSnippetExpression) {
      GenerateSnippetExpression((CodeSnippetExpression)e);
    }
    else if (e is CodeMethodInvokeExpression) {
      GenerateMethodInvokeExpression((CodeMethodInvokeExpression)e);
    }
    else if (e is CodeMethodReferenceExpression) {
      GenerateMethodReferenceExpression((CodeMethodReferenceExpression)e);
    }
    else if (e is CodeEventReferenceExpression) {
      GenerateEventReferenceExpression((CodeEventReferenceExpression)e);
    }
    else if (e is CodeDelegateInvokeExpression) {
      GenerateDelegateInvokeExpression((CodeDelegateInvokeExpression)e);
    }
    else if (e is CodeObjectCreateExpression) {
      GenerateObjectCreateExpression((CodeObjectCreateExpression)e);
    }
    else if (e is CodeParameterDeclarationExpression) {
      GenerateParameterDeclarationExpression((CodeParameterDeclarationExpression)e);
    }
    else if (e is CodeDirectionExpression) {
      GenerateDirectionExpression((CodeDirectionExpression)e);
    }
    else if (e is CodePrimitiveExpression) {
      GeneratePrimitiveExpression((CodePrimitiveExpression)e);
    }
    else if (e is CodePropertyReferenceExpression) {
      GeneratePropertyReferenceExpression((CodePropertyReferenceExpression)e);
    }
    else if (e is CodePropertySetValueReferenceExpression) {
      GeneratePropertySetValueReferenceExpression((CodePropertySetValueReferenceExpression)e);
    }
    else if (e is CodeThisReferenceExpression) {
      GenerateThisReferenceExpression((CodeThisReferenceExpression)e);
    }
    else if (e is CodeTypeReferenceExpression) {
      GenerateTypeReferenceExpression((CodeTypeReferenceExpression)e);
    }
    else if (e is CodeTypeOfExpression) {
      GenerateTypeOfExpression((CodeTypeOfExpression)e);
    }
    else if (e is CodeDefaultValueExpression) {
      GenerateDefaultValueExpression((CodeDefaultValueExpression)e);
    }
    else {
      if (e == null)
        throw new ArgumentNullException(nameof(e));

      throw new ArgumentException($"Element type {e.GetType().FullName} is not suppported.", nameof(e));
    }
  }

  private void GenerateField(CodeMemberField e)
  {
    if (IsCurrentDelegate || IsCurrentInterface)
      return;

    if (IsCurrentEnum) {
      if (e.CustomAttributes.Count > 0)
        GenerateAttributes(e.CustomAttributes);

      OutputIdentifier(e.Name);

      if (e.InitExpression != null) {
        Output.Write(" = ");
        GenerateExpression(e.InitExpression);
      }

      Output.WriteLine(',');
    }
    else {
      if (e.CustomAttributes.Count > 0)
        GenerateAttributes(e.CustomAttributes);

      OutputMemberAccessModifier(e.Attributes);
      OutputVTableModifier(e.Attributes);
      OutputFieldScopeModifier(e.Attributes);
      OutputTypeNamePair(e.Type, e.Name);

      if (e.InitExpression != null) {
        Output.Write(" = ");
        GenerateExpression(e.InitExpression);
      }

      Output.WriteLine(';');
    }
  }

  private void GenerateSnippetMember(CodeSnippetTypeMember e)
    => Output.Write(e.Text);

  private void GenerateParameterDeclarationExpression(CodeParameterDeclarationExpression e)
  {
    if (e.CustomAttributes.Count > 0)

      // Parameter attributes should be in-line for readability
      GenerateAttributes(e.CustomAttributes, null, true);

    OutputDirection(e.Direction);
    OutputTypeNamePair(e.Type, e.Name);

    var initExpression = e.UserData["InitExpression"];

    if (initExpression is CodeExpression ce) {
      Output.Write(" = ");
      GenerateExpression(ce);
    }
  }

  private void GenerateEntryPointMethod(CodeEntryPointMethod e, CodeTypeDeclaration c)
  {
    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    Output.Write("public static ");
    OutputType(e.ReturnType);
    Output.Write(" Main()");
    OutputStartingBrace();
    Indent++;

    GenerateStatements(e.Statements);

    Indent--;
    Output.WriteLine('}');
  }

  private void GenerateMethods(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeMemberMethod and not CodeTypeConstructor and not CodeConstructor) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeMemberMethod)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        if (current is CodeEntryPointMethod)
          GenerateEntryPointMethod((CodeEntryPointMethod)current, e);
        else
          GenerateMethod(imp, e);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateMethod(CodeMemberMethod e, CodeTypeDeclaration c)
  {
    if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface))
      return;

    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    if (e.ReturnTypeCustomAttributes.Count > 0)
      GenerateAttributes(e.ReturnTypeCustomAttributes, "return: ");

    if (!IsCurrentInterface) {
      if (e.PrivateImplementationType == null) {
        OutputMemberAccessModifier(e.Attributes);
        OutputVTableModifier(e.Attributes);
        OutputMemberScopeModifier(e.Attributes);
      }
    }
    else {
      // interfaces still need "new"
      OutputVTableModifier(e.Attributes);
    }

    OutputType(e.ReturnType);
    Output.Write(' ');

    if (e.PrivateImplementationType != null) {
      Output.Write(GetBaseTypeOutput(e.PrivateImplementationType, false));
      Output.Write('.');
    }

    OutputIdentifier(e.Name);

    OutputTypeParameters(e.TypeParameters);

    Output.Write('(');
    OutputParameters(e.Parameters);
    Output.Write(')');

    OutputTypeParameterConstraints(e.TypeParameters);

    if (!IsCurrentInterface
        && (e.Attributes & MemberAttributes.ScopeMask) != MemberAttributes.Abstract) {
      OutputStartingBrace();
      Indent++;

      GenerateStatements(e.Statements);

      Indent--;
      Output.WriteLine('}');
    }
    else {
      Output.WriteLine(';');
    }
  }

  private void GenerateProperties(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeMemberProperty) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeMemberProperty)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        GenerateProperty(imp, e);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateProperty(CodeMemberProperty e, CodeTypeDeclaration c)
  {
    if (!(IsCurrentClass || IsCurrentStruct || IsCurrentInterface))
      return;

    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    if (!IsCurrentInterface) {
      if (e.PrivateImplementationType == null) {
        OutputMemberAccessModifier(e.Attributes);
        OutputVTableModifier(e.Attributes);
        OutputMemberScopeModifier(e.Attributes);
      }
    }
    else {
      OutputVTableModifier(e.Attributes);
    }

    OutputType(e.Type);
    Output.Write(' ');

    if (e.PrivateImplementationType != null && !IsCurrentInterface) {
      Output.Write(GetBaseTypeOutput(e.PrivateImplementationType, false));
      Output.Write('.');
    }

    if (e.Parameters.Count > 0 && string.Equals(e.Name, "Item", StringComparison.OrdinalIgnoreCase)) {
      Output.Write("this[");
      OutputParameters(e.Parameters);
      Output.Write(']');
    }
    else {
      OutputIdentifier(e.Name);
    }

    if ((IsCurrentInterface || GetUserData(e, "AutoImpl", false)) && (e.HasGet || e.HasSet)) {
      Output.Write(" {");

      if (e.HasGet)
        Output.Write(" get;");

      if (e.HasSet)
        Output.Write(" set;");

      Output.Write(" }");

      var initExpression = e.UserData["InitExpression"];

      if (initExpression is CodeExpression ce) {
        Output.Write(" = ");
        GenerateExpression(ce);
        Output.Write(";");
      }

      Output.WriteLine();

      return;
    }

    OutputStartingBrace();
    Indent++;

    if (e.HasGet) {
      if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract) {
        Output.WriteLine("get;");
      }
      else {
        Output.Write("get");
        OutputStartingBrace();
        Indent++;
        GenerateStatements(e.GetStatements);
        Indent--;
        Output.WriteLine('}');
      }
    }

    if (e.HasSet) {
      if (IsCurrentInterface || (e.Attributes & MemberAttributes.ScopeMask) == MemberAttributes.Abstract) {
        Output.WriteLine("set;");
      }
      else {
        Output.Write("set");
        OutputStartingBrace();
        Indent++;
        GenerateStatements(e.SetStatements);
        Indent--;
        Output.WriteLine('}');
      }
    }

    Indent--;
    Output.WriteLine('}');
  }

  private void GenerateSingleFloatValue(float s)
  {
    if (float.IsNaN(s)) {
      Output.Write("float.NaN");
    }
    else if (float.IsNegativeInfinity(s)) {
      Output.Write("float.NegativeInfinity");
    }
    else if (float.IsPositiveInfinity(s)) {
      Output.Write("float.PositiveInfinity");
    }
    else {
      Output.Write(s.ToString(CultureInfo.InvariantCulture));
      Output.Write('F');
    }
  }

  private void GenerateDoubleValue(double d)
  {
    if (double.IsNaN(d)) {
      Output.Write("double.NaN");
    }
    else if (double.IsNegativeInfinity(d)) {
      Output.Write("double.NegativeInfinity");
    }
    else if (double.IsPositiveInfinity(d)) {
      Output.Write("double.PositiveInfinity");
    }
    else {
      Output.Write(d.ToString("R", CultureInfo.InvariantCulture));

      // always mark a double as being a double in case we have no decimal portion (e.g write 1D instead of 1 which is an int)
      Output.Write('D');
    }
  }

  private void GenerateDecimalValue(decimal d)
  {
    Output.Write(d.ToString(CultureInfo.InvariantCulture));
    Output.Write('m');
  }

  private void OutputVTableModifier(MemberAttributes attributes)
  {
    switch (attributes & MemberAttributes.VTableMask) {
      case MemberAttributes.New:
        Output.Write("new ");
        break;
    }
  }

  private void OutputMemberAccessModifier(MemberAttributes attributes)
  {
    switch (attributes & MemberAttributes.AccessMask) {
      case MemberAttributes.Assembly:
        Output.Write("internal ");
        break;

      case MemberAttributes.FamilyAndAssembly:
        Output.Write("internal "); /*FamANDAssem*/
        break;

      case MemberAttributes.Family:
        Output.Write("protected ");
        break;

      case MemberAttributes.FamilyOrAssembly:
        Output.Write("protected internal ");
        break;

      case MemberAttributes.Private:
        Output.Write("private ");
        break;

      case MemberAttributes.Public:
        Output.Write("public ");
        break;
    }
  }

  private void OutputMemberScopeModifier(MemberAttributes attributes)
  {
    switch (attributes & MemberAttributes.ScopeMask) {
      case MemberAttributes.Abstract:
        Output.Write("abstract ");
        break;

      case MemberAttributes.Final:
        Output.Write("");
        break;

      case MemberAttributes.Static:
        Output.Write("static ");
        break;

      case MemberAttributes.Override:
        Output.Write("override ");
        break;

      default:
        switch (attributes & MemberAttributes.AccessMask) {
          case MemberAttributes.Family:
          case MemberAttributes.Public:
          case MemberAttributes.Assembly:
            Output.Write("virtual ");
            break;
        }

        break;
    }
  }

  private void OutputOperator(CodeBinaryOperatorType op)
  {
    switch (op) {
      case CodeBinaryOperatorType.Add:
        Output.Write('+');
        break;

      case CodeBinaryOperatorType.Subtract:
        Output.Write('-');
        break;

      case CodeBinaryOperatorType.Multiply:
        Output.Write('*');
        break;

      case CodeBinaryOperatorType.Divide:
        Output.Write('/');
        break;

      case CodeBinaryOperatorType.Modulus:
        Output.Write('%');
        break;

      case CodeBinaryOperatorType.Assign:
        Output.Write('=');
        break;

      case CodeBinaryOperatorType.IdentityInequality:
        Output.Write("!=");
        break;

      case CodeBinaryOperatorType.IdentityEquality:
        Output.Write("==");
        break;

      case CodeBinaryOperatorType.ValueEquality:
        Output.Write("==");
        break;

      case CodeBinaryOperatorType.BitwiseOr:
        Output.Write('|');
        break;

      case CodeBinaryOperatorType.BitwiseAnd:
        Output.Write('&');
        break;

      case CodeBinaryOperatorType.BooleanOr:
        Output.Write("||");
        break;

      case CodeBinaryOperatorType.BooleanAnd:
        Output.Write("&&");
        break;

      case CodeBinaryOperatorType.LessThan:
        Output.Write('<');
        break;

      case CodeBinaryOperatorType.LessThanOrEqual:
        Output.Write("<=");
        break;

      case CodeBinaryOperatorType.GreaterThan:
        Output.Write('>');
        break;

      case CodeBinaryOperatorType.GreaterThanOrEqual:
        Output.Write(">=");
        break;
    }
  }

  private void OutputFieldScopeModifier(MemberAttributes attributes)
  {
    switch (attributes & MemberAttributes.ScopeMask) {
      case MemberAttributes.Final:
        Output.Write("readonly ");
        break;

      case MemberAttributes.Static:
        Output.Write("static ");
        break;

      case MemberAttributes.Const:
        Output.Write("const ");
        break;
    }
  }

  private void GeneratePropertyReferenceExpression(CodePropertyReferenceExpression e)
  {
    if (e.TargetObject != null) {
      GenerateExpression(e.TargetObject);
      Output.Write('.');
    }

    OutputIdentifier(e.PropertyName);
  }

  private void GenerateConstructors(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeConstructor) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeConstructor)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        GenerateConstructor(imp, e);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateConstructor(CodeConstructor e, CodeTypeDeclaration c)
  {
    if (!(IsCurrentClass || IsCurrentStruct))
      return;

    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    OutputMemberAccessModifier(e.Attributes);
    OutputIdentifier(CurrentTypeName);
    Output.Write('(');
    OutputParameters(e.Parameters);
    Output.Write(')');

    var baseArgs = e.BaseConstructorArgs;
    var thisArgs = e.ChainedConstructorArgs;

    if (baseArgs.Count > 0) {
      Output.WriteLine(" : ");
      Indent++;
      Indent++;
      Output.Write("base(");
      OutputExpressionList(baseArgs);
      Output.Write(')');
      Indent--;
      Indent--;
    }

    if (thisArgs.Count > 0) {
      Output.WriteLine(" : ");
      Indent++;
      Indent++;
      Output.Write("this(");
      OutputExpressionList(thisArgs);
      Output.Write(')');
      Indent--;
      Indent--;
    }

    OutputStartingBrace();
    Indent++;
    GenerateStatements(e.Statements);
    Indent--;
    Output.WriteLine('}');
  }

  private void GenerateTypeConstructor(CodeTypeConstructor e)
  {
    if (!(IsCurrentClass || IsCurrentStruct))
      return;

    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    Output.Write("static ");
    Output.Write(CurrentTypeName);
    Output.Write("()");
    OutputStartingBrace();
    Indent++;
    GenerateStatements(e.Statements);
    Indent--;
    Output.WriteLine('}');
  }

  private void GenerateTypeReferenceExpression(CodeTypeReferenceExpression e)
    => OutputType(e.Type);

  private void GenerateTypeOfExpression(CodeTypeOfExpression e)
  {
    Output.Write("typeof(");
    OutputType(e.Type);
    Output.Write(')');
  }

  private void GenerateType(CodeTypeDeclaration e)
  {
    _currentClass = e;

    if (e.StartDirectives.Count > 0)
      GenerateDirectives(e.StartDirectives);

    GenerateCommentStatements(e.Comments);

    if (e.LinePragma != null)
      GenerateLinePragmaStart(e.LinePragma);

    GenerateTypeStart(e);

    if (_options.VerbatimOrder) {
      foreach (CodeTypeMember member in e.Members)
        GenerateTypeMember(member, e);
    }
    else {
      GenerateFields(e);

      GenerateSnippetMembers(e);

      GenerateTypeConstructors(e);

      GenerateConstructors(e);

      GenerateProperties(e);

      GenerateEvents(e);

      GenerateMethods(e);

      GenerateNestedTypes(e);
    }

    // Nested types clobber the current class, so reset it.
    _currentClass = e;

    GenerateTypeEnd(e);

    if (e.LinePragma != null)
      GenerateLinePragmaEnd(e.LinePragma);

    if (e.EndDirectives.Count > 0)
      GenerateDirectives(e.EndDirectives);
  }

  private void GenerateTypes(CodeNamespace e)
  {
    foreach (CodeTypeDeclaration c in e.Types) {
      if (_options.BlankLinesBetweenMembers)
        Output.WriteLine();

      ((ICodeGenerator)this).GenerateCodeFromType(c, _output.InnerWriter, _options);
    }
  }

  private void GenerateTypeStart(CodeTypeDeclaration e)
  {
    if (e.CustomAttributes.Count > 0)
      GenerateAttributes(e.CustomAttributes);

    if (IsCurrentDelegate) {
      switch (e.TypeAttributes & TypeAttributes.VisibilityMask) {
        case TypeAttributes.Public:
          Output.Write("public ");
          break;

        case TypeAttributes.NotPublic:
        default:
          break;
      }

      var del = (CodeTypeDelegate)e;
      Output.Write("delegate ");
      OutputType(del.ReturnType);
      Output.Write(' ');
      OutputIdentifier(e.Name);
      Output.Write('(');
      OutputParameters(del.Parameters);
      Output.WriteLine(");");
    }
    else {
      OutputTypeAttributes(e);
      OutputIdentifier(e.Name);

      OutputTypeParameters(e.TypeParameters);

      var first = true;

      foreach (CodeTypeReference typeRef in e.BaseTypes) {
        if (first) {
          Output.Write(" : ");
          first = false;
        }
        else {
          Output.Write(", ");
        }

        OutputType(typeRef);
      }

      OutputTypeParameterConstraints(e.TypeParameters);

      OutputStartingBrace();
      Indent++;
    }
  }

  private void GenerateTypeMember(CodeTypeMember member, CodeTypeDeclaration declaredType)
  {
    if (_options.BlankLinesBetweenMembers)
      Output.WriteLine();

    if (member is CodeTypeDeclaration) {
      ((ICodeGenerator)this).GenerateCodeFromType((CodeTypeDeclaration)member, _output.InnerWriter, _options);

      // Nested types clobber the current class, so reset it.
      _currentClass = declaredType;

      // For nested types, comments and line pragmas are handled separately, so return here
      return;
    }

    if (member.StartDirectives.Count > 0)
      GenerateDirectives(member.StartDirectives);

    GenerateCommentStatements(member.Comments);

    if (member.LinePragma != null)
      GenerateLinePragmaStart(member.LinePragma);

    if (member is CodeMemberField) {
      GenerateField((CodeMemberField)member);
    }
    else if (member is CodeMemberProperty) {
      GenerateProperty((CodeMemberProperty)member, declaredType);
    }
    else if (member is CodeMemberMethod) {
      if (member is CodeConstructor)
        GenerateConstructor((CodeConstructor)member, declaredType);
      else if (member is CodeTypeConstructor)
        GenerateTypeConstructor((CodeTypeConstructor)member);
      else if (member is CodeEntryPointMethod)
        GenerateEntryPointMethod((CodeEntryPointMethod)member, declaredType);
      else
        GenerateMethod((CodeMemberMethod)member, declaredType);
    }
    else if (member is CodeMemberEvent) {
      GenerateEvent((CodeMemberEvent)member, declaredType);
    }
    else if (member is CodeSnippetTypeMember) {
      // Don't indent snippets, in order to preserve the column
      // information from the original code.  This improves the debugging
      // experience.
      var savedIndent = Indent;
      Indent = 0;

      GenerateSnippetMember((CodeSnippetTypeMember)member);

      // Restore the indent
      Indent = savedIndent;

      // Generate an extra new line at the end of the snippet.
      // If the snippet is comment and this type only contains comments.
      // The generated code will not compile.
      Output.WriteLine();
    }

    if (member.LinePragma != null)
      GenerateLinePragmaEnd(member.LinePragma);

    if (member.EndDirectives.Count > 0)
      GenerateDirectives(member.EndDirectives);
  }

  private void GenerateTypeConstructors(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeTypeConstructor) {
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeTypeConstructor)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        GenerateTypeConstructor(imp);

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }
  }

  private void GenerateSnippetMembers(CodeTypeDeclaration e)
  {
    var hasSnippet = false;

    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeSnippetTypeMember) {
        hasSnippet = true;
        _currentMember = current;

        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        if (_currentMember.StartDirectives.Count > 0)
          GenerateDirectives(_currentMember.StartDirectives);

        GenerateCommentStatements(_currentMember.Comments);
        var imp = (CodeSnippetTypeMember)current;

        if (imp.LinePragma != null)
          GenerateLinePragmaStart(imp.LinePragma);

        // Don't indent snippets, in order to preserve the column
        // information from the original code.  This improves the debugging
        // experience.
        var savedIndent = Indent;
        Indent = 0;

        GenerateSnippetMember(imp);

        // Restore the indent
        Indent = savedIndent;

        if (imp.LinePragma != null)
          GenerateLinePragmaEnd(imp.LinePragma);

        if (_currentMember.EndDirectives.Count > 0)
          GenerateDirectives(_currentMember.EndDirectives);
      }
    }

    // Generate an extra new line at the end of the snippet.
    // If the snippet is comment and this type only contains comments.
    // The generated code will not compile.
    if (hasSnippet)
      Output.WriteLine();
  }

  private void GenerateNestedTypes(CodeTypeDeclaration e)
  {
    foreach (CodeTypeMember current in e.Members) {
      if (current is CodeTypeDeclaration) {
        if (_options.BlankLinesBetweenMembers)
          Output.WriteLine();

        var currentClass = (CodeTypeDeclaration)current;
        ((ICodeGenerator)this).GenerateCodeFromType(currentClass, _output.InnerWriter, _options);
      }
    }
  }

  private void GenerateNamespaces(CodeCompileUnit e)
  {
    foreach (CodeNamespace n in e.Namespaces)
      ((ICodeGenerator)this).GenerateCodeFromNamespace(n, _output.InnerWriter, _options);
  }

  private void OutputAttributeArgument(CodeAttributeArgument arg)
  {
    if (!string.IsNullOrEmpty(arg.Name)) {
      OutputIdentifier(arg.Name);
      Output.Write('=');
    }

    ((ICodeGenerator)this).GenerateCodeFromExpression(arg.Value, _output.InnerWriter, _options);
  }

  private void OutputDirection(FieldDirection dir)
  {
    switch (dir) {
      case FieldDirection.In:
        break;

      case FieldDirection.Out:
        Output.Write("out ");
        break;

      case FieldDirection.Ref:
        Output.Write("ref ");
        break;
    }
  }

  private void OutputExpressionList(CodeExpressionCollection expressions)
  {
    OutputExpressionList(expressions, false /*newlineBetweenItems*/);
  }

  private void OutputExpressionList(CodeExpressionCollection expressions, bool newlineBetweenItems)
  {
    var first = true;
    Indent++;

    foreach (CodeExpression current in expressions) {
      if (first) {
        first = false;
      }
      else {
        if (newlineBetweenItems)
          ContinueOnNewLine(",");
        else
          Output.Write(", ");
      }

      ((ICodeGenerator)this).GenerateCodeFromExpression(current, _output.InnerWriter, _options);
    }

    Indent--;
  }

  private void OutputParameters(CodeParameterDeclarationExpressionCollection parameters)
  {
    var first = true;
    var multiline = parameters.Count > ParameterMultilineThreshold;

    if (multiline)
      Indent += 3;

    foreach (CodeParameterDeclarationExpression current in parameters) {
      if (first)
        first = false;
      else
        Output.Write(", ");

      if (multiline)
        ContinueOnNewLine("");

      GenerateExpression(current);
    }

    if (multiline)
      Indent -= 3;
  }

  private void OutputTypeNamePair(CodeTypeReference typeRef, string name)
  {
    OutputType(typeRef);
    Output.Write(' ');
    OutputIdentifier(name);
  }

  private void OutputTypeParameters(CodeTypeParameterCollection typeParameters)
  {
    if (typeParameters.Count == 0)
      return;

    Output.Write('<');
    var first = true;

    for (var i = 0; i < typeParameters.Count; i++) {
      if (first)
        first = false;
      else
        Output.Write(", ");

      if (typeParameters[i].CustomAttributes.Count > 0) {
        GenerateAttributes(typeParameters[i].CustomAttributes, null, true);
        Output.Write(' ');
      }

      Output.Write(typeParameters[i].Name);
    }

    Output.Write('>');
  }

  private void OutputTypeParameterConstraints(CodeTypeParameterCollection typeParameters)
  {
    if (typeParameters.Count == 0)
      return;

    for (var i = 0; i < typeParameters.Count; i++) {
      // generating something like: "where KeyType: IComparable, IEnumerable"
      Indent++;

      var first = true;

      if (typeParameters[i].Constraints.Count > 0)
        foreach (CodeTypeReference typeRef in typeParameters[i].Constraints) {
          if (first) {
            Output.WriteLine();
            Output.Write("where ");
            Output.Write(typeParameters[i].Name);
            Output.Write(" : ");
            first = false;
          }
          else {
            Output.Write(", ");
          }

          OutputType(typeRef);
        }

      if (typeParameters[i].HasConstructorConstraint) {
        if (first) {
          Output.WriteLine();
          Output.Write("where ");
          Output.Write(typeParameters[i].Name);
          Output.Write(" : new()");
        }
        else {
          Output.Write(", new ()");
        }
      }

      Indent--;
    }
  }

  private void OutputTypeAttributes(CodeTypeDeclaration e)
  {
    if ((e.Attributes & MemberAttributes.New) != 0)
      Output.Write("new ");

    var attributes = e.TypeAttributes;

    switch (attributes & TypeAttributes.VisibilityMask) {
      case TypeAttributes.Public:
      case TypeAttributes.NestedPublic:
        Output.Write("public ");
        break;

      case TypeAttributes.NestedPrivate:
        Output.Write("private ");
        break;

      case TypeAttributes.NestedFamily:
        Output.Write("protected ");
        break;

      case TypeAttributes.NotPublic:
      case TypeAttributes.NestedAssembly:
      case TypeAttributes.NestedFamANDAssem:
        Output.Write("internal ");
        break;

      case TypeAttributes.NestedFamORAssem:
        Output.Write("protected internal ");
        break;
    }

    if (e.IsStruct) {
      if (e.IsPartial)
        Output.Write("partial ");

      Output.Write("struct ");
    }
    else if (e.IsEnum) {
      Output.Write("enum ");
    }
    else {
      switch (attributes & TypeAttributes.ClassSemanticsMask) {
        case TypeAttributes.Class:
          if ((attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed)
            Output.Write("sealed ");

          if ((attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
            Output.Write("abstract ");

          if (e.IsPartial)
            Output.Write("partial ");

          Output.Write("class ");

          break;

        case TypeAttributes.Interface:
          if (e.IsPartial)
            Output.Write("partial ");

          Output.Write("interface ");
          break;
      }
    }
  }

  private void GenerateTypeEnd(CodeTypeDeclaration e)
  {
    if (!IsCurrentDelegate) {
      Indent--;
      Output.WriteLine('}');
    }
  }

  private void GenerateNamespaceStart(CodeNamespace e)
  {
    if (!string.IsNullOrEmpty(e.Name)) {
      Output.Write("namespace ");
      var names = e.Name.Split(s_periodArray);
      Debug.Assert(names.Length > 0);
      OutputIdentifier(names[0]);

      for (var i = 1; i < names.Length; i++) {
        Output.Write('.');
        OutputIdentifier(names[i]);
      }

      OutputStartingBrace();
      Indent++;
    }
  }

  private void GenerateCompileUnit(CodeCompileUnit e)
  {
    GenerateCompileUnitStart(e);
    GenerateNamespaces(e);
    GenerateCompileUnitEnd(e);
  }

  private void GenerateCompileUnitStart(CodeCompileUnit e)
  {
    if (e.StartDirectives.Count > 0)
      GenerateDirectives(e.StartDirectives);

    Output.WriteLine("//------------------------------------------------------------------------------");
    Output.WriteLine("// <auto-generated>");
    Output.WriteLine("//     This code was generated by a tool.");
    Output.WriteLine("//");
    Output.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
    Output.WriteLine("//     the code is regenerated.");
    Output.WriteLine("// </auto-generated>");
    Output.WriteLine("//------------------------------------------------------------------------------");
    Output.WriteLine();

    // CSharp needs to put assembly attributes after using statements.
    // Since we need to create an empty namespace even if we don't need it,
    // using will generated after assembly attributes.
    var importList = new SortedSet<string>(StringComparer.Ordinal);

    foreach (CodeNamespace nspace in e.Namespaces) {
      if (string.IsNullOrEmpty(nspace.Name)) {
        // mark the namespace to stop it generating its own import list
        nspace.UserData["GenerateImports"] = false;

        // Collect the unique list of imports
        foreach (CodeNamespaceImport import in nspace.Imports)
          importList.Add(import.Namespace);
      }
    }

    // now output the imports
    foreach (var import in importList) {
      Output.Write("using ");
      OutputIdentifier(import);
      Output.WriteLine(';');
    }

    if (importList.Count > 0)
      Output.WriteLine();

    // in C# the best place to put these is at the top level.
    if (e.AssemblyCustomAttributes.Count > 0) {
      GenerateAttributes(e.AssemblyCustomAttributes, "assembly: ");
      Output.WriteLine();
    }
  }

  private void GenerateCompileUnitEnd(CodeCompileUnit e)
  {
    if (e.EndDirectives.Count > 0)
      GenerateDirectives(e.EndDirectives);
  }

  private void GenerateDirectionExpression(CodeDirectionExpression e)
  {
    OutputDirection(e.Direction);
    GenerateExpression(e.Expression);
  }

  private void GenerateDirectives(CodeDirectiveCollection directives)
  {
    for (var i = 0; i < directives.Count; i++) {
      var directive = directives[i];

      if (directive is CodeChecksumPragma)
        GenerateChecksumPragma((CodeChecksumPragma)directive);
      else if (directive is CodeRegionDirective)
        GenerateCodeRegionDirective((CodeRegionDirective)directive);
    }
  }

  private void GenerateChecksumPragma(CodeChecksumPragma checksumPragma)
  {
    Output.Write("#pragma checksum \"");
    Output.Write(checksumPragma.FileName);
    Output.Write("\" \"");
    Output.Write(checksumPragma.ChecksumAlgorithmId.ToString("B", CultureInfo.InvariantCulture));
    Output.Write("\" \"");

    if (checksumPragma.ChecksumData != null)
      foreach (var b in checksumPragma.ChecksumData)
        Output.Write(b.ToString("X2"));

    Output.WriteLine("\"");
  }

  private void GenerateCodeRegionDirective(CodeRegionDirective regionDirective)
  {
    if (regionDirective.RegionMode == CodeRegionMode.Start) {
      Output.Write("#region ");
      Output.WriteLine(regionDirective.RegionText);
    }
    else if (regionDirective.RegionMode == CodeRegionMode.End) {
      Output.WriteLine("#endregion");
    }
  }

  private void GenerateNamespaceEnd(CodeNamespace e)
  {
    if (!string.IsNullOrEmpty(e.Name)) {
      Indent--;
      Output.WriteLine('}');
    }
  }

  private void GenerateNamespaceImport(CodeNamespaceImport e)
  {
    Output.Write("using ");
    OutputIdentifier(e.Namespace);
    Output.WriteLine(';');
  }

  private void GenerateAttributeDeclarationsStart(CodeAttributeDeclarationCollection attributes)
    => Output.Write('[');

  private void GenerateAttributeDeclarationsEnd(CodeAttributeDeclarationCollection attributes)
    => Output.Write(']');

  private void GenerateAttributes(CodeAttributeDeclarationCollection attributes)
    => GenerateAttributes(attributes, null, false);

  private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix)
    => GenerateAttributes(attributes, prefix, false);

  private void GenerateAttributes(CodeAttributeDeclarationCollection attributes, string prefix, bool inLine)
  {
    if (attributes.Count == 0)
      return;

    var paramArray = false;

    foreach (CodeAttributeDeclaration current in attributes) {
      // we need to convert paramArrayAttribute to params keyword to
      // make csharp compiler happy. In addition, params keyword needs to be after
      // other attributes.

      if (current.Name.Equals("system.paramarrayattribute", StringComparison.OrdinalIgnoreCase)) {
        paramArray = true;
        continue;
      }

      GenerateAttributeDeclarationsStart(attributes);

      if (prefix != null)
        Output.Write(prefix);

      if (current.AttributeType != null)
        Output.Write(GetTypeOutput(current.AttributeType));

      Output.Write('(');

      var firstArg = true;

      foreach (CodeAttributeArgument arg in current.Arguments) {
        if (firstArg)
          firstArg = false;
        else
          Output.Write(", ");

        OutputAttributeArgument(arg);
      }

      Output.Write(')');
      GenerateAttributeDeclarationsEnd(attributes);

      if (inLine)
        Output.Write(' ');
      else
        Output.WriteLine();
    }

    if (paramArray) {
      if (prefix != null)
        Output.Write(prefix);

      Output.Write("params");

      if (inLine)
        Output.Write(' ');
      else
        Output.WriteLine();
    }
  }

  public bool Supports(GeneratorSupport support)
    => (support & LanguageSupport) == support;

  public bool IsValidIdentifier(string value)
  {
    // identifiers must be 1 char or longer
    //
    if (string.IsNullOrEmpty(value))
      return false;

    if (value.Length > 512)
      return false;

    // identifiers cannot be a keyword, unless they are escaped with an '@'
    //
    if (value[0] != '@') {
      if (CSharpHelpers.IsKeyword(value))
        return false;
    }
    else {
      value = value.Substring(1);
    }

    return CodeGenerator.IsValidLanguageIndependentIdentifier(value);
  }

  public void ValidateIdentifier(string value)
  {
    if (!IsValidIdentifier(value))
      throw new ArgumentException($"Identifier {value} is not valid.", nameof(value));
  }

  public string CreateValidIdentifier(string name)
  {
    if (name == null)
      throw new ArgumentNullException(nameof(name));

    if (CSharpHelpers.IsPrefixTwoUnderscore(name))
      name = "_" + name;

    while (CSharpHelpers.IsKeyword(name))
      name = "_" + name;

    return name;
  }

  public string CreateEscapedIdentifier(string name)
  {
    if (name == null)
      throw new ArgumentNullException(nameof(name));

    return CSharpHelpers.CreateEscapedIdentifier(name);
  }

  // returns the type name without any array declaration.
  private string GetBaseTypeOutput(CodeTypeReference typeRef, bool preferBuiltInTypes = true)
  {
    var s = typeRef.BaseType;

    if (preferBuiltInTypes) {
      if (s.Length == 0)
        return "void";

      var lowerCaseString = s.ToLowerInvariant().Trim();

      switch (lowerCaseString) {
        case "system.int16":
          return "short";

        case "system.int16?":
          return "short?";

        case "system.int32":
          return "int";

        case "system.int32?":
          return "int?";

        case "system.int64":
          return "long";

        case "system.int64?":
          return "long?";

        case "system.string":
          return "string";

        case "system.string?":
          return "string?";

        case "system.object":
          return "object";

        case "system.object?":
          return "object?";

        case "system.boolean":
          return "bool";

        case "system.boolean?":
          return "bool?";

        case "system.void":
          return "void";

        case "system.void?":
          return "void?";

        case "system.char":
          return "char";

        case "system.char?":
          return "char?";

        case "system.byte":
          return "byte";

        case "system.byte?":
          return "byte?";

        case "system.uint16":
          return "ushort";

        case "system.uint16?":
          return "ushort?";

        case "system.uint32":
          return "uint";

        case "system.uint32?":
          return "uint?";

        case "system.uint64":
          return "ulong";

        case "system.uint64?":
          return "ulong?";

        case "system.sbyte":
          return "sbyte";

        case "system.sbyte?":
          return "sbyte?";

        case "system.single":
          return "float";

        case "system.single?":
          return "float?";

        case "system.double":
          return "double";

        case "system.double?":
          return "double?";

        case "system.decimal":
          return "decimal";

        case "system.decimal?":
          return "decimal?";
      }
    }

    // replace + with . for nested classes.
    //
    var sb = new StringBuilder(s.Length + 10);

    if ((typeRef.Options & CodeTypeReferenceOptions.GlobalReference) != 0)
      sb.Append("global::");

    var baseType = typeRef.BaseType;

    var lastIndex = 0;
    var currentTypeArgStart = 0;

    for (var i = 0; i < baseType.Length; i++) {
      switch (baseType[i]) {
        case '+':
        case '.':
          sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex, i - lastIndex)));
          sb.Append('.');
          i++;
          lastIndex = i;
          break;

        case '`':
          sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex, i - lastIndex)));
          i++; // skip the '
          var numTypeArgs = 0;

          while (i < baseType.Length && baseType[i] >= '0' && baseType[i] <= '9') {
            numTypeArgs = numTypeArgs * 10 + (baseType[i] - '0');
            i++;
          }

          GetTypeArgumentsOutput(typeRef.TypeArguments, currentTypeArgStart, numTypeArgs, sb);
          currentTypeArgStart += numTypeArgs;

          // Arity can be in the middle of a nested type name, so we might have a . or + after it.
          // Skip it if so.
          if (i < baseType.Length && (baseType[i] == '+' || baseType[i] == '.')) {
            sb.Append('.');
            i++;
          }

          lastIndex = i;
          break;
      }
    }

    if (lastIndex < baseType.Length)
      sb.Append(CreateEscapedIdentifier(baseType.Substring(lastIndex)));

    return sb.ToString();
  }

  private string GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments)
  {
    var sb = new StringBuilder(128);
    GetTypeArgumentsOutput(typeArguments, 0, typeArguments.Count, sb);
    return sb.ToString();
  }

  private void GetTypeArgumentsOutput(CodeTypeReferenceCollection typeArguments, int start, int length, StringBuilder sb)
  {
    sb.Append('<');
    var first = true;

    for (var i = start; i < start + length; i++) {
      if (first)
        first = false;
      else
        sb.Append(", ");

      // it's possible that we call GetTypeArgumentsOutput with an empty typeArguments collection.  This is the case
      // for open types, so we want to just output the brackets and commas.
      if (i < typeArguments.Count)
        sb.Append(GetTypeOutput(typeArguments[i]));
    }

    sb.Append('>');
  }

  public string GetTypeOutput(CodeTypeReference typeRef)
  {
    var s = string.Empty;

    var baseTypeRef = typeRef;

    while (baseTypeRef.ArrayElementType != null)
      baseTypeRef = baseTypeRef.ArrayElementType;

    s += GetBaseTypeOutput(baseTypeRef);

    while (typeRef is { ArrayRank: > 0 }) {
      var results = new char[typeRef.ArrayRank + 1];
      results[0] = '[';
      results[typeRef.ArrayRank] = ']';

      for (var i = 1; i < typeRef.ArrayRank; i++)
        results[i] = ',';

      s += new string(results);
      typeRef = typeRef.ArrayElementType;
    }

    return s;
  }

  private void OutputStartingBrace()
  {
    if (_options.BracingStyle == "C") {
      Output.WriteLine();
      Output.WriteLine('{');
    }
    else {
      Output.WriteLine(" {");
    }
  }

  //private static string JoinStringArray(string[] sa, string separator)
  //{
  //  if (sa is not { Length: not 0 })
  //    return string.Empty;

  //  if (sa.Length == 1)
  //    return "\"" + sa[0] + "\"";

  //  var sb = new StringBuilder();

  //  for (var i = 0; i < sa.Length - 1; i++) {
  //    sb.Append('\"');
  //    sb.Append(sa[i]);
  //    sb.Append('\"');
  //    sb.Append(separator);
  //  }

  //  sb.Append('\"');
  //  sb.Append(sa[sa.Length - 1]);
  //  sb.Append('\"');

  //  return sb.ToString();
  //}

  void ICodeGenerator.GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
  {
    var setLocal = false;

    if (_output != null && w != _output.InnerWriter)
      throw new InvalidOperationException(CodeGenOutputWriter);

    if (_output == null) {
      setLocal = true;
      _options = o ?? new CodeGeneratorOptions();
      _output = new ExposedTabStringIndentedTextWriter(w, _options.IndentString);
    }

    try {
      GenerateType(e);
    }
    finally {
      if (setLocal) {
        _output = null;
        _options = null;
      }
    }
  }

  void ICodeGenerator.GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
  {
    var setLocal = false;

    if (_output != null && w != _output.InnerWriter)
      throw new InvalidOperationException(CodeGenOutputWriter);

    if (_output == null) {
      setLocal = true;
      _options = o ?? new CodeGeneratorOptions();
      _output = new ExposedTabStringIndentedTextWriter(w, _options.IndentString);
    }

    try {
      GenerateExpression(e);
    }
    finally {
      if (setLocal) {
        _output = null;
        _options = null;
      }
    }
  }

  void ICodeGenerator.GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
  {
    var setLocal = false;

    if (_output != null && w != _output.InnerWriter)
      throw new InvalidOperationException(CodeGenOutputWriter);

    if (_output == null) {
      setLocal = true;
      _options = o ?? new CodeGeneratorOptions();
      _output = new ExposedTabStringIndentedTextWriter(w, _options.IndentString);
    }

    try {
      if (e is CodeSnippetCompileUnit)
        GenerateSnippetCompileUnit((CodeSnippetCompileUnit)e);
      else
        GenerateCompileUnit(e);
    }
    finally {
      if (setLocal) {
        _output = null;
        _options = null;
      }
    }
  }

  void ICodeGenerator.GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
  {
    var setLocal = false;

    if (_output != null && w != _output.InnerWriter)
      throw new InvalidOperationException(CodeGenOutputWriter);

    if (_output == null) {
      setLocal = true;
      _options = o ?? new CodeGeneratorOptions();
      _output = new ExposedTabStringIndentedTextWriter(w, _options.IndentString);
    }

    try {
      GenerateNamespace(e);
    }
    finally {
      if (setLocal) {
        _output = null;
        _options = null;
      }
    }
  }

  void ICodeGenerator.GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
  {
    var setLocal = false;

    if (_output != null && w != _output.InnerWriter)
      throw new InvalidOperationException(CodeGenOutputWriter);

    if (_output == null) {
      setLocal = true;
      _options = o ?? new CodeGeneratorOptions();
      _output = new ExposedTabStringIndentedTextWriter(w, _options.IndentString);
    }

    try {
      GenerateStatement(e);
    }
    finally {
      if (setLocal) {
        _output = null;
        _options = null;
      }
    }
  }
}
