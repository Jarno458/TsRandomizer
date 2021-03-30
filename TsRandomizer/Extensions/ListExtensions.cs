using System;
using System.Collections.Generic;

namespace TsRandomizer.Extensions
{
	static class ListExtensions
	{
		internal static T SelectRandom<T>(this IList<T> items, Random r)
		{
			return items[r.Next(items.Count)];
		}

		internal static T PopRandom<T>(this IList<T> items, Random r)
		{
			var index = r.Next(items.Count);
			T result = items[index];
			items.RemoveAt(index);

			return result;
		}
	}
}
