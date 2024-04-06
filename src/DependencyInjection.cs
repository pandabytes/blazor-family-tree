using Microsoft.Extensions.DependencyInjection;
using Blazor.FamilyTreeJS.Interop;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS;

public static class DependencyInjection
{
  /// <summary>
  /// Register FamilyTreeJS dependencies.
  /// </summary>
  public static IServiceCollection AddBlazorFamilyJS(this IServiceCollection services)
  {
    return services
      .AddScoped<FamilyTreeInteropJsModule>()
      .AddTransient<ObjectTraversal>();
  }

  /// <summary>
  /// Configure the <see cref="IJSRuntime"/>'s JSON options
  /// to not serialize "null" values. This will affect globally.
  /// </summary>
  public static IServiceProvider ConfigureIJSRuntimeJsonOptions(this IServiceProvider services)
  {
    var jsRuntime = services.GetRequiredService<IJSRuntime>();
    var options = GetJsonSerializerOptions(jsRuntime);

    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.TypeInfoResolver = new PolymorphicTypeResolver();
    return services;
  }

  /// <summary>
  /// Register classes that derive from <see cref="NodeMenu"/>
  /// into the JSON serialization/deserialization process. This
  /// enables the derived classes to be serialized/deserialized
  /// correctly with their custom properties.
  /// </summary>
  /// <param name="types">Types that derive from <see cref="NodeMenu"/></param>
  /// <exception cref="InvalidOperationException">
  /// Thrown when <paramref name="types"/> do not derive
  /// from <see cref="NodeMenu"/>.
  /// </exception>
  public static IServiceProvider UseNodeMenuDerivedTypes(this IServiceProvider services, params Type[] types)
  {
    var jsRuntime = services.GetRequiredService<IJSRuntime>();
    var options = GetJsonSerializerOptions(jsRuntime);
    if (options.TypeInfoResolver is not PolymorphicTypeResolver polymorhphicResolver)
    {
      throw new InvalidOperationException($"Expect {nameof(JsonSerializerOptions.TypeInfoResolver)} to be " +
                                          $"of type {nameof(PolymorphicTypeResolver)}.");
    }

    polymorhphicResolver.AddDerivedNodeMenuTypes(types);
    return services;
  }

  /// <summary>
  /// See https://github.com/dotnet/aspnetcore/issues/12685#issuecomment-603050776
  /// </summary>
  /// <param name="jsRuntime"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  private static JsonSerializerOptions GetJsonSerializerOptions(IJSRuntime jsRuntime)
  {
    var property = typeof(JSRuntime).GetProperty("JsonSerializerOptions", BindingFlags.NonPublic | BindingFlags.Instance);
    if (property?.GetValue(jsRuntime, null) is not JsonSerializerOptions options)
    {
      throw new ArgumentException($"Unable to get {nameof(JsonSerializerOptions)} from {nameof(IJSRuntime)}.");
    }

    return options;
  }
}