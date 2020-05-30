namespace DeriSock.Converter
{
  using System;
  using DeriSock.Extensions;
  using Newtonsoft.Json.Linq;

  public class TimestampJsonConverter : IJsonConverter<DateTime>
  {
    public DateTime Convert(JToken value)
    {
      return value.ToObject<long>().AsDateTimeFromMilliseconds();
    }
  }
}
