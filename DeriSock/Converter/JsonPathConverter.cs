namespace DeriSock.Converter
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Newtonsoft.Json.Serialization;

  public class JsonPathConverter : JsonConverter
  {
    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jo = JObject.Load(reader);
      var targetObj = Activator.CreateInstance(objectType);

      foreach (var prop in objectType.GetProperties().Where(p => p.CanRead && p.CanWrite))
      {
        var att = prop.GetCustomAttributes(true)
          .OfType<JsonPropertyAttribute>()
          .FirstOrDefault();

        var jsonPath = att != null ? att.PropertyName : prop.Name;

        if (serializer.ContractResolver is DefaultContractResolver)
        {
          var resolver = (DefaultContractResolver)serializer.ContractResolver;
          jsonPath = resolver.GetResolvedPropertyName(jsonPath);
        }

        if (!Regex.IsMatch(jsonPath, @"^[a-zA-Z0-9_.-]+$"))
        {
          throw new InvalidOperationException(
            "JProperties of JsonPathConverter can have only letters, numbers, underscores, dashes and dots but name was '" +
            jsonPath + "'."); // Array operations not permitted
        }

        var token = jo.SelectToken(jsonPath);
        if (token != null && token.Type != JTokenType.Null)
        {
          var value = token.ToObject(prop.PropertyType, serializer);
          prop.SetValue(targetObj, value, null);
        }
      }

      return targetObj;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
      // CanConvert is not called when [JsonConverter] attribute is used
      return objectType.GetCustomAttributes(true).OfType<JsonPathConverter>().Any();
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var properties = value.GetType().GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite);
      var main = new JObject();
      foreach (var prop in properties)
      {
        var att = prop.GetCustomAttributes(true)
          .OfType<JsonPropertyAttribute>()
          .FirstOrDefault();

        var jsonPath = att != null ? att.PropertyName : prop.Name;
        if (serializer.ContractResolver is DefaultContractResolver)
        {
          var resolver = (DefaultContractResolver)serializer.ContractResolver;
          jsonPath = resolver.GetResolvedPropertyName(jsonPath);
        }

        var lastLevel = main;
        if (jsonPath.Contains('.'))
        {
          var nesting = jsonPath.Split('.');

          for (var i = 0; i < nesting.Length; i++)
          {
            if (i == nesting.Length - 1)
            {
              var propValue = prop.GetValue(value);
              lastLevel[nesting[i]] = new JValue(propValue);
            }
            else
            {
              if (lastLevel[nesting[i]] == null)
              {
                lastLevel[nesting[i]] = new JObject();
              }

              lastLevel = (JObject)lastLevel[nesting[i]];
            }
          }
        }
      }

      serializer.Serialize(writer, main);
    }
  }
}
