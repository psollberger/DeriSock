namespace DeriSock.Converter;

using System;
using Newtonsoft.Json.Linq;

internal class TimestampJsonConverter : IJsonConverter<DateTime>
{
  public DateTime Convert(JToken value)
  {
    return value.ToObject<long>().AsDateTimeFromMilliseconds();
  }
}
