namespace Blazor.FamilyTreeJS.Components;

/// <summary>
/// FamilyTreeJS component.
/// </summary>
public sealed partial class FamilyTree : BaseScopeComponent
{
  [InjectScope, AutoImportJsModule]
  private readonly FamilyTreeInteropJsModule _familyTreeJsInterop = null!;

  /// <summary>
  /// Specify the id of the tree.
  /// </summary>
  [Parameter, EditorRequired]
  public string TreeId { get; set; } = string.Empty;

  /// <summary>
  /// Options to configure and/or initialize the family tree.
  /// </summary>
  [Parameter, EditorRequired]
  public FamilyTreeOptions Options { get; init; } = null!;

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
  public string Style { get; set; } = "width: 100%; height: 100%;";

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
    }
  }

  private async Task SetupFamilyTreeAsync()
  {
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
