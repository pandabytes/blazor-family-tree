namespace Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeStatic;

/// <summary>
/// Module that provides static methods in
/// the FamilyTreeJS library. 
/// </summary>
public sealed class FamilyTreeStaticModule : BaseJsModule
{
  private const string FamilyTreeObject = "FamilyTreeObj";

  /// <inheritdoc />
  protected override string ModulePath { get; }

  /// <summary>
  /// Constructor.
  /// </summary>
  public FamilyTreeStaticModule(IJSRuntime jSRuntime) : base(jSRuntime)
  {
    var pathComponents = new string[]
    {
      ModulePrefixPath,
      "js",
      nameof(Components),
      nameof(Interop),
      nameof(Modules),
      nameof(FamilyTreeStatic),
      "family-tree-static.js",
    };
    ModulePath = string.Join('/', pathComponents);
  }

  /// <summary>
  /// Get the HTML SVG string of <paramref name="icon"/>.
  /// </summary>
  /// <remarks>
  /// Only the following icons can accept <paramref name="x"/> and <paramref name="y"/>:
  /// <see cref="Icon.Share"/>, <see cref="Icon.User"/>, 
  /// <see cref="Icon.AddUser"/>, <see cref="Icon.Close"/>, <see cref="Icon.FamilyTree"/>.
  /// Other icons will thrown an <see cref="ArgumentException"/>.
  /// </remarks>
  /// <param name="icon">Icon type.</param>
  /// <param name="width">Icon width.</param>
  /// <param name="height">Icon height.</param>
  /// <param name="color">Icon color.</param>
  /// <param name="x">Icon x position.</param>
  /// <param name="y">Icon y position.</param>
  /// <returns>Icon in SVG string.</returns>
  public async Task<string> GetIconAsync(Icon icon, string width, string height, string color, string? x = null, string? y = null)
  {
    var iconsWithXAndYParams = new List<Icon> { Icon.Share, Icon.User, Icon.AddUser, Icon.Close, Icon.FamilyTree };
    var hasXOrYParameter = !string.IsNullOrWhiteSpace(x) || !string.IsNullOrWhiteSpace(y);

    if (!iconsWithXAndYParams.Contains(icon) && hasXOrYParameter)
    {
      throw new ArgumentException($"Icon {icon} does not accept {nameof(x)} and {nameof(y)} parameters.");
    }

    return await Module.InvokeAsync<string>($"{FamilyTreeObject}.icon.{icon}", width, height, color, x, y);
  }
}
