using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Node or person in the family tree. This is the
/// basic unit of the family tree. Each node/person
/// in the family tree is a <see cref="BaseNode"/> object
/// or any record that inherits from <see cref="BaseNode"/>.
/// </summary>
public record BaseNode
{
  /// <summary>
  /// Id of the node. Need to be unique within
  /// a family tree.
  /// </summary>
  public required string Id { get; init; }

  /// <summary>
  /// Gender of the node.
  /// </summary>
  [JsonConverter(typeof(GenderConverter))]
  public Gender? Gender { get; init; }

  /// <summary>
  /// List of partner ids. A node
  /// can have more than one partner such as
  /// ex-husband/ex-wife.
  /// See https://balkan.app/FamilyTreeJS/API/interfaces/FamilyTree.options#nodes-1
  /// </summary>
  [JsonPropertyName("pids")]
  public IReadOnlyList<string>? PartnerIds { get; init; }

  /// <summary>
  /// Father id of this node. This id belongs
  /// to another <see cref="Node"/> object.
  /// </summary>
  [JsonPropertyName("fid")]
  public string? FatherId { get; init; }

  /// <summary>
  /// Mother id of this node. This id belongs
  /// to another <see cref="Node"/> object.
  /// </summary>
  [JsonPropertyName("mid")]
  public string? MotherId { get; init; }

  /// <summary>
  /// Profile photo of this node. This is an
  /// URL link to where the photo is located.
  /// </summary>
  public string? Photo { get; init; }
}
