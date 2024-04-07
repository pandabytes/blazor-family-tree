using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Blazor.FamilyTreeJS.Interop;

internal class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
  /// <summary>
  /// Contain type information of classes that
  /// derive from <see cref="NodeMenu"/>.
  /// </summary>
  private readonly JsonPolymorphismOptions _nodeMenuJsonPolyOptions;

  public PolymorphicTypeResolver()
  {
    _nodeMenuJsonPolyOptions = new()
    {
      IgnoreUnrecognizedTypeDiscriminators = true,
      UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
    };
  }

  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
  {
    JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);
    if (jsonTypeInfo.Type == typeof(NodeMenu) && _nodeMenuJsonPolyOptions.DerivedTypes.Any())
    {
      jsonTypeInfo.PolymorphismOptions = _nodeMenuJsonPolyOptions;
    }
    return jsonTypeInfo;
  }

  /// <summary>
  /// Add custom derived classes that inhereit from
  /// <see cref="NodeMenu"/> to be serialized.
  /// </summary>
  /// <param name="types">ypes that inhereit <see cref="NodeMenu"/></param>
  /// <exception cref="ArgumentException">
  /// Thrown when a type does not inhereit <see cref="NodeMenu"/>
  /// </exception>
  public void AddDerivedNodeMenuTypes(params Type[] types)
  {
    var nodeMenuType = typeof(NodeMenu);
    foreach (var type in types)
    {
      if (!nodeMenuType.IsAssignableFrom(type))
      {
        throw new ArgumentException($"Type {type.FullName} doesn't inhereit class {nodeMenuType.FullName}.");
      }

      _nodeMenuJsonPolyOptions.DerivedTypes.Add(new JsonDerivedType(type));
    }
  }
}
