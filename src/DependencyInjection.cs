using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.Core;

namespace Blazor.FamilyTreeJS;

/// <summary>
/// Provide dependency injection methods to
/// setup this library.
/// </summary>
public static class DependencyInjection
{
  /// <summary>
  /// Register FamilyTreeJS dependencies.
  /// </summary>
  public static IServiceCollection AddBlazorFamilyJS(this IServiceCollection services)
  {
    return services
      .AddScoped(typeof(FamilyTreeInteropJsModule<>))
      .AddTransient<ObjectTraversal>();
  }

  /// <summary>
  /// Configure the <see cref="IJSRuntime"/>'s JSON options
  /// to not serialize "null" values. This will affect globally.
  /// </summary>
  public static WebAssemblyHost ConfigureIJSRuntimeJsonOptionsForBlazorFamilyTree(this WebAssemblyHost webHost)
  {
    return webHost.ConfigureIJSRuntimeJsonOptions(options =>
    {
      options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      options.TypeInfoResolver = new PolymorphicTypeResolver();
    });
  }

  /// <summary>
  /// Register classes that derive from <typeparamref name="TBase"/>
  /// into the JSON serialization/deserialization process. This
  /// enables the derived classes to be serialized/deserialized
  /// correctly with their custom properties. Make sure you call
  /// <see cref="ConfigureIJSRuntimeJsonOptionsForBlazorFamilyTree(WebAssemblyHost)"/>
  /// first before calling this method.
  /// </summary>
  /// <param name="webHost"></param>
  /// <param name="types">Types that derive from <typeparamref name="TBase"/>.</param>
  /// <exception cref="InvalidOperationException">
  /// Thrown when <paramref name="types"/> do not derive
  /// from <typeparamref name="TBase"/>.
  /// </exception>
  public static WebAssemblyHost UseDerivedTypes<TBase>(this WebAssemblyHost webHost, params Type[] types)
    where TBase : class
  {
    return webHost.ConfigureIJSRuntimeJsonOptions(options =>
    {
      if (options.TypeInfoResolver is not PolymorphicTypeResolver polymorhphicResolver)
      {
        throw new InvalidOperationException($"Expect {nameof(JsonSerializerOptions.TypeInfoResolver)} to be " +
                                            $"of type {nameof(PolymorphicTypeResolver)}. Please call " +
                                            $"method {nameof(ConfigureIJSRuntimeJsonOptionsForBlazorFamilyTree)} first.");
      }

      polymorhphicResolver.AddDerivedTypes<TBase>(types);
    });
  }
}