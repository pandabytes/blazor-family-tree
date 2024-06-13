namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// Result returned to FamilyTreeJS. FamilyTreeJS will use
/// this object to render the input field.
/// </summary>
/// <param name="Html">HTML string of the input to be rendered.</param>
/// <param name="Id">Node id.</param>
/// <param name="Value">Value of the input.</param>
/// <returns></returns>
public sealed record InputElementResult(string Html, string? Id, object? Value);

/// <summary>
/// Callback that will return a HTML string that describes
/// the input to FamilyTreeJS.
/// </summary>
/// <param name="data">Node data.</param>
/// <param name="editElement">Edit element object.</param>
/// <param name="minWidth">
/// Minimum width of the input. This is a string that describes
/// the width in pixel, such as "280px".
/// </param>
/// <param name="readOnly">
/// This indicates whether the input was invoked in a readonly or edit
/// view. If true, it means the input is readonly when the user "views" the details
/// of a node. Else it is editable when the user "edits" the details of a node.
/// This parameter can also be null.
/// </param>
/// <typeparam name="TNode">
/// The type of the nodes that are
/// stored in this fanmily tree.
/// </typeparam>
/// <returns>Result for FamilyTreeJS to use.</returns>
public delegate InputElementResult InputElementCallback<TNode>(
  TNode data, EditFormElement editElement,
  string minWidth, bool? readOnly
) where TNode : BaseNode;
