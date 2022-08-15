namespace DeriSock.Converter;

using Newtonsoft.Json.Linq;

/// <inheritdoc />
public class ObjectJsonConverter<T> : IJsonConverter<T>
{
  /// <inheritdoc />
  public T Convert(JToken value)
  {
    return value.ToObject<T>();
  }
}
