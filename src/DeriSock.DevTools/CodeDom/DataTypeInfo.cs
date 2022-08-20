namespace DeriSock.DevTools.CodeDom;

public class DataTypeInfo
{
  public string TypeName;
  public bool IsArray;
  public bool IsNullable;
  public bool IsImported;

  public DataTypeInfo(string typeName, bool isArray, bool isNullable)
  {
    TypeName = typeName;
    IsArray = isArray;
    IsNullable = isNullable;
    IsImported = false;
  }

  public string ToFullTypeName()
  {
    var fullTypeName = TypeName;

    if (IsArray)
      fullTypeName = $"{fullTypeName}[]";

    if (IsNullable)
      fullTypeName = $"{fullTypeName}?";

    return fullTypeName;
  }
}
