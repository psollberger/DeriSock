namespace DeriSock.DevTools.CodeDom;

public struct DataTypeInfo
{
  public string TypeName;
  public bool IsArray;
  public bool IsNullable;

  public DataTypeInfo(string typeName, bool isArray, bool isNullable)
  {
    TypeName = typeName;
    IsArray = isArray;
    IsNullable = isNullable;
  }
}
