namespace Blazor.FamilyTreeJS.Interop;

/// <summary>
/// Interop layer from C# to JavaScript.
/// </summary>
internal class FamilyTreeJsInterop : IDisposable
{
  private class CallbackInterops
  {
    public IList<BaseCallbackInterop> ClientOwned { get; } = new List<BaseCallbackInterop>();

    public IList<BaseCallbackInterop> InternalOwned { get; } = new List<BaseCallbackInterop>();

  }

  private const string FamilyTreeJsInteropModule = "FamilyTreeJsInterop";

  private bool _disposed;

  private readonly IJSRuntime _jSRuntime;

  private readonly CallbackInterops _callbackInterops;

  private readonly ObjectTraversal _objectTraversal;

  public FamilyTreeJsInterop(IJSRuntime jSRuntime, ObjectTraversal objectTraversal)
  {
    _disposed = false;
    _jSRuntime = jSRuntime;
    _callbackInterops = new();
    _objectTraversal = objectTraversal;
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    // Dispose all callback interops
    var allCallbackInterops = _callbackInterops.InternalOwned
      .Concat(_callbackInterops.ClientOwned);

    foreach (var callbackInterop in allCallbackInterops)
    {
      callbackInterop.Dispose();
    }

    _callbackInterops.ClientOwned.Clear();
    _callbackInterops.InternalOwned.Clear();
    _disposed = true;
  }

  public async Task SetupFamilyTreeAsync(string treeId, FamilyTreeOptions? options = null)
  {
    if (options is null)
    {
      await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId);
      return;  
    }

    await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.setupFamilyTree", treeId, options);
  }

  public async Task LoadNodesAsync(string treeId, IEnumerable<Node> nodes)
    => await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.loadNodes", treeId, nodes);

  public async Task<bool> RemoveNodeAsync(string treeId, string nodeId)
    => await _jSRuntime.InvokeAsync<bool>($"{FamilyTreeJsInteropModule}.removeNode", treeId, nodeId);

  public async Task ReplaceNodeIdsAsync(string treeId, IDictionary<string, string> oldNewIdMappings)
    => await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.replaceNodeIds", treeId, oldNewIdMappings);

  public async Task RegisterOnUpdateNodeCallbackAsync(string treeId, EventCallback<UpdateNodeArgs> handler)
  {
    var callbackInterop = new EventCallbackInterop<UpdateNodeArgs>(handler);
    _callbackInterops.InternalOwned.Add(callbackInterop);

    await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerUpdateNodeHandler", treeId, callbackInterop);    
  }

  public async Task RegisterDefaultFirstNodeHandlerAsync(string treeId, Func<Node> handler)
  {
    var callbackInterop = new FuncCallbackInterop<Node>(handler);
    _callbackInterops.InternalOwned.Add(callbackInterop);

    var functionId = $"{FamilyTreeJsInteropModule}.registerDefaultFirstNodeHandler";
    await _jSRuntime.InvokeVoidAsync(functionId, treeId, callbackInterop);    
  }

  public async Task RegisterOnPhotoUploadCallbackAsync(string treeId, Func<PhotoUploadArgs, Task<string>> handler)
  {
    var callbackInterop = new FuncCallbackInterop<PhotoUploadArgs, Task<string>>(handler);
    _callbackInterops.InternalOwned.Add(callbackInterop);

    await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.registerPhotoUploadHandler", treeId, callbackInterop);
  }

  public async Task DestroyTreeAsync(string treeId)
  {
    await _jSRuntime.InvokeVoidAsync($"{FamilyTreeJsInteropModule}.destroyTree", treeId);
    
    // Only dispose the internal callback interops
    foreach (var callbackInterop in _callbackInterops.InternalOwned)
    {
      callbackInterop.Dispose();
    }
    _callbackInterops.InternalOwned.Clear();
  }

  public void AddCallbackInterops(FamilyTreeOptions options)
  {
    var baseCallbackInterops = _objectTraversal
      .FindRecursively(options, obj => typeof(BaseCallbackInterop).IsAssignableFrom(obj.GetType()))
      .Select(obj => (BaseCallbackInterop)obj);

    foreach (var callbackInterop in baseCallbackInterops)
    {
      _callbackInterops.ClientOwned.Add(callbackInterop);
    }
  }
}
