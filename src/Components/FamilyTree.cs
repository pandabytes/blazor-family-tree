namespace Blazor.FamilyTreeJS.Components;

public partial class FamilyTree : BaseScopeComponent
{
  private bool _finishedInitiazling = false;

  /// <summary>
  /// Wil be injected via DI scope.
  /// </summary>
  [InjectScope]
  private FamilyTreeInteropJsModule _familyTreeJsInterop = null!;

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

  [Parameter]
  public Guid TreeId { get; set; } = Guid.Empty;

  [Parameter]
  public string Style { get; set; } = "width: 100%; height: 100%;";

  /// <summary>
  /// Have to prefix with "tree" to satisfy selector format.
  /// </summary>
  protected string TreeIdForInterop => $"tree-{TreeId}";

//   public async Task LoadNodesAsync(IEnumerable<Node> nodes)
//     => await _familyTreeJsInterop.LoadNodesAsync(TreeIdForInterop, nodes);

//   public async Task ReplaceNodeIdsAsync(IDictionary<string, string> old_new_id_mappings)
//     => await _familyTreeJsInterop.ReplaceNodeIdsAsync(TreeIdForInterop, old_new_id_mappings);

//   public async Task<bool> RemoveNodeAsync(string nodeId)
//     => await _familyTreeJsInterop.RemoveNodeAsync(TreeIdForInterop, nodeId);

//   /// <summary>
//   /// Reset the the family tree to a clean state, i.e. its initial state.
//   /// </summary>
//   public async Task ResetAsync()
//   {
//     await _familyTreeJsInterop.DestroyTreeAsync(TreeIdForInterop);
//     await SetupFamilyTreeAsync();
//     StateHasChanged();
//   }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      await _familyTreeJsInterop.ImportAsync();
      await _familyTreeJsInterop.SetupFamilyTreeAsync(TreeIdForInterop);
    }
  }

//   private async Task SetupFamilyTreeAsync()
//   {
//     await _familyTreeJsInterop.SetupFamilyTreeAsync(TreeIdForInterop, Options);
//     await _familyTreeJsInterop.RegisterOnUpdateNodeCallbackAsync(TreeIdForInterop, OnUpdatedNode);

//     if (OnPhotoUpload is not null)
//     {
//       await _familyTreeJsInterop.RegisterOnPhotoUploadCallbackAsync(TreeIdForInterop, OnPhotoUpload);
//     }

//     if (OnDefaultFirstNode is not null)
//     {
//       await _familyTreeJsInterop.RegisterDefaultFirstNodeHandlerAsync(TreeIdForInterop, OnDefaultFirstNode);
//     }
//   }
}
