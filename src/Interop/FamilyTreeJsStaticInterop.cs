namespace Blazor.FamilyTreeJS.Interop;

/// <summary>
/// Interop layer from C# to JavaScript.
/// Mainly expose some static FamilyJS functions.
/// </summary>
public class FamilyTreeJsStaticInterop
{
  private const string FamilyTreeJsStaticInteropModule = "FamilyTreeStaticInterop";

  private readonly IJSRuntime _jSRuntime;

  public FamilyTreeJsStaticInterop(IJSRuntime jSRuntime)
  {
    _jSRuntime = jSRuntime;
  }

  public async Task<string> GetBuiltInIconAsync(string iconType, float width, float height, string color)
    => await _jSRuntime.InvokeAsync<string>($"{FamilyTreeJsStaticInteropModule}.getBuiltInIcon", iconType, width, height, color);
}
