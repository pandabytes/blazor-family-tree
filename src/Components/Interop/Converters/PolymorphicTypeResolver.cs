using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Blazor.FamilyTreeJS.Components.Interop.Converters;

internal class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
  private readonly IDictionary<Type, JsonPolymorphismOptions> _typeJsonPolyOpts;

  public PolymorphicTypeResolver()
    => _typeJsonPolyOpts = new Dictionary<Type, JsonPolymorphismOptions>();

  /// <inheritdoc />
  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
  {
    var jsonTypeInfo = base.GetTypeInfo(type, options);

    var found = _typeJsonPolyOpts.TryGetValue(jsonTypeInfo.Type, out var jsonPolyOpts);
    if (found && jsonPolyOpts!.DerivedTypes.Any())
    {
      jsonTypeInfo.PolymorphismOptions = jsonPolyOpts;
    }

    return jsonTypeInfo;
  }

  /// <summary>
  /// Add custom derived classes that inhereit from
  /// <typeparamref name="TBase"/> to be serialized.
  /// </summary>
  /// <param name="types">
  /// Types that inhereit <typeparamref name="TBase"/>.
  /// </param>
  /// <exception cref="ArgumentException">
  /// Thrown when a type does not inhereit <typeparamref name="TBase"/>.
  /// </exception>
  public void AddDerivedTypes<TBase>(params Type[] types) where TBase : class
  {
    var baseType = typeof(TBase);
    foreach (var type in types)
    {
      if (!baseType.IsAssignableFrom(type))
      {
        throw new ArgumentException($"Type {type.FullName} doesn't inhereit type {baseType.FullName}.");
      }

      var found = _typeJsonPolyOpts.TryGetValue(baseType, out var jsonPolyOpts);
      if (!found)
      {
        jsonPolyOpts = new()
        {
          IgnoreUnrecognizedTypeDiscriminators = true,
          UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };

        _typeJsonPolyOpts.Add(baseType, jsonPolyOpts);
      }

      jsonPolyOpts!.DerivedTypes.Add(new JsonDerivedType(type));
    }
  }
}
