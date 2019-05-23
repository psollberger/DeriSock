namespace DeriSock.Converter
{
  using Newtonsoft.Json.Linq;

  public interface JsonConverter<T>
  {
    T Convert(JToken value);
  }
}
