namespace Blazor.FamilyTreeJS.Components.Interop.Events;

/// <summary>
/// Contain data when an input textbox button is clicked.
/// </summary>
/// <param name="NodeId">Node id associated to the input textbox.</param>
/// <param name="BindingName">Binding name of the input textbox.</param>
/// <param name="ButtonText">Text of the input textbox button.</param>
public record TextboxButtonClickedArgs(string NodeId, string? BindingName = null, string? ButtonText = null);
