namespace Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeInterop;

internal sealed class FamilyTreeInteropJsModule : BaseJsModule
{
  private const string FamilyTreeJsInteropModule = "FamilyTreeJsInteropObj";

  private readonly ObjectTraversal _objectTraversal;

  protected override string ModulePath { get; }

  public FamilyTreeInteropJsModule(ObjectTraversal objectTraversal, IJSRuntime jSRuntime)
    : base(jSRuntime)
  {
    _objectTraversal = objectTraversal;
    var pathComponents = new string[]
    {
      ModulePrefixPath,
      "js",
      nameof(Components),
      nameof(Interop),
      nameof(Modules),
      nameof(FamilyTreeInterop),
      "family-tree-interop.js",
    };
    ModulePath = string.Join('/', pathComponents);
  }

  public async Task<bool> TreeExistAsync(string treeId)
    => await Module.InvokeAsync<bool>($"{FamilyTreeJsInteropModule}.treeExist", treeId);

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

  public async Task LoadNodesAsync(string treeId, IEnumerable<Node> nodes)
    => await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.loadNodes", treeId, nodes);

  public async Task<bool> RemoveNodeAsync(string treeId, string nodeId)
    => await Module.InvokeAsync<bool>($"{FamilyTreeJsInteropModule}.removeNode", treeId, nodeId);

  public async Task ReplaceNodeIdsAsync(string treeId, IDictionary<string, string> oldNewIdMappings)
    => await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.replaceNodeIds", treeId, oldNewIdMappings);

  public async Task RegisterOnUpdateNodeCallbackAsync(string treeId, EventCallback<UpdateNodeArgs> handler)
  {
    var callbackInterop = new EventCallbackInterop<UpdateNodeArgs>(handler);
    _callbackInterops.Add(callbackInterop);
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerUpdateNodeHandler", treeId, callbackInterop);    
  }

  public async Task RegisterDefaultFirstNodeHandlerAsync(string treeId, Func<Node> handler)
  {
    var callbackInterop = new FuncCallbackInterop<Node>(handler);
    _callbackInterops.Add(callbackInterop);

    var functionId = $"{FamilyTreeJsInteropModule}.registerDefaultFirstNodeHandler";
    await Module.InvokeVoidAsync(functionId, treeId, callbackInterop);  
  }

  public async Task RegisterOnPhotoUploadCallbackAsync(string treeId, Func<PhotoUploadArgs, Task<string>> handler)
  {
    var callbackInterop = new FuncCallbackInterop<PhotoUploadArgs, Task<string>>(handler);
    _callbackInterops.Add(callbackInterop);
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerPhotoUploadHandler", treeId, callbackInterop);
  }

  public async Task DestroyTreeAsync(string treeId)
  {
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.destroyTree", treeId);

    foreach (var callbackInterop in _callbackInterops)
    {
      callbackInterop.Dispose();
    }
    _callbackInterops.Clear();
  }
}
