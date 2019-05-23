namespace DeriSock.Converter
{
  using Newtonsoft.Json.Linq;

  public class ObjectJsonConverter<T> : JsonConverter<T> where T : class
  {
    public T Convert(JToken value)
    {
      return value.ToObject<T>();
    }
  }
}
