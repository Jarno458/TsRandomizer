using System;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;

namespace TsRandomizer.IntermediateObjects
{
	public class SingleItemInfo : ItemInfo
	{
		static readonly MethodInfo GetIconFromUseItemMethod;
		static readonly MethodInfo GetIconFromOrbMethod;
		static readonly MethodInfo GetIconFromRelicMethod;
		static readonly MethodInfo GetIconFromEnquipmentMethod;
		static readonly MethodInfo GetIconFromFamilierMethod;

		static SingleItemInfo()
		{
			var inventoryItemType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
			GetIconFromUseItemMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
			GetIconFromOrbMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
			GetIconFromRelicMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
			GetIconFromEnquipmentMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
			GetIconFromFamilierMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));
		}

		public override ItemIdentifier Identifier { get; }
		internal override Requirement Unlocks { get; }

		public override Enum TreasureLootType => Identifier.LootType.ToETreasureLootType();
		public override int AnimationIndex => GetAnimationIndex();
		public override BestiaryItemDropSpecification BestiaryItemDropSpecification => GetBestiaryItemDropSpecification();

		Action<Level> PickupAction { get; }

		public SingleItemInfo(ItemIdentifier identifier)
		{
			Identifier = identifier;
		}

		internal SingleItemInfo(ItemUnlockingMap unlockingMap, ItemIdentifier identifier)
		{
			Identifier = identifier;
			Unlocks = unlockingMap.GetAllUnlock(identifier);
			PickupAction = unlockingMap.GetPickupAction(identifier);
		}

		public override void OnPickup(Level level)
		{
			PickupAction?.Invoke(level);
		}

		int GetAnimationIndex()
		{
			switch (Identifier.LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.InvokeStatic(Identifier.OrbType, Identifier.OrbSlot) - 1;
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.InvokeStatic(Identifier.Enquipment) - 1;
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.InvokeStatic(Identifier.Familiar) - 1;
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.InvokeStatic(Identifier.Relic) - 1;
				case LootType.ConstStat:
					return (int)GetIconFromStat(Identifier.Stat);
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(Identifier.UseItem) - 1;
				default:
					throw new ArgumentOutOfRangeException($"LootType {Identifier.LootType} isnt a valid loot type");
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
					throw new ArgumentOutOfRangeException($"Stat {Identifier.Stat} isnt a valid stat boost type");
			}
		}

		BestiaryItemDropSpecification GetBestiaryItemDropSpecification()
		{
			return new BestiaryItemDropSpecification
			{
				Category = (int)Identifier.LootType.ToEInventoryCategoryType(),
				Item = Identifier.ItemId
			};
		}
	}
}