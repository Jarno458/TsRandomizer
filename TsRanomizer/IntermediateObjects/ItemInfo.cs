using System;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo
	{
		public static ItemInfo Dummy =
				new ItemInfo(EInventoryEquipmentType.DemonHorn);

		public LootType LootType { get; protected set; }
		public int ItemId { get; protected set; }
		public bool Obtained { get; protected set; }

		public Enum TreasureLootType => LootType.ToETreasureLootType();

		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Emquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)(ItemId % 100);
		public EOrbSlot OrbSlot => (EOrbSlot)(ItemId / 100);

		protected ItemInfo() { }

		public ItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.ConstUseItem;
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
			ItemId = (int)orbType + 100 * (int)orbSlot;

			//if(orbSlot != EOrbSlot.Melee)
			//    PrerequisiteItem = new ItemInfo(orbType, EOrbSlot.Melee);
		}

		public virtual void IsObtained()
		{
			Obtained = true;
		}

		public static ItemInfo FromProgressionItem(ProgressionItem item)
		{
			if (!item.HasSingleItem())
				throw new ArgumentOutOfRangeException(nameof(item), item, "ProgressionItem contains multiple flags");

			switch (item)
			{
				case ProgressionItem.ConstTimeStop:
					return new ItemInfo(EInventoryRelicType.TimespinnerWheel);
				case ProgressionItem.ConstDoubleJump:
					return new ItemInfo(EInventoryRelicType.DoubleJump);
				case ProgressionItem.ConstForwardDash:
					return new ItemInfo(EInventoryRelicType.Dash);
				case ProgressionItem.ConstUpwardDash:
					return new ItemInfo(EInventoryRelicType.EssenceOfSpace);
				case ProgressionItem.ConstLightwall:
					return new ItemInfo(EInventoryOrbType.Barrier, EOrbSlot.Spell);
				case ProgressionItem.ConstAntiWeed:
					return new ItemInfo(EInventoryOrbType.Flame, EOrbSlot.Passive);
				case ProgressionItem.ConstCardA:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardA);
				case ProgressionItem.ConstCardB:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardB);
				case ProgressionItem.ConstCardC:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardC);
				case ProgressionItem.ConstCardD:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardD);
				case ProgressionItem.ConstCardE:
					return new ItemInfo(EInventoryRelicType.ElevatorKeycard);
				case ProgressionItem.ConstCardV:
					return new ItemInfo(EInventoryRelicType.ScienceKeycardV);
				default:
					throw new ArgumentOutOfRangeException(nameof(item), item, "ProgressionItem not mapped to ItemInfo item");
			}
		}
	}
}