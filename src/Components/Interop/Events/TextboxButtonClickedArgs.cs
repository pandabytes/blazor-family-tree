namespace Blazor.FamilyTreeJS.Components.Interop.Events;

/// <summary>
/// 
/// </summary>
/// <param name="NodeId"></param>
/// <param name="BindingName"></param>
/// <param name="ButtonText"></param>
/// <returns></returns>
public record TextboxButtonClickedArgs(string NodeId, string? BindingName = null, string? ButtonText = null);
