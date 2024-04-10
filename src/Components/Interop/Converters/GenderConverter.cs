using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Converters;

internal sealed class GenderConverter : JsonConverter<Gender>
{
  private const string Male = "male";

  private const string Female = "female";

  public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var genderStr = reader.GetString();
    if (genderStr != Male && genderStr != Female)
    {
      throw new NotSupportedException($"Gender must be either \"{Male}\" or \"{Female}\".");
    }
    
    return genderStr == Male ? Gender.Male : Gender.Female;
  }

  public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
  {
    var genderStr = value switch
    {
      Gender.Male => Male,
      Gender.Female => Female,
      _ => throw new NotSupportedException($"Gender {value} not supported. This should never happen.")
    };

    writer.WriteStringValue(genderStr);
  }
}
