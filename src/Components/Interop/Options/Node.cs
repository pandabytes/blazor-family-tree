using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Node or person in the family tree. This is the
/// basic unit of the family tree. Each node/person
/// in the family tree is a <see cref="Node"/> object.
/// </summary>
public record Node
{
  private string _name = string.Empty;

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
  /// Name of the node which is the concatenation
  /// of <see cref="FirstName"/>, <see cref="MiddleName"/>,
  /// and <see cref="LastName"/>.
  /// </summary>
  public string Name
  {
    get => _name;
    init
    {
      _name = value.Trim();

      if (string.IsNullOrWhiteSpace(_name))
      {
        FirstName = string.Empty;
        MiddleName = string.Empty;
        LastName = string.Empty;
        return;
      }

      var firstIndex = _name.IndexOf(' ');
      var lastIndex = _name.LastIndexOf(' ');

      // At this point we have at least non-whitespace characters
      // If we don't find any whitespace, it means we use the
      // the entire string for first name
      if (firstIndex == -1)
      {
        FirstName = _name;
        MiddleName = string.Empty;
        LastName = string.Empty;
      }
      // This means we only have first and last name, no middle name
      else if (firstIndex == lastIndex)
      {
        FirstName = _name[..firstIndex].Trim();
        MiddleName = string.Empty;
        LastName = _name[(lastIndex + 1)..].Trim();
      }
      // We have first name, middle name, and last name
      else
      {
        FirstName = _name[..firstIndex].Trim();
        MiddleName = NormalizeMiddleName(_name[(firstIndex + 1)..lastIndex]);
        LastName = _name[(lastIndex + 1)..].Trim();
      }
    }
  }

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
  /// First name of this node.
  /// </summary>
  [JsonIgnore]
  public string FirstName { get; private set; } = string.Empty;

  /// <summary>
  /// Middle name of this node.
  /// </summary>
  [JsonIgnore]
  public string MiddleName { get; private set; } = string.Empty;

  /// <summary>
  /// Last name of this node.
  /// </summary>
  [JsonIgnore]
  public string LastName { get; private set; } = string.Empty;

  /// <summary>
  /// Birth date of this node.
  /// </summary>
  [JsonConverter(typeof(DateOnlyJsonConverter))]
  public DateOnly? BirthDate { get; init; }

  /// <summary>
  /// Death date of this node.
  /// </summary>
  [JsonConverter(typeof(DateOnlyJsonConverter))]
  public DateOnly? DeathDate { get; init; }

  /// <summary>
  /// Profile photo of this node. This is an
  /// URL link to where the photo is located.
  /// </summary>
  public string? Photo { get; init; }

  private static string NormalizeMiddleName(string middleName)
  {
    var tokens = middleName.Split(' ');
    var normalizedTokens = tokens
      .Select(word => word.Trim())
      .Where(word => !string.IsNullOrWhiteSpace(word));

    return string.Join(' ', normalizedTokens);
  }
}
