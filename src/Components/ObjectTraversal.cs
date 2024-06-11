using System.Collections;
using System.Reflection;

namespace Blazor.FamilyTreeJS.Components;

/// <summary>
/// Purpose of this class is to
/// traverse an object hierarchy.
/// </summary>
internal class ObjectTraversal
{
  private readonly ISet<object> _traversedObjs;

  public ObjectTraversal()
  {
    _traversedObjs = new HashSet<object>();
  }

  /// <summary>
  /// Find all objects in <paramref name="obj"/> recursively
  /// that satisfy <paramref name="condition"/>. 
  /// Null, indexer, and System.Reflection.* properties will be skipped.
  /// This method will only look at public read properties.
  /// </summary>
  /// <param name="obj">Root object.</param>
  /// <param name="condition">Condition.</param>
  /// <returns>Objects that satisfy <paramref name="condition"/>.</returns>
  public IEnumerable<object> FindRecursively(object obj, Predicate<object> condition)
  {
    _traversedObjs.Clear();
    var collectedObjs = new List<object>();

    FindRecursively(obj, condition, collectedObjs);
    return collectedObjs;
  }

  private void FindRecursively(object obj, Predicate<object> condition, IList<object> collectedObjs)
  {
    if (_traversedObjs.Contains(obj))
    {
      return;
    }

    if (condition.Invoke(obj))
    {
      collectedObjs.Add(obj);
    }

    _traversedObjs.Add(obj);

    var properties = obj
      .GetType()
      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
      // Ignore indexers
      .Where(property => !property.GetIndexParameters().Any())
      // Ignore type from the System.Reflection namespace
      .Where(property => !property.PropertyType?.Namespace?.StartsWith("System.Reflection") ?? true);

    foreach (var property in properties)
    {
      var propValue = property.GetValue(obj, null);
      if (propValue is null)
      {
        continue;
      }

      if (IsEnumerable(property.PropertyType))
      {
        foreach (var item in (IEnumerable)propValue)
        {
          FindRecursively(item, condition, collectedObjs);
        }
      }
      else
      {
        FindRecursively(propValue, condition, collectedObjs);
      }
    }
  }

  /// <summary>
  /// Check if given type is enumerable, aka can
  /// we iterate the object with the given type.
  /// </summary>
  /// <remarks>
  /// This method checks if type implements <see cref="IEnumerable{T}"/>
  /// </remarks>
  /// <param name="type">Type</param>
  /// <returns>True if type is enumerable. False otherwise</returns>
  private static bool IsEnumerable(Type type)
  {
    var interfaces = type.GetInterfaces();
    return interfaces.Any(interf => interf == typeof(IEnumerable));
  }
}
