namespace DeriSock.DevTools.ApiDoc.Model;

public interface IApiDocPropertyNode
{
  IApiDocPropertyNode? Parent { get; }
  ApiDocFunctionType FunctionType { get; set; }
  string Name { get; set; }
  ApiDocPropertyCollection? Properties { get; set; }

  void UpdateRelations(IApiDocPropertyNode? parent);
}
