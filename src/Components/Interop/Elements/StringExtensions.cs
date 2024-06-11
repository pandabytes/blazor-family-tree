namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
  /// <summary>
  /// Return a new string that only has its first letter capitalized.
  /// </summary>
  /// <param name="input">String input.</param>
  /// <returns>First letter capitalized string.</returns>
  public static string ToFirstCharUpper(this string input)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      return string.Empty;
    }
    return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
  }
}
