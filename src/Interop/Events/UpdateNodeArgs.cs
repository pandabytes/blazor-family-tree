namespace Blazor.FamilyTreeJS.Interop.Events;

/// <summary>
/// Contain data when the update node event is triggered.
/// </summary>
/// <param name="AddNodesData">New nodes.</param>
/// <param name="UpdateNodesData">Updated nodes.</param>
/// <param name="RemoveNodeId">
/// This can be null but the Typescript definition states that its type is "string | number".
/// </param>
public record UpdateNodeArgs(
  IReadOnlyList<Node> AddNodesData,
  IReadOnlyList<Node> UpdateNodesData,
  string? RemoveNodeId = null
);
