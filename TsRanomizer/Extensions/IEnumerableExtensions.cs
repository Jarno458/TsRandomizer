using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TsRanodmizer.Extensions
{
	public static class EnumerableExtensions
	{
		static readonly Type EnumerableType = typeof(Enumerable);

		internal static IEnumerable Cast(this IEnumerable source, Type targetType)
		{
			var castMethod = EnumerableType.GetMethod("Cast")?.MakeGenericMethod(targetType);

			return (IEnumerable)castMethod?.Invoke(null, new object[] { source });
		}

		internal static IList ToList(this IEnumerable source, Type targetType)
		{
			var enumerable = Cast(source, targetType);

			var listMethod = EnumerableType.GetMethod("ToList")?.MakeGenericMethod(targetType);

			return (IList)listMethod?.Invoke(null, new object[] { enumerable });
		}

		internal static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
		{
			return items.Concat(new[] {item});
		}

		internal static T SelectRandom<T>(this IEnumerable<T> items, Random r)
		{
			var array = items.ToArray();
			return array.SelectRandom(r);
		}

		internal static IOrderedEnumerable<T> InRandomOrder<T>(this IEnumerable<T> items, Random r)
		{
			return items.OrderBy(i => r.Next());
		}
	}
}
