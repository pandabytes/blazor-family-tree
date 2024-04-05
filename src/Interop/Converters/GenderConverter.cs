using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Interop.Converters;

internal sealed class GenderConverter : JsonConverter<Gender>
{
  public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var genderStr = reader.GetString();
    if (genderStr != GenderString.Male && genderStr != GenderString.Female)
    {
      throw new NotSupportedException($"Gender must be either \"{GenderString.Male}\" or \"{GenderString.Female}\".");
    }
    
    return genderStr == GenderString.Male ? Gender.Male : Gender.Female;
  }

  public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
  {
    var genderStr = value switch
    {
      Gender.Male => GenderString.Male,
      Gender.Female => GenderString.Female,
      _ => throw new NotSupportedException($"Gender {value} not supported. This should never happen.")
    };

    writer.WriteStringValue(genderStr);
  }
}
