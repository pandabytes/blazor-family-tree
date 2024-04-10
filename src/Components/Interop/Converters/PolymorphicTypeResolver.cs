using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Blazor.FamilyTreeJS.Interop;

internal class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
  private readonly IDictionary<Type, JsonPolymorphismOptions> _typeJsonPolyOpts;

  public PolymorphicTypeResolver()
  {  
    _typeJsonPolyOpts = new DefaultDictionary<Type, JsonPolymorphismOptions>(
      () => new()
      {
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
      }
    );
  }

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
        throw new ArgumentException($"Type {type.FullName} doesn't inhereit class {baseType.FullName}.");
      }

      _typeJsonPolyOpts[baseType].DerivedTypes.Add(new JsonDerivedType(type));
    }
  }
}
