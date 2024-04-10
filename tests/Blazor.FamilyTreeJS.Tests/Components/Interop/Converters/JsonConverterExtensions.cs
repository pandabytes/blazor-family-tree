using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Tests.Components.Interop.Converters;

/// <summary>
/// Taken from: https://khalidabuhakmeh.com/systemtextjson-jsonconverter-test-helpers
/// </summary>
public static class JsonConverterTestExtensions
{
  public static TResult? Read<TResult>(
    this JsonConverter<TResult> converter, 
    string token,
    JsonSerializerOptions? options = null)
  {
    options ??= JsonSerializerOptions.Default;
    var bytes = Encoding.UTF8.GetBytes(token);
    var reader = new Utf8JsonReader(bytes);
    
    // Advance to token
    reader.Read();
    return converter.Read(ref reader, typeof(TResult), options);
  }

  public static string Write<T>(
    this JsonConverter<T> converter, 
    T value,
    JsonSerializerOptions? options = null)
  {
    options ??= JsonSerializerOptions.Default;
    using var memoryStream = new MemoryStream();
    using var writer = new Utf8JsonWriter(memoryStream);

    converter.Write(writer, value, options);
    writer.Flush();
    return Encoding.UTF8.GetString(memoryStream.ToArray());
  }
}
