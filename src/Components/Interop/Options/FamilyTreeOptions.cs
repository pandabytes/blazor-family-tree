namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// This is the root class of all FamilyTreeJS's options and
/// it mirrors the FamilyTreeJS's options interface
/// https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options.
/// </summary>
/// <remarks>
/// Some of the properties (and their nested properties) have the type
/// <see cref="BaseCallbackInterop"/>. This type allows passing
/// a C# callback, such as Action or Func, to Javascript. Hence to
/// make it easy for client to use, all objects of type
/// <see cref="BaseCallbackInterop"/> (and its the derived types)
/// specified in this record, will be automatically disposed when
/// their associated <see cref="FamilyTree"/> component is disposed.
/// </remarks>
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
