namespace Blazor.FamilyTreeJS.Interop.Options;

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
