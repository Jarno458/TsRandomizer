using System;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	struct ProgressionItem : IEquatable<ProgressionItem>
	{
		const ulong ConstNone = 0;
		const ulong ConstTimeStop = 1 << 0;
		const ulong ConstForwardDash = 1 << 1;
		const ulong ConstLightwall = 1 << 2;
		const ulong ConstAntiWeed = 1 << 3;
		const ulong ConstCardA = 1 << 4;
		const ulong ConstCardB = 1 << 5;
		const ulong ConstCardC = 1 << 6;
		const ulong ConstCardD = 1 << 7;
		const ulong ConstCardE = 1 << 8;
		const ulong ConstCardV = 1 << 9;
		const ulong ConstTimespinnerSpindle = 1 << 10;
		const ulong ConstSwimming = 1 << 11;
		const ulong ConstUpwardDash = 1 << 12;
		const ulong ConstDoubleJump = 1 << 13;
		const ulong ConstGassMask = 1 << 14;

		public static readonly ProgressionItem None = ConstNone;
		public static readonly ProgressionItem TimeStop = ConstTimeStop;
		public static readonly ProgressionItem ForwardDash = ConstForwardDash;
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
		public static readonly ProgressionItem UpwardDash = ConstUpwardDash;
		public static readonly ProgressionItem DoubleJump = ConstDoubleJump;


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

		[Pure]
		public static ItemInfo ToItemInfo(ProgressionItem item)
		{
			switch (item)
			{
				case ConstTimeStop:
					return new ItemInfo(EInventoryRelicType.TimespinnerWheel);
				case ConstDoubleJump:
					return new ItemInfo(EInventoryRelicType.DoubleJump);
				case ConstForwardDash:
					return new ItemInfo(EInventoryRelicType.Dash);
				case ConstUpwardDash:
					return new ItemInfo(EInventoryRelicType.EssenceOfSpace);
				case ConstLightwall:
					return new ItemInfo(EInventoryOrbType.Barrier, EOrbSlot.Spell);
				case ConstAntiWeed:
					return new ItemInfo(EInventoryOrbType.Flame, EOrbSlot.Passive);
				case ConstCardA:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardA);
				case ConstCardB:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardB);
				case ConstCardC:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardC);
				case ConstCardD:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardD);
				case ConstCardE:
					return new ItemInfo(EInventoryRelicType.ElevatorKeycard);
				case ConstCardV:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardV);
				case ConstSwimming:
					return new ItemInfo(EInventoryRelicType.WaterMask);
				default:
					throw new ArgumentOutOfRangeException(nameof(item), item, "ProgressionItem not mapped to ItemInfo item");
			}
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
			return ToItemInfo(value);
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

			switch (flags)
			{
				case ConstTimeStop | ConstForwardDash | ConstLightwall | ConstUpwardDash | ConstDoubleJump:
					return "ForwardJumpOfNpc";
				case ConstTimeStop | ConstLightwall | ConstUpwardDash | ConstDoubleJump:
					return "JumpOfNpc";
				case ConstLightwall | ConstUpwardDash | ConstDoubleJump:
					return "DoubleJump";
				case ConstLightwall | ConstUpwardDash:
					return "UpwardDash";
				case ConstCardD | ConstCardC | ConstCardB | ConstCardA:
					return "SecurityAccessD";
				case ConstCardC | ConstCardB | ConstCardA:
					return "SecurityAccessC";
				case ConstCardB | ConstCardA:
					return "SecurityAccessD";
			}

			var flagNames = typeof(ProgressionItem)
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.Where(f => !f.Name.StartsWith("Const") 
				            && ((ulong)(ProgressionItem)f.GetValue(null) & flags) > 0)
				.Select(f => f.Name);

			return string.Join(" | ", flagNames);
		}
	}
}