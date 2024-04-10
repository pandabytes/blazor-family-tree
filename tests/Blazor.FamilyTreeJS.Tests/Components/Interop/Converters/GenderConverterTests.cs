using Blazor.FamilyTreeJS.Components.Interop.Options;

namespace Blazor.FamilyTreeJS.Tests.Components.Interop.Converters;

public class GenderConverterTests
{
  private readonly GenderConverter _genderConverter;

  public GenderConverterTests()
    => _genderConverter = new();

  [InlineData("\"\"")]
  [InlineData("\"    \"")]
  [InlineData("null")]
  [InlineData("\"mALE\"")]
  [InlineData("\"FemALE\"")]
  [Theory]
  public void Read_InvalidString_ThrowsException(string json)
  {
    Assert.Throws<NotSupportedException>(() => _genderConverter.Read(json));
  }

  [InlineData("\"male\"", Gender.Male)]
  [InlineData("\"female\"", Gender.Female)]
  [Theory]
  public void Read_ValidString_ThrowsException(string json, Gender expectedGender)
  {
    var actualGender = _genderConverter.Read(json);
    Assert.Equal(expectedGender, actualGender);
  }

  [InlineData("\"male\"", Gender.Male)]
  [InlineData("\"female\"", Gender.Female)]
  [Theory]
  public void Write(string expectedJson, Gender gender)
  {
    var actualJson = _genderConverter.Write(gender);
    Assert.Equal(expectedJson, actualJson);
  }
}
