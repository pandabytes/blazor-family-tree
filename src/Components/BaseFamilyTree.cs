namespace Blazor.FamilyTreeJS.Components;

/// <summary>
/// Base class for the FamilyTreeJS component.
/// Although it is public, this class is not meant
/// to be extended by classes from external assemblies.
/// </summary>
/// <typeparam name="TNode">
/// The type of the nodes that are
/// stored in this fanmily tree.
/// </typeparam>
public abstract partial class BaseFamilyTree<TNode> : BaseScopeComponent where TNode : BaseNode
{
  /// <summary>
  /// Allow this field to be inherited only within this assembly.
  /// </summary>
  [InjectScope, AutoImportJsModule]
  private protected FamilyTreeInteropJsModule<TNode> FamilyTreeJsInterop { get; init; } = null!;

  /// <summary>
  /// Specify the id of the tree.
  /// </summary>
  [Parameter, EditorRequired]
  public string TreeId { get; init; } = string.Empty;

  /// <summary>
  /// Options to configure and/or initialize the family tree.
  /// </summary>
  [Parameter]
  public RootOptions<TNode>? Options { get; init; }

  /// <summary>
  /// Event that gets fired when a node is added, updated, and/or removed.
  /// </summary>
  [Parameter]
  public EventCallback<UpdateNodeArgs<TNode>> OnUpdatedNode { get; init; }

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
  public Func<TNode>? OnDefaultFirstNode { get; init; }

  /// <summary>
  /// Event that gets fired when a user clicks on the button link
  /// associated to an input textbox. The return string indicates the value
  /// to be set in the input textbox.
  /// 
  /// Since this is a delegate instead of <see cref="EventCallback"/>,
  /// <see cref="ComponentBase.StateHasChanged"/> will not be automatically
  /// called. You would need to call this yourself in your delegate.
  /// </summary>
  [Parameter]
  public Func<TextboxButtonClickedArgs, Task<string>>? OnTextboxButtonClicked { get; init; }

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
  protected string TreeIdForInterop => $"tree-{TreeId}";

  /// <summary>
  /// Make the consturctor internal so that
  /// external assemblies cannot inherit this class.
  /// Only classes in this assembly can inherit.
  /// </summary>
  internal BaseFamilyTree() : base() {}

  /// <summary>
  /// Load nodes to the family tree. This is useful
  /// when you don't have nodes to load during the
  /// family tree initialization and want to load
  /// the nodes at a later time.
  /// </summary>
  /// <param name="nodes">Nodes to be loaded.</param>
  public async Task LoadNodesAsync(IEnumerable<TNode> nodes)
    => await FamilyTreeJsInterop.LoadNodesAsync(TreeIdForInterop, nodes);

  /// <summary>
  /// Replace all references of an old node id with new node id.
  /// <paramref name="oldNewIdMappings"/> is a dictionary
  /// where key is the old id and value is the new id.
  /// </summary>
  /// <param name="oldNewIdMappings">Old id is key, new id is value.</param>
  public async Task ReplaceNodeIdsAsync(IDictionary<string, string> oldNewIdMappings)
    => await FamilyTreeJsInterop.ReplaceNodeIdsAsync(TreeIdForInterop, oldNewIdMappings);

  /// <summary>
  /// Remove a node from the family tree.
  /// </summary>
  /// <param name="nodeId">Id of the node to be removed.</param>
  /// <returns>True if node was removed, false otherwise.</returns>
  public async Task<bool> RemoveNodeAsync(string nodeId)
    => await FamilyTreeJsInterop.RemoveNodeAsync(TreeIdForInterop, nodeId);

  /// <summary>
  /// Add curved link.
  /// </summary>
  /// <param name="fromNodeId">From node.</param>
  /// <param name="toNodeId">To node.</param>
  /// <param name="label">Label on the curved link.</param>
  /// <param name="template">Template of the curved link.</param>
  public async Task AddClinkAsync(string fromNodeId, string toNodeId, string? label = null, string? template = null)
    => await FamilyTreeJsInterop.AddClinkAsync(TreeIdForInterop, fromNodeId, toNodeId, label, template);

  /// <summary>
  /// Remove curved link.
  /// </summary>
  /// <param name="fromNodeId">From node.</param>
  /// <param name="toNodeId">To node.</param>
  public async Task RemoveClinkAsync(string fromNodeId, string toNodeId)
    => await FamilyTreeJsInterop.RemoveClinkAsync(TreeIdForInterop, fromNodeId, toNodeId);

  /// <summary>
  /// Reset the the family tree to a clean state, i.e. to its initial state.
  /// </summary>
  public async Task ResetAsync()
  {
    await FamilyTreeJsInterop.DestroyTreeAsync(TreeIdForInterop);
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
    await FamilyTreeJsInterop.DestroyTreeAsync(TreeIdForInterop);
    await base.DisposeAsyncCore();
  }

  private protected async Task SetupFamilyTreeAsync()
  {
    var exist = await FamilyTreeJsInterop.TreeExistAsync(TreeIdForInterop);
    if (exist)
    {
      throw new ArgumentException($"Tree id \"{TreeId}\" already exists. Please use a different id.");
    }

    var familyTreeOpts = Options?.FamilyTreeOptions;
    var nonFamilyTreeOpts = Options?.NonFamilyTreeOptions;

    await FamilyTreeJsInterop.SetupFamilyTreeAsync(TreeIdForInterop, familyTreeOpts);
    await FamilyTreeJsInterop.RegisterOnUpdateNodeCallbackAsync(TreeIdForInterop, OnUpdatedNode);

    if (OnDefaultFirstNode is not null)
    {
      await FamilyTreeJsInterop.RegisterDefaultFirstNodeHandlerAsync(TreeIdForInterop, OnDefaultFirstNode);
    }
  
    if (OnTextboxButtonClicked is not null)
    {
      await FamilyTreeJsInterop.RegisterTextboxButtonClickedHandlerAsync(TreeIdForInterop, OnTextboxButtonClicked);
    }

    await AddCustomInputElementsAsync(nonFamilyTreeOpts);
  }

  private async Task AddCustomInputElementsAsync(NonFamilyTreeOptions<TNode>? nonFamilyTreeOpts)
  {
    var customInputElements = nonFamilyTreeOpts?.CustomInputElements ??
      Enumerable.Empty<KeyValuePair<string, InputElementCallback<TNode>>>();

    foreach (var (inputType, inputElementCallback) in customInputElements)
    {
      await FamilyTreeJsInterop.AddCustomInputElementAsync(TreeIdForInterop, inputType, inputElementCallback);
    }
  }
}
