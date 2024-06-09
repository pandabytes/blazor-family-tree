namespace Blazor.FamilyTreeJS.Components;

/// <summary>
/// FamilyTreeJS component.
/// </summary>
public partial class FamilyTree<TNode> : BaseScopeComponent where TNode : Node
{
  [InjectScope, AutoImportJsModule]
  private protected readonly FamilyTreeInteropJsModule _familyTreeJsInterop = null!;

  /// <summary>
  /// Specify the id of the tree.
  /// </summary>
  [Parameter, EditorRequired]
  public string TreeId { get; init; } = string.Empty;

  /// <summary>
  /// Options to configure and/or initialize the family tree.
  /// </summary>
  [Parameter]
  public FamilyTreeOptions? Options { get; init; }

  /// <summary>
  /// Event that gets fired when a node is added, updated, and/or removed.
  /// </summary>
  [Parameter]
  public EventCallback<UpdateNodeArgs> OnUpdatedNode { get; init; }

  /// <summary>
  /// Event that gets fired when a user uploads a person's photo.
  /// The return string indicates the url to where the image is uploaded to.
  /// Return empty string to indicate upload fails.
  /// 
  /// Since this is a delegate instead of <see cref="EventCallback"/>,
  /// <see cref="ComponentBase.StateHasChanged"/> will not be automatically
  /// called. You would need to call this yourself in your delegate.
  /// </summary>
  [Parameter]
  public Func<PhotoUploadArgs, Task<string>>? OnPhotoUpload { get; init; }

  /// <summary>
  /// Event that gets fired when a user first creates a new node
  /// on an empty family tree. The return node will be used as
  /// a "default" node when this operation happens.
  /// 
  /// Since this is a delegate instead of <see cref="EventCallback"/>,
  /// <see cref="ComponentBase.StateHasChanged"/> will not be automatically
  /// called. You would need to call this yourself in your delegate.
  /// </summary>
  [Parameter]
  public Func<Node>? OnDefaultFirstNode { get; init; }

  /// <summary>
  /// Custom style for the family tree.
  /// </summary>
  [Parameter]
  public string Style { get; init; } = string.Empty;

  /// <summary>
  /// Invoke the registered Action after the FamilyTree
  /// component is first rendered. This is only invoked once.
  /// Useful to know when the FamilyTree is available to use.
  /// This is invoked before <see cref="AfterFamilyTreeRenderAsync"/>.
  /// </summary>
  [Parameter]
  public Action? AfterFamilyTreeRender { get; init; }

  /// <summary>
  /// Invoke the registered Func asynchronously
  /// after the FamilyTree component is first rendered.
  /// This is only invoked once. Useful to know when the
  /// FamilyTree is available to use. This is invoked
  /// after <see cref="AfterFamilyTreeRender"/>.
  /// </summary>
  [Parameter]
  public Func<Task>? AfterFamilyTreeRenderAsync { get; init; }

  /// <summary>
  /// Have to prefix with "tree" to satisfy selector format.
  /// </summary>
  private string TreeIdForInterop => $"tree-{TreeId}";

  /// <summary>
  /// Load nodes to the family tree. This is useful
  /// when you don't have nodes to load during the
  /// family tree initialization and want to load
  /// the nodes at a later time.
  /// </summary>
  /// <param name="nodes">Nodes to be loaded.</param>
  public async Task LoadNodesAsync(IEnumerable<Node> nodes)
    => await _familyTreeJsInterop.LoadNodesAsync(TreeIdForInterop, nodes);

  /// <summary>
  /// Replace all references of an old node id with new node id.
  /// <paramref name="oldNewIdMappings"/> is a dictionary
  /// where key is the old id and value is the new id.
  /// </summary>
  /// <param name="oldNewIdMappings">Old id is key, new id is value.</param>
  public async Task ReplaceNodeIdsAsync(IDictionary<string, string> oldNewIdMappings)
    => await _familyTreeJsInterop.ReplaceNodeIdsAsync(TreeIdForInterop, oldNewIdMappings);

  /// <summary>
  /// Remove a node from the family tree.
  /// </summary>
  /// <param name="nodeId">Id of the node to be removed.</param>
  /// <returns>True if node was removed, false otherwise.</returns>
  public async Task<bool> RemoveNodeAsync(string nodeId)
    => await _familyTreeJsInterop.RemoveNodeAsync(TreeIdForInterop, nodeId);

  /// <summary>
  /// Reset the the family tree to a clean state, i.e. to its initial state.
  /// </summary>
  public async Task ResetAsync()
  {
    await _familyTreeJsInterop.DestroyTreeAsync(TreeIdForInterop);
    await SetupFamilyTreeAsync();
    StateHasChanged();
  }

  /// <inheritdoc />
  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    await base.OnAfterRenderAsync(firstRender);

    if (firstRender)
    {
      await SetupFamilyTreeAsync();

      AfterFamilyTreeRender?.Invoke();

      if (AfterFamilyTreeRenderAsync is not null)
      {
        await AfterFamilyTreeRenderAsync.Invoke();
      }
    }
  }

  /// <inheritdoc />
  protected override void OnParametersSet()
  {
    base.OnParametersSet();

    if (string.IsNullOrWhiteSpace(TreeId))
    {
      throw new ArgumentException($"{nameof(TreeId)} cannot be empty.");
    }
  }

  /// <inheritdoc />
  public override async Task SetParametersAsync(ParameterView parameters)
  {
    // Prevent any parameter from changing after this component
    // has been initialized. Once we know TreeId is non-null we
    // know for sure this component has been initialized and
    // its parameter cannot be changed
    if (string.IsNullOrWhiteSpace(TreeId))
    {
      await base.SetParametersAsync(parameters);
    }
  }

  /// <inheritdoc/>
  protected override async ValueTask DisposeAsyncCore()
  {
    await _familyTreeJsInterop.DestroyTreeAsync(TreeIdForInterop);
    await base.DisposeAsyncCore();
  }

  private async Task SetupFamilyTreeAsync()
  {
    var exist = await _familyTreeJsInterop.TreeExistAsync(TreeIdForInterop);
    if (exist)
    {
      throw new ArgumentException($"Tree id \"{TreeId}\" already exists. Please use a different id.");
    }

    await _familyTreeJsInterop.SetupFamilyTreeAsync(TreeIdForInterop, Options);
    await _familyTreeJsInterop.RegisterOnUpdateNodeCallbackAsync(TreeIdForInterop, OnUpdatedNode);

    if (OnPhotoUpload is not null)
    {
      await _familyTreeJsInterop.RegisterOnPhotoUploadCallbackAsync(TreeIdForInterop, OnPhotoUpload);
    }

    if (OnDefaultFirstNode is not null)
    {
      await _familyTreeJsInterop.RegisterDefaultFirstNodeHandlerAsync(TreeIdForInterop, OnDefaultFirstNode);
    }
  }
}
