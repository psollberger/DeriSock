namespace DeriSock.Converter;

using Newtonsoft.Json.Linq;

/// <summary>
///   Allows an object to control deserialization of a json value
/// </summary>
/// <typeparam name="T">The target .Net type</typeparam>
public interface IJsonConverter<out T>
{
  /// <summary>
  ///   Converts the given <see cref="JToken" /> to the target type
  /// </summary>
  /// <param name="value">The json value that needs to be converted</param>
  /// <returns>The result from the conversion as the target .Net type</returns>
  T Convert(JToken value);
}
