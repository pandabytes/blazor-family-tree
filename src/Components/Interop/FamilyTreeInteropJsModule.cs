namespace Blazor.FamilyTreeJS.Components.Interop;

internal sealed class FamilyTreeInteropJsModule : BaseJsModule
{
  private const string FamilyTreeJsInteropModule = "FamilyTreeJsInteropObj";

  private readonly ObjectTraversal _objectTraversal;

  protected override string ModulePath => $"{ModulePrefixPath}/js/{nameof(Components)}/{nameof(Interop)}/TypeScript/family-tree-interop.js";

  public FamilyTreeInteropJsModule(ObjectTraversal objectTraversal, IJSRuntime jSRuntime)
    : base(jSRuntime)
  {
    _objectTraversal = objectTraversal;
  }

  public async Task SetupFamilyTreeAsync(string treeId, FamilyTreeOptions? options = null)
  {
    if (options is null)
    {
      await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId);
      return;  
    }

    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId, options);

    // Store the callback interops in the options object
    // so that we can manage when to dispose them, so
    // that client doesn't need to manually dispose
    // these callback interops
    var baseCallbackInterops = _objectTraversal
      .FindRecursively(options, obj => typeof(BaseCallbackInterop).IsAssignableFrom(obj.GetType()))
      .Select(obj => (BaseCallbackInterop)obj);

    foreach (var callbackInterop in baseCallbackInterops)
    {
      _callbackInterops.Add(callbackInterop);
    }
  }
}
