using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TsRandomizer
{
	class LookupDictionary<TLookup, TValue> : IEnumerable<TValue>
	{
		readonly Func<TValue, TLookup> keySelector;
		readonly Dictionary<TLookup, TValue> lookupTable;

		public LookupDictionary(Func<TValue, TLookup> keySelector) : this(0, keySelector)
		{
		}

		public LookupDictionary(int capacity, Func<TValue, TLookup> keySelector)
		{
			lookupTable = new Dictionary<TLookup, TValue>(capacity);
			this.keySelector = keySelector;
		}

		public TValue this[TLookup key] => lookupTable[key];

		public int Count => lookupTable.Count;

		public bool TryGetValue(TLookup key, out TValue value) =>
			lookupTable.TryGetValue(key, out value);

		public void Filter(IEnumerable<TLookup> intersectionFilter, Action<TValue> removeAction = null)
		{
			var keysToRemove = lookupTable.Keys
				.Where(e => !intersectionFilter.Contains(e))
				.ToList();

			foreach (var key in keysToRemove)
			{
				removeAction?.Invoke(lookupTable[key]);
				lookupTable.Remove(key);
			}
		}

		public IEnumerator<TValue> GetEnumerator() =>
			((IEnumerable<TValue>)lookupTable.Values).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			lookupTable.Values.GetEnumerator();

		public void Add(TValue value) =>
			lookupTable.Add(keySelector(value), value);

		public void AddOrUpdate(TValue value) =>
			lookupTable[keySelector(value)] = value;

		public bool Contains(TLookup key) =>
			lookupTable.ContainsKey(key);
	}
}
