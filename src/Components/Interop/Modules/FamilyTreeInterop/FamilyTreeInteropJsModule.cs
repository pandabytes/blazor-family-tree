namespace Blazor.FamilyTreeJS.Components.Interop.Modules.FamilyTreeInterop;

internal sealed class FamilyTreeInteropJsModule<TNode> : BaseJsModule where TNode : BaseNode
{
  private const string FamilyTreeJsInteropModule = "FamilyTreeJsInteropObj";

  private readonly ObjectTraversal _objectTraversal;

  /// <summary>
  /// This stores the callbacks supplied by the client.
  /// </summary>
  private readonly IList<BaseCallbackInterop> _clientCallbacks;

  protected override string ModulePath { get; }

  public FamilyTreeInteropJsModule(ObjectTraversal objectTraversal, IJSRuntime jSRuntime)
    : base(jSRuntime)
  {
    _objectTraversal = objectTraversal;
    _clientCallbacks = new List<BaseCallbackInterop>();

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

  public async Task SetupFamilyTreeAsync(string treeId, FamilyTreeOptions<TNode>? options = null)
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
      _clientCallbacks.Add(callbackInterop);
    }
  }

  public async Task LoadNodesAsync(string treeId, IEnumerable<TNode> nodes)
    => await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.loadNodes", treeId, nodes);

  public async Task<bool> RemoveNodeAsync(string treeId, string nodeId)
    => await Module.InvokeAsync<bool>($"{FamilyTreeJsInteropModule}.removeNode", treeId, nodeId);

  public async Task ReplaceNodeIdsAsync(string treeId, IDictionary<string, string> oldNewIdMappings)
    => await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.replaceNodeIds", treeId, oldNewIdMappings);

  public async Task RegisterOnUpdateNodeCallbackAsync(string treeId, EventCallback<UpdateNodeArgs<TNode>> handler)
  {
    var callbackInterop = new EventCallbackInterop<UpdateNodeArgs<TNode>>(handler);
    CallbackInterops.Add(callbackInterop);
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerUpdateNodeHandler", treeId, callbackInterop);    
  }

  public async Task RegisterDefaultFirstNodeHandlerAsync(string treeId, Func<TNode> handler)
  {
    var callbackInterop = new FuncCallbackInterop<TNode>(handler);
    CallbackInterops.Add(callbackInterop);

    var functionId = $"{FamilyTreeJsInteropModule}.registerDefaultFirstNodeHandler";
    await Module.InvokeVoidAsync(functionId, treeId, callbackInterop);  
  }

  public async Task RegisterOnPhotoUploadCallbackAsync(string treeId, Func<PhotoUploadArgs, Task<string>> handler)
  {
    var callbackInterop = new FuncCallbackInterop<PhotoUploadArgs, Task<string>>(handler);
    CallbackInterops.Add(callbackInterop);
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerPhotoUploadHandler", treeId, callbackInterop);
  }

  public async Task AddCustomInputElementAsync(
    string treeId,
    string inputType,
    InputElementCallback<TNode> callback
  )
  {
    var callbackInterop = new FuncCallbackInterop<TNode, EditFormElement, string, bool?, InputElementResult>(callback.Invoke);
    CallbackInterops.Add(callbackInterop);
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.addCustomInputElement", treeId, inputType, callbackInterop);
  }

  /// <summary>
  /// Destroy the tree object in JS (removing it from view). This doesn't mean
  /// disposing all resources, it simply removes the FamilyTree object in JS.
  /// Do not rely on this object's dispose method to "destroy" the family tree.
  /// The caller must explicitly destroy the family tree and then dispose this
  /// object. The dispose method in this object is only responsible for disposing
  /// itself not the FamilyTree object in JS.
  /// </summary>
  /// <param name="treeId">Id of tree to be destroyed.</param>
  public async Task DestroyTreeAsync(string treeId)
  {
    await Module.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.destroyTree", treeId);

    // For callbacks that we create internally (within this class), it's safe to
    // dispose them. But for callbacks created by the client, we defer
    // disposing them in DisposeAsyncCore because the client owns those callbacks
    // and we don't want to dispose them when DestroyTreeAsync is called.
    // Calling DestroyTreeAsync() does not mean we release all resources.
    foreach (var callbackInterop in CallbackInterops)
    {
      callbackInterop.Dispose();
    }
    CallbackInterops.Clear();
  }

  /// <inheritdoc />
  protected override async ValueTask DisposeAsyncCore()
  {
    foreach (var callbackInterop in _clientCallbacks)
    {
      callbackInterop.Dispose();
    }
    _clientCallbacks.Clear();

    await base.DisposeAsyncCore();
  }
}
