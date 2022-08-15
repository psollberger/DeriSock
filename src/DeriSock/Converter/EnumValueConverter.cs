namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;

/// <summary>
///   Converts <see cref="EnumValue" /> classes into strings in requests
/// </summary>
public class EnumValueConverter : JsonConverter<EnumValue>
{
  /// <inheritdoc />
  public override bool CanRead { get; } = false;

  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, EnumValue? value, JsonSerializer serializer)
  {
    Debug.Assert(value is not null, nameof(value) + " != null");
    writer.WriteValue(value!.ToString());
  }

  /// <inheritdoc />
  public override EnumValue ReadJson(JsonReader reader, Type objectType, EnumValue? existingValue, bool hasExistingValue, JsonSerializer serializer)
    => throw new NotSupportedException();
}
