using System.Text.Json.Serialization;

namespace Blazor.FamilyTreeJS.Components.Interop.Options;

/// <summary>
/// Derived from <see cref="BaseNode"/> to include
/// additional properties like Name, Photo, BirthDate
/// and DeathDate.
/// </summary>
public record Node : BaseNode
{
  private string _name = string.Empty;

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

  private static string NormalizeMiddleName(string middleName)
  {
    var tokens = middleName.Split(' ');
    var normalizedTokens = tokens
      .Select(word => word.Trim())
      .Where(word => !string.IsNullOrWhiteSpace(word));

    return string.Join(' ', normalizedTokens);
  }
}
