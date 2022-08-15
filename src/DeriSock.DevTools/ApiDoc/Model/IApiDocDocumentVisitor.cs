namespace DeriSock.DevTools.ApiDoc.Model;

public interface IApiDocDocumentVisitor
{
  void VisitDocument(ApiDocDocument document);
  void VisitFunction(ApiDocFunction function);
  void VisitProperty(ApiDocProperty property);
}
