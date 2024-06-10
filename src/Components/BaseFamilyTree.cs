namespace Blazor.FamilyTreeJS.Components;

public static class BaseNodeExtension
{
  public static T Get<T>(this BaseNode node, string propertyName)
  {
    var type = node.GetType();
    var property = type.GetProperty(propertyName);
    var value = (property?.GetValue(node)) ?? throw new ArgumentException($"Failed to get value of property \"{propertyName}\".");
    return (T)value;
  }

  public static string ToFirstCharUpper(this string input)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      return string.Empty;
    }
    return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
  }
}

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
  private protected readonly FamilyTreeInteropJsModule<TNode> _familyTreeJsInterop = null!;

  /// <summary>
  /// Specify the id of the tree.
  /// </summary>
  [Parameter, EditorRequired]
  public string TreeId { get; init; } = string.Empty;

  /// <summary>
  /// Options to configure and/or initialize the family tree.
  /// </summary>
  [Parameter]
  public FamilyTreeOptions<TNode>? Options { get; init; }

  /// <summary>
  /// Event that gets fired when a node is added, updated, and/or removed.
  /// </summary>
  [Parameter]
  public EventCallback<UpdateNodeArgs<TNode>> OnUpdatedNode { get; init; }

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
  public Func<TNode>? OnDefaultFirstNode { get; init; }

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
  
    var inputName = "readOnlyTextBox";
    await _familyTreeJsInterop.AddCustomInputElementAsync(inputName, CustomInputElement);
    await _familyTreeJsInterop.AddCustomInputElementAsync(inputName, CustomInputElement);
  }

  private InputElementResult CustomInputElement(
    TNode data, EditFormElement editElement,
    string minWidth, bool readOnly
  )
  {
    var emptyResult = new InputElementResult(string.Empty, null, null);
    if (editElement.Binding is null)
    {
      return emptyResult;
    }

    // Binding value is always in camel case so we need to convert it to
    // pascal case to match with C# naming convention
    var nodeId = data.Id;
    var value = data.Get<object?>(editElement.Binding.ToFirstCharUpper());

    if (readOnly && value is null)
    {
      return emptyResult;
    }

    var html = readOnly ? @$"
      <div class=""bft-input"" data-bft-input="""" data-bft-input-disabled="""">
        <label for=""{nodeId}"" class=""hasval"">{editElement.Label}</label>
        <input readonly
               data-binding=""{editElement.Binding}""
               maxlength=""256""
               id=""{nodeId}""
               name=""{nodeId}""
               type=""text"" value=""{value}"" autocomplete=""off"">
      </div>
    " : @$"
      <div class=""bft-form-field"" style=""min-width: {minWidth};"">
        <div class=""bft-input"" data-bft-input="""">
          <label for=""{nodeId}"" class=""hasval"">{editElement.Label}</label>
          <input readonly
                 disabled
                 data-binding=""{editElement.Binding}""
                 maxlength=""256"" id=""{nodeId}""
                 name=""{nodeId}""
                 type=""text""
                 value=""{value}""
                 autocomplete=""off"">
        </div>
      </div>
    ";

    return new(html, nodeId, value);
  }
}
