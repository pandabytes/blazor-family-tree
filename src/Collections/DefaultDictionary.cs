namespace Blazor.FamilyTreeJS.Collections;

/// <summary>
/// Default dictionary similar to Python's defaultdict.
/// https://docs.python.org/3/library/collections.html#collections.defaultdict
/// </summary>
internal class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
  where TKey : notnull
{
  private readonly Func<TValue> _createDefaultValue;

  public DefaultDictionary(Func<TValue> createDefaultValue) : base()
    => _createDefaultValue = createDefaultValue;

  public DefaultDictionary(Func<TValue> createDefaultValue, IDictionary<TKey, TValue> dict) : base(dict)
    => _createDefaultValue = createDefaultValue;

  public new TValue this[TKey key]
  {
		get
    {
			if (!ContainsKey(key))
      {
			  Add(key, _createDefaultValue());
      }
			return base[key];
		}
		set => base[key] = value;
	}	
}
