using System.Text.Json.Serialization;
using Blazor.FamilyTreeJS.Interop.Converters;

namespace Blazor.FamilyTreeJS.Interop.Options;

public record Node
{
  private string _name = string.Empty;

  public required string Id { get; init; }

  [JsonConverter(typeof(GenderConverter))]
  public required Gender Gender { get; init; }

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
        return;
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

  [JsonPropertyName("pids")]
  public IReadOnlyList<string>? PartnerIds { get; init; }

  [JsonPropertyName("fid")]
  public string? FatherId { get; init; }

  [JsonPropertyName("mid")]
  public string? MotherId { get; init; }

  [JsonIgnore]
  public string FirstName { get; private set; } = string.Empty;

  [JsonIgnore]
  public string MiddleName { get; private set; } = string.Empty;

  [JsonIgnore]
  public string LastName { get; private set; } = string.Empty;

  [JsonConverter(typeof(DateOnlyJsonConverter))]
  public DateOnly? BirthDate { get; init; }

  [JsonConverter(typeof(DateOnlyJsonConverter))]
  public DateOnly? DeathDate { get; init; }

  public string? Photo { get; init; }

  private static string NormalizeMiddleName(string middleName)
  {
    var tokens = middleName.Split(" ");
    var normalizedTokens = tokens
      .Select(c => c.Trim())
      .Where(c => !string.IsNullOrWhiteSpace(c));

    return string.Join(' ', normalizedTokens);
  }
}
