using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	struct ProgressionItem : IEquatable<ProgressionItem>
	{
		public const ulong ConstNone = 0;
		public const ulong ConstTimeStop = 1 << 0;
		public const ulong ConstDoubleJump = 1 << 1;
		public const ulong ConstForwardDash = 1 << 2;
		public const ulong ConstUpwardDash = 1 << 3;
		public const ulong ConstLightwall = 1 << 4;
		public const ulong ConstAntiWeed = 1 << 5;
		public const ulong ConstCardA = 1 << 6;
		public const ulong ConstCardB = 1 << 7;
		public const ulong ConstCardC = 1 << 8;
		public const ulong ConstCardD = 1 << 9;
		public const ulong ConstCardE = 1 << 10;
		public const ulong ConstCardV = 1 << 11;
		public const ulong ConstTimespinnerSpindle = 1 << 12;
		public const ulong ConstSwimming = 1 << 13;

		public static readonly ProgressionItem None = ConstNone;
		public static readonly ProgressionItem TimeStop = ConstTimeStop;
		public static readonly ProgressionItem DoubleJump = ConstDoubleJump;
		public static readonly ProgressionItem ForwardDash = ConstForwardDash;
		public static readonly ProgressionItem UpwardDash = ConstUpwardDash;
		public static readonly ProgressionItem Lightwall = ConstLightwall;
		public static readonly ProgressionItem AntiWeed = ConstAntiWeed;
		public static readonly ProgressionItem CardA = ConstCardA;
		public static readonly ProgressionItem CardB = ConstCardB;
		public static readonly ProgressionItem CardC = ConstCardC;
		public static readonly ProgressionItem CardD = ConstCardD;
		public static readonly ProgressionItem CardE = ConstCardE;
		public static readonly ProgressionItem CardV = ConstCardV;
		public static readonly ProgressionItem TimespinnerSpindle = ConstTimespinnerSpindle;
		public static readonly ProgressionItem Swimming = ConstSwimming;
		
		readonly ulong flags;

		ProgressionItem(ulong flags)
		{
			this.flags = flags;
		}

		[Pure]
		public bool HasMatchingFlag(ProgressionItem other)
		{
			return (flags & other.flags) > 0;
		}

		[Pure]
		public bool HasSingleItem()
		{
			return flags != 0 && (flags & (flags - 1)) == 0;
		}


		[Pure]
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is ProgressionItem item && Equals(item);
		}

		[Pure]
		public bool Equals(ProgressionItem other)
		{
			return flags == other.flags;
		}

		[Pure]
		public override int GetHashCode()
		{
			return flags.GetHashCode();
		}

		public static implicit operator ProgressionItem(ulong value)
		{
			return new ProgressionItem(value);
		}

		public static implicit operator ulong(ProgressionItem value)
		{
			return value.flags;
		}

		public static implicit operator ItemInfo(ProgressionItem value)
		{
			return ItemInfo.FromProgressionItem(value);
		}

		public static ProgressionItem operator |(ProgressionItem a, ProgressionItem b)
		{
			return a.flags | b.flags;
		}

		public static Gate operator &(ProgressionItem a, ProgressionItem b)
		{
			return (Gate)a & b;
		}

		public static bool operator ==(ProgressionItem a, ProgressionItem b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ProgressionItem a, ProgressionItem b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return GetFlagNames(flags);
		}

		static string GetFlagNames(ulong flags)
		{
			if (flags == None) return "";

			var flagNames = typeof(ProgressionItem)
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.Where(f =>
					!f.Name.StartsWith("Const")
					&& ((ulong)(ProgressionItem)f.GetValue(null) & flags) > 0)
				.Select(f => f.Name);

			return string.Join(" | ", flagNames);
		}
	}
}