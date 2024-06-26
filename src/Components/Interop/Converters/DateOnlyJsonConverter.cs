using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Converters;

internal sealed class DateOnlyJsonConverter : JsonConverter<DateOnly?>
{
  public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    try
    {
      var dateTimeStr = reader.GetString();
      if (string.IsNullOrWhiteSpace(dateTimeStr))
      {
        return null;
      }

      return DateOnly.FromDateTime(reader.GetDateTime());
    }
    catch (Exception ex) when 
      (ex is InvalidOperationException || ex is FormatException)
    {
      throw new JsonException("Fail to parse date.", ex);
    }
  }

  public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
  {
    if (value is null)
    {
      writer.WriteNullValue();
      return;
    }

    var isoDate = ((DateOnly)value).ToString("O");
    writer.WriteStringValue(isoDate);
  }
}
