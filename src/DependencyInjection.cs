using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
  public static WebAssemblyHost ConfigureIJSRuntimeJsonOptions(this WebAssemblyHost webHost)
  {
    var jsRuntime = webHost.Services.GetRequiredService<IJSRuntime>();
    var options = GetJsonSerializerOptions(jsRuntime);

    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.TypeInfoResolver = new PolymorphicTypeResolver();
    return webHost;
  }

  /// <summary>
  /// Register classes that derive from <typeparamref name="TBase"/>
  /// into the JSON serialization/deserialization process. This
  /// enables the derived classes to be serialized/deserialized
  /// correctly with their custom properties.
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
    var jsRuntime = webHost.Services.GetRequiredService<IJSRuntime>();
    var options = GetJsonSerializerOptions(jsRuntime);
    if (options.TypeInfoResolver is not PolymorphicTypeResolver polymorhphicResolver)
    {
      throw new InvalidOperationException($"Expect {nameof(JsonSerializerOptions.TypeInfoResolver)} to be " +
                                          $"of type {nameof(PolymorphicTypeResolver)}. Please call " +
                                          $"method {nameof(ConfigureIJSRuntimeJsonOptions)} first.");
    }

    polymorhphicResolver.AddDerivedTypes<TBase>(types);
    return webHost;
  }

  /// <summary>
  /// See https://github.com/dotnet/aspnetcore/issues/12685#issuecomment-603050776
  /// </summary>
  /// <param name="jsRuntime"></param>
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