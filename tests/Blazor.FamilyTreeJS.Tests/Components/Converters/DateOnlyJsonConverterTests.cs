using System.Text.Json;

namespace Blazor.FamilyTreeJS.Tests.Components.Converters;

public class DateOnlyJsonConverterTests
{
  private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

  private readonly DateOnlyJsonConverter _dateOnlyConverter;

  public DateOnlyJsonConverterTests()
    => _dateOnlyConverter = new();

  [Fact]
  public void Read_ValidDateFormat_ParsedSuccessfully()
  {
    // Act
    var dateOnly = _dateOnlyConverter.Read($"\"{Today:yyyy-MM-dd}\"");

    // Assert
    Assert.Equal(Today, dateOnly);
  }

  [InlineData("\"\"")]
  [InlineData("\"   \"")]
  [InlineData("null")]
  [Theory]
  public void Read_EmptyJson_ReturnsNull(string json)
  {
    // Act
    var dateOnly = _dateOnlyConverter.Read(json);

    // Assert
    Assert.Null(dateOnly);
  }

  [Fact]
  public void Read_InvalidDateFormat_ThrowsException()
  {
    Assert.Throws<JsonException>(() => _dateOnlyConverter.Read($"\"{Today:yyyy/MM/dd}\""));
  }

  [Fact]
  public void Read_NotStringValue_ThrowsException()
  {
    Assert.Throws<JsonException>(() => _dateOnlyConverter.Read("0"));
  }

  [Fact]
  public void Write_DateOnlyObject_ReturnsValidJsonString()
  {
    // Arrange
    var expectedJson = $"\"{Today:yyyy-MM-dd}\"";

    // Act
    var actualJson = _dateOnlyConverter.Write(Today);

    // Assert
    Assert.Equal(expectedJson, actualJson);
  }

  [Fact]
  public void Write_NullDateOnly_ReturnsNullJsonString()
  {
    // Act
    var actualJson = _dateOnlyConverter.Write(null);

    // Assert
    Assert.Equal("null", actualJson);
  }
}
