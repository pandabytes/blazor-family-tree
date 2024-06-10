namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// A custom readonly textbox input element.
/// </summary>
/// <remarks>
/// Based off of https://github.com/BALKANGraph/FamilyTreeJS/issues/119#issuecomment-2155809675.
/// </remarks>
public static class ReadOnlyTextBox
{
  /// <summary>
  /// Type of the the readonly text box.
  /// </summary>
  public static readonly string Type = "readOnlyTextBox";

  /// <summary>
  /// 
  /// </summary>
  /// <param name="data"></param>
  /// <param name="editElement"></param>
  /// <param name="minWidth"></param>
  /// <param name="readOnly"></param>
  /// <typeparam name="TNode"></typeparam>
  /// <returns></returns>
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

    // Binding value is always in camel case so we need to convert it to
    // pascal case to match with C# naming convention
    var nodeId = data.Id;
    var value = data.Get<object?>(editElement.Binding.ToFirstCharUpper());

    if ((readOnly ?? false) && value is null)
    {
      return emptyResult;
    }

    var html = (readOnly ?? false) ? @$"
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
