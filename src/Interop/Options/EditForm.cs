namespace Blazor.FamilyTreeJS.Interop.Options;

public record ButtonEdit(
  string? Icon = null,
  string? Text = null,
  bool? HideIfEditMode = null,
  bool? HideIfDetailsMode = null
);

public record ValidatorContent(string? Required = null, string? Email = null);

public record EditFormElement(
  string? Type = null,
  string? Label = null,
  string? Binding = null,
  IReadOnlyList<object>? Options = null,
  string? Btn = null,
  // Typo in the FamilyTreeJS library?
  ValidatorContent? Vlidators = null
);

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
