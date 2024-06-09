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
/// their associated <see cref="FamilyTree{TNode}"/> component is disposed.
/// </remarks>
/// <typeparam name="TNode">
/// The type of the nodes that are
/// stored in this fanmily tree options object.
/// </typeparam>
public record FamilyTreeOptions<TNode>(
  string Mode = "light",
  bool? EnableSearch = null,
  bool? EnablePan = null,
  bool? MiniMap = null,
  string? Template = null,
  NodeMenu? NodeMenu = null,
  IReadOnlyList<TNode>? Nodes = null,
  IReadOnlyDictionary<string, string>? NodeBinding = null,
  EditForm? EditForm = null,
  bool? NodeTreeMenu = null
) where TNode : BaseNode;
