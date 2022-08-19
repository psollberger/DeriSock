namespace DeriSock.Converter;

using Newtonsoft.Json.Linq;

/// <inheritdoc />
public class ObjectJsonConverter<T> : IJsonConverter<T>
{
  /// <inheritdoc />
  public T? Convert(JToken? value)
  {
    if (value is null)
      return default(T);

    return value.ToObject<T>();
  }
}
