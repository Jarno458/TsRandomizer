using System;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo
	{
		static readonly Type InventoryItemType = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
		static readonly MethodInfo GetIconFromUseItemMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
		static readonly MethodInfo GetIconFromOrbMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
		static readonly MethodInfo GetIconFromRelicMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
		static readonly MethodInfo GetIconFromEnquipmentMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
		static readonly MethodInfo GetIconFromFamilierMethod = InventoryItemType
			.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));
		
		public static ItemInfo Dummy = new ItemInfo(EInventoryEquipmentType.DemonHorn);

		public LootType LootType { get; protected set; }
		public int ItemId { get; protected set; }
		public int ItemSubId { get; protected set; }

		public Enum TreasureLootType => LootType.ToETreasureLootType();

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public EOrbSlot OrbSlot => (EOrbSlot)ItemSubId;

		public int AnimationIndex => GetAnimationIndex();

		public ItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.UseItem;
			ItemId = (int)useItem;
		}

		public ItemInfo(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		public ItemInfo(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		public ItemInfo(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType;
			ItemSubId = (int)orbSlot;
		}

		public ItemInfo(EInventoryFamiliarType familiar)
		{
			LootType = LootType.Familiar;
			ItemId = (int)familiar;
		}

		int GetAnimationIndex()
		{
			switch (LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.Invoke(null, new object[] {OrbType, OrbSlot});
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.Invoke(null, new object[] { Enquipment });
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.Invoke(null, new object[] { Familiar });
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.Invoke(null, new object[] { Relic });
				case LootType.ConstStat:
					return -1;
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.Invoke(null, new object[] { UseItem });
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}
	}
}