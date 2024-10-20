namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// See https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.link
/// </summary>
/// <param name="From"></param>
/// <param name="To"></param>
/// <param name="Label"></param>
/// <param name="Template"></param>
public sealed record Link(
  string? From = null,
  string? To = null,
  string? Label = null,
  string? Template = null
);
