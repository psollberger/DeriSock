namespace DeriSock.Converter;

using System;

using Newtonsoft.Json.Linq;

internal class TimestampJsonConverter : IJsonConverter<DateTime>
{
  public DateTime Convert(JToken? value)
  {
    if (value is null)
      return default(DateTime);

    return value.ToObject<long>().ToDateTimeFromUnixTimeMilliseconds();
  }
}
