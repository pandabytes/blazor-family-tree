using System.Text.Json;
using System.Text.Json.Serialization;
namespace Blazor.FamilyTreeJS.Extensions;

public static class DynamicObjExtensions
{
  private static readonly JsonSerializerOptions JsonSerializerOpts = new()
  {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  /// <summary>
  /// Convert <paramref name="value"/> to the proper JSON casing
  /// format for Javascript interopability.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string ToJsonCasing(this string value)
    => JsonSerializerOpts.PropertyNamingPolicy?.ConvertName(value) ?? value;
}
