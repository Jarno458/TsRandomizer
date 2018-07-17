using System;
using System.Diagnostics.Contracts;

namespace TsRanodmizer.IntermediateObjects
{
	struct LootType
	{
		public const int ConstUseItem = 0;
		public const int ConstRelic = 1;
		public const int ConstStat = 2;
		public const int ConstEquipment = 3;
		public const int ConstOrb = 4;
		public const int ConstFamiliar = 5;

		public static readonly LootType UseItem = ConstUseItem;
		public static readonly LootType Relic = ConstRelic;
		public static readonly LootType Stat = ConstStat;
		public static readonly LootType Equipment = ConstEquipment;
		public static readonly LootType Orb = ConstOrb;
		public static readonly LootType Familiar = ConstFamiliar;

		static readonly Type ETreasureLootType = TimeSpinnerType
			.Get("Timespinner.GameObjects.Events.ETreasureLootType");

		readonly int lootType;

		LootType(int treasureLootType)
		{
			lootType = treasureLootType;
		}

		public static LootType FromETreasureLootType(Enum value)
		{
			return (int)(object)value;
		}

		[Pure]
		public Enum ToETreasureLootType()
		{
			return (Enum)Enum.ToObject(ETreasureLootType, lootType);
		}

		public static implicit operator LootType(int value)
		{
			return new LootType(value);
		}

		public static implicit operator int(LootType value)
		{
			return value.lootType;
		}
	}
}