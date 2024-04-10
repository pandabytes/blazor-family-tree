namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Mirror the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options
/// </summary>
public record ButtonEdit(
  string? Icon = null,
  string? Text = null,
  bool? HideIfEditMode = null,
  bool? HideIfDetailsMode = null
);

/// <summary>
/// Mirror the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options
/// </summary>
public record ValidatorContent(string? Required = null, string? Email = null);

/// <summary>
/// Mirror the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options
/// </summary>
public record EditFormElement(
  string? Type = null,
  string? Label = null,
  string? Binding = null,
  IReadOnlyList<object>? Options = null,
  string? Btn = null,
  // Typo in the FamilyTreeJS library?
  ValidatorContent? Vlidators = null
);

/// <summary>
/// Mirror the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options
/// </summary>
public record EditForm(
  bool? ReadOnly = null,
  string? TitleBinding = null,
  string? PhotoBinding = null,
  string? FocusBinding = null,
  string? AddMore = null,
  string? AddMoreBtn = null,
  string? AddMoreFieldName = null,
  string? SaveAndCloseBtn = null,
  string? CancelBtn = null,
  bool? GenerateElementsFromFields = null,
  IReadOnlyDictionary<string, ButtonEdit?>? Buttons = null,
  IReadOnlyList<EditFormElement>? Elements = null
);
