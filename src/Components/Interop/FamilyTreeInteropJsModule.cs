namespace Blazor.FamilyTreeJS.Components.Interop;

public sealed class FamilyTreeInteropJsModule : BaseJsModule
{
  private const string FamilyTreeJsInteropModule = "FamilyTreeJsInterop";

  protected override string ModulePath => $"{ModulePrefixPath}/js/{nameof(Components)}/{nameof(Interop)}/TypeScript/family-tree-interop.js";

  public FamilyTreeInteropJsModule(IJSRuntime jSRuntime) : base(jSRuntime)
  {}

  public async Task SetupFamilyTreeAsync(string treeId, FamilyTreeOptions? options = null)
  {
    if (options is null)
    {
      await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId);
      return;  
    }

    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId, options);
  }
}
