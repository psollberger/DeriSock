namespace DeriSock.Model;

using System;

using DeriSock.Converter;

using Newtonsoft.Json;

/// <summary>
///   Base class for API value enumerations
/// </summary>
[JsonConverter(typeof(EnumValueConverter))]
public abstract class EnumValue : IEquatable<EnumValue>, IEquatable<string>
{
  private readonly string _value;

  /// <summary>
  ///   Initializes a new instance of the <see cref="EnumValue" /> class.
  /// </summary>
  /// <param name="value">The value for the enum entry</param>
  protected EnumValue(string value)
  {
    _value = value;
  }

  /// <summary>
  ///   Returns the value of the enum
  /// </summary>
  public override string ToString()
    => _value;

  /// <inheritdoc />
  public bool Equals(EnumValue? other)
  {
    if (ReferenceEquals(null, other))
      return false;

    if (ReferenceEquals(this, other))
      return true;

    return _value == other._value;
  }

  /// <inheritdoc />
  public bool Equals(string? other)
  {
    if (ReferenceEquals(null, other))
      return false;

    return _value == other;
  }

  /// <inheritdoc />
  public override bool Equals(object? obj)
    => ReferenceEquals(this, obj) || (obj is EnumValue other && Equals(other));

  /// <inheritdoc />
  public override int GetHashCode()
    => _value.GetHashCode();

#pragma warning disable CS1591
  public static bool operator ==(EnumValue? left, EnumValue? right)
    => Equals(left, right);

  public static bool operator !=(EnumValue? left, EnumValue? right)
    => !Equals(left, right);

  public static bool operator ==(EnumValue? left, string? right)
  {
    switch (left) {
      case null when ReferenceEquals(null, right):
        return true;

      case null when !ReferenceEquals(null, right):
        return false;
    }

    if (!ReferenceEquals(null, left) && ReferenceEquals(null, right))
      return false;

    return left!.Equals(right);
  }

  public static bool operator !=(EnumValue? left, string? right)
  {
    switch (left) {
      case null when ReferenceEquals(null, right):
        return false;

      case null when !ReferenceEquals(null, right):
        return true;
    }

    if (!ReferenceEquals(null, left) && ReferenceEquals(null, right))
      return true;

    return !left!.Equals(right);
  }

  public static bool operator ==(string? left, EnumValue? right)
    => right == left;

  public static bool operator !=(string? left, EnumValue? right)
    => right != left;
#pragma warning restore CS1591
}
