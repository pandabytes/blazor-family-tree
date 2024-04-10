using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Tests.Components.Interop.Converters;

public class PolymorphicTypeResolverTests
{
  private class Animal {}

  private class Tiger : Animal {}

  private class Bear : Animal {}

  public static TheoryData<IList<Type>> TestData =>
    new()
    {
      { new List<Type> { typeof(Tiger) } },
      { new List<Type> { typeof(Tiger), typeof(Bear) } },
    };

  private readonly PolymorphicTypeResolver _resolver;

  public PolymorphicTypeResolverTests() => _resolver = new();

  [Fact]
  public void AddDerivedTypes_TypeDoesNotInheritBaseType_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() => _resolver.AddDerivedTypes<Animal>(typeof(string))); 
  }

  [MemberData(nameof(TestData))]
  [Theory]
  public void AddDerivedTypes_TypesInheritBaseType_PolymorphismOptionIsUpdated(IList<Type> derivedTypes)
  {
    // Arrange
    var animalType = typeof(Animal);

    // Act
    _resolver.AddDerivedTypes<Animal>(derivedTypes.ToArray());

    // Assert
    var jsonTypeInfo = _resolver.GetTypeInfo(animalType, new());
    var polyOptions = jsonTypeInfo.PolymorphismOptions;

    Assert.Equal(animalType, jsonTypeInfo.Type);
    Assert.NotNull(polyOptions);
    Assert.True(polyOptions.IgnoreUnrecognizedTypeDiscriminators);
    Assert.Equal(JsonUnknownDerivedTypeHandling.FailSerialization,
                 polyOptions.UnknownDerivedTypeHandling);

    var actualDerivedTypes = polyOptions.DerivedTypes
      .Select(jsonDerivedType => jsonDerivedType.DerivedType)
      .ToHashSet();

    Assert.Equal(derivedTypes.ToHashSet(), actualDerivedTypes);
  }
}
