using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// The root object that contains all the options
/// for the <see cref="FamilyTree{TNode}"/> component.
/// </summary>
/// <typeparam name="TNode">
/// The type of the nodes that are
/// stored in this fanmily tree.
/// </typeparam>
public sealed record RootOptions<TNode>(
  FamilyTreeOptions<TNode>? FamilyTreeOptions = null,
  [property: JsonIgnore] NonFamilyTreeOptions<TNode>? NonFamilyTreeOptions = null
) where TNode : BaseNode;

/// <summary>
/// This contains options that do not mirror the FamilyTreeJS's options
/// interface - https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options.
/// They provide additional options that we want to provide outside of FamiyTreeJS.
/// </summary>
/// <param name="CustomInputElements">
/// Specificy any custom input element for the family tree.
/// Key is the input type such as "myCustomTextBox" and value
/// is a callback that constructs an HTML string that FamilyTreeJS
/// can use to render. Note that the input type must be unique
/// across ALL instances of <see cref="FamilyTree{TNode}" />.
/// For example, if input type "myCustomTextBox" is already used by
/// a family tree, then other family trees cannot use this input type.
/// </param>
/// <typeparam name="TNode"></typeparam>
public sealed record NonFamilyTreeOptions<TNode>(
  IReadOnlyDictionary<string, InputElementCallback<TNode>>? CustomInputElements = null
) where TNode : BaseNode;

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
public sealed record FamilyTreeOptions<TNode>(
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
