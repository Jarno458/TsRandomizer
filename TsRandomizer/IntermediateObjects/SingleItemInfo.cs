using System;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.IntermediateObjects
{
	public class SingleItemInfo : ItemInfo
	{
		static readonly MethodInfo GetIconFromUseItemMethod;
		static readonly MethodInfo GetIconFromOrbMethod;
		static readonly MethodInfo GetIconFromRelicMethod;
		static readonly MethodInfo GetIconFromEnquipmentMethod;
		static readonly MethodInfo GetIconFromFamilierMethod;

		public static ItemInfo Dummy;

		static SingleItemInfo()
		{
			var inventoryItemType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
			GetIconFromUseItemMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
			GetIconFromOrbMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
			GetIconFromRelicMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
			GetIconFromEnquipmentMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
			GetIconFromFamilierMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));

			Dummy = new SingleItemInfo(EInventoryEquipmentType.DemonHorn);
		}

		public override LootType LootType { get; }
		public override int ItemId { get; }
		public override EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public override EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public override EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public override EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public override EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public override EOrbSlot OrbSlot { get; }
		public override EItemType Stat => (EItemType)ItemId;

		public override Enum TreasureLootType => LootType.ToETreasureLootType();
		public override int AnimationIndex => GetAnimationIndex();
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => GetBestiaryItemDropSpecification();

		Action<Level> PickupAction { get; set; }

		public SingleItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.UseItem;
			ItemId = (int)useItem;
		}

		public SingleItemInfo(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		public SingleItemInfo(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		public SingleItemInfo(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType;
			OrbSlot = orbSlot;
		}

		public SingleItemInfo(EInventoryFamiliarType familiar)
		{
			LootType = LootType.Familiar;
			ItemId = (int)familiar;
		}

		public SingleItemInfo(EItemType stat)
		{
			LootType = LootType.Stat;
			ItemId = (int)stat;
		}

		public override void SetPickupAction(Action<Level> onPickUp)
		{
			PickupAction = onPickUp;
		}

		public override void OnPickup(Level level)
		{
			PickupAction?.Invoke(level);
		}

		int GetAnimationIndex()
		{
			switch (LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.InvokeStatic(OrbType, OrbSlot) - 1;
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.InvokeStatic(Enquipment) - 1;
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.InvokeStatic(Familiar) - 1;
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.InvokeStatic(Relic) - 1;
				case LootType.ConstStat:
					return (int)GetIconFromStat(Stat);
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(UseItem) - 1;
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}

		object GetIconFromStat(EItemType itemType)
		{
			switch (itemType)
			{
				case EItemType.MaxHP:
					return 24;
				case EItemType.MaxAura:
					return 25;
				case EItemType.MaxSand:
					return 26;
				default:
					throw new ArgumentOutOfRangeException($"Stat {Stat} isnt a valid stat boost type");
			}
		}

		BestiaryItemDropSpecification GetBestiaryItemDropSpecification()
		{
			return new BestiaryItemDropSpecification
			{
				Category = (int)LootType.ToEInventoryCategoryType(),
				Item = ItemId
			};
		}
	}
}