namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Mirror the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options
/// </summary>
public record FamilyTreeOptions(
  string Mode = "light",
  bool? EnableSearch = null,
  bool? EnablePan = null,
  bool? MiniMap = null,
  string? Template = null,
  NodeMenu? NodeMenu = null,
  IReadOnlyList<Node>? Nodes = null,
  IReadOnlyDictionary<string, string>? NodeBinding = null,
  EditForm? EditForm = null,
  bool? NodeTreeMenu = null
);
