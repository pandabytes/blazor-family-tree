namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// A custom readonly textbox input element.
/// </summary>
/// <remarks>
/// Based off of https://github.com/BALKANGraph/FamilyTreeJS/issues/119#issuecomment-2155809675.
/// </remarks>
public static class ReadOnlyTextbox
{
  /// <summary>
  /// Type of the the readonly text box.
  /// </summary>
  public static readonly string Type = "readOnlyTextbox";

  /// <summary>
  /// Callback that will return a HTML string that describes
  /// the input to FamilyTreeJS. For more details on the parameter
  /// please see <see cref="InputElementCallback{TNode}"/>.
  /// </summary>
  /// <param name="data">Node data.</param>
  /// <param name="editElement">Edit element object.</param>
  /// <param name="minWidth">Minimum width.</param>
  /// <param name="readOnly">Readonly.</param>
  /// <typeparam name="TNode">
  /// The type of the nodes that are
  /// stored in this fanmily tree.
  /// </typeparam>
  /// <returns>Result for FamilyTreeJS to use.</returns>
  public static InputElementResult Callback<TNode>(
    TNode data, EditFormElement editElement,
    string minWidth, bool? readOnly
  ) where TNode : BaseNode
  {
    var emptyResult = new InputElementResult(string.Empty, null, null);
    if (editElement.Binding is null)
    {
      return emptyResult;
    }

    // If readOnly is null it means the details
    // page was opened via the "Edit" menu, so
    // we treat null as editable
    var localReadOnly = readOnly ?? false;

    // Binding value is always in camel case so we need to convert it to
    // pascal case to match with C# naming convention
    var nodeId = data.Id;
    var value = data.Get<object?>(editElement.Binding.ToFirstCharUpper());

    // Input has no value so simply exit early
    if (localReadOnly &&
        (value is null || string.IsNullOrWhiteSpace(value as string)))
    {
      return emptyResult;
    }

    // Whether it is readonly or not, for this input
    // type we always want to make it readonly
    var html = localReadOnly ? @$"
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
                 maxlength=""256""
                 id=""{nodeId}""
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
