namespace Blazor.FamilyTreeJS.Components.Interop.Elements;

/// <summary>
/// Extension methods for <see cref="BaseNode"/>.
/// </summary>
public static class BaseNodeExtension
{
  /// <summary>
  /// Provide a dynamic way to get a <see cref="BaseNode"/>'s property value.
  /// </summary>
  /// <param name="node">Node object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <typeparam name="T">Type of the property value.</typeparam>
  /// <returns>Property value.</returns>
  public static T Get<T>(this BaseNode node, string propertyName)
  {
    var type = node.GetType();
    var property = type.GetProperty(propertyName) ??
      throw new ArgumentException($"Failed to get value of property \"{propertyName}\".");

    var value = property.GetValue(node);
    return (T)value!;
  }
}