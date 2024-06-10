namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// 
/// </summary>
/// <param name="Html"></param>
/// <param name="Id"></param>
/// <param name="Value"></param>
/// <returns></returns>
public record InputElementResult(string Html, string? Id, object? Value);

/// <summary>
/// 
/// </summary>
/// <param name="data"></param>
/// <param name="editElement"></param>
/// <param name="minWidth"></param>
/// <param name="readOnly"></param>
/// <typeparam name="TNode"></typeparam>
/// <returns></returns>
public delegate InputElementResult InputElementCallback<TNode>(
  TNode data, EditFormElement editElement,
  string minWidth, bool? readOnly
) where TNode : BaseNode;
