namespace DeriSock.Converter
{
  using Newtonsoft.Json.Linq;

  public class ObjectJsonConverter<T> : IJsonConverter<T>
  {
    public T Convert(JToken value)
    {
      return value.ToObject<T>();
    }
  }
}
