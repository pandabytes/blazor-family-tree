namespace Blazor.FamilyTreeJS.Components;

/// <summary>
/// FamilyTreeJS component.
/// </summary>
/// <typeparam name="TNode">
/// The type of the nodes that are
/// stored in this fanmily tree.
/// </typeparam>
public sealed class FamilyTree<TNode> : BaseFamilyTree<TNode> where TNode : BaseNode
{}
