namespace DeriSock.DevTools.ApiDoc.Model;

public interface IApiDocPropertyNode
{
  IApiDocPropertyNode? Parent { get; }
  public string Name { get; set; }
  ApiDocPropertyCollection? Properties { get; set; }

  void UpdateParent(IApiDocPropertyNode? parent);
}
