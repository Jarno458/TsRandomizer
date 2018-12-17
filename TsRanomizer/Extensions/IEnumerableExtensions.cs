using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TsRanodmizer.Extensions
{
	public static class EnumerableExtensions
	{
		static readonly Type EnumerableType = typeof(Enumerable);

		internal static IEnumerable CastAsType(this IEnumerable source, Type targetType)
		{
			var castMethod = EnumerableType.GetMethod("Cast")?.MakeGenericMethod(targetType);

			return (IEnumerable)castMethod?.Invoke(null, new object[] { source });
		}

		internal static IList ToListOfType(this IEnumerable source, Type targetType)
		{
			var enumerable = CastAsType(source, targetType);

			var listMethod = EnumerableType.GetMethod("ToList")?.MakeGenericMethod(targetType);

			return (IList)listMethod?.Invoke(null, new object[] { enumerable });
		}

		internal static IEnumerable<T> Append<T>(this IEnumerable<T> items, T item)
		{
			return items.Concat(new[] {item});
		}
	}
}
