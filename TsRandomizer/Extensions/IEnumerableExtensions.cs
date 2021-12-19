using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TsRandomizer.Extensions
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

		internal static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) => items.Concat(new[] {item});

		public static IEnumerable<T> NotOfType<T,T2>(this IEnumerable<T> items) => items.Where(i => i.GetType() != typeof(T2));

		internal static T SelectRandom<T>(this IEnumerable<T> items, Random r)
		{
			var array = items.ToArray();
			return array.SelectRandom(r);
		}

		internal static IOrderedEnumerable<T> InRandomOrder<T>(this IEnumerable<T> items, Random r) => items.OrderBy(i => r.Next());

		internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new HashSet<T>(items);
	}
}
