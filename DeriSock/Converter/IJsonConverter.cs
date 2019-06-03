namespace DeriSock.Converter
{
  using Newtonsoft.Json.Linq;

  public interface IJsonConverter<out T>
  {
    T Convert(JToken value);
  }
}
