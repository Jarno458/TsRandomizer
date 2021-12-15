using System;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	class TreasureChest : ItemManipulator<TreasureChestEvent>
	{
		static readonly Type ETreasureStatBoostType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.TreasureChestEvent+ETreasureStatBoostType");

		bool hasDroppedLoot;

		public TreasureChest(TreasureChestEvent treasureChest, ItemLocation itemLocation) : base(treasureChest, itemLocation)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			if (ItemInfo == null)
				return;

			Dynamic._treasureLootType = ItemInfo.TreasureLootType;

			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstUseItem:
					Dynamic._lootUseItemType = ItemInfo.Identifier.UseItem;
					break;
				case LootType.ConstRelic:
					Dynamic._lootRelicType = ItemInfo.Identifier.Relic;
					break;
				case LootType.ConstEquipment:
					Dynamic._lootEquipmentType = ItemInfo.Identifier.Equipment;
					break;
				case LootType.ConstOrb:
					Dynamic._lootOrbType = ItemInfo.Identifier.OrbType;
					Dynamic._lootOrbSlot = ItemInfo.Identifier.OrbSlot;
					break;
				case LootType.ConstFamiliar:
					Dynamic._lootFamiliarType = ItemInfo.Identifier.Familiar;
					break;
				case LootType.ConstStat:
					switch (ItemInfo.Identifier.Stat)
					{
						case EItemType.MaxHP:
							Dynamic._lootStatBoostType = ETreasureStatBoostType.GetEnumValue("HP");
							break;
						case EItemType.MaxAura:
							Dynamic._lootStatBoostType = ETreasureStatBoostType.GetEnumValue("Aura");
							break;
						case EItemType.MaxSand:
							Dynamic._lootStatBoostType = ETreasureStatBoostType.GetEnumValue("Sand");
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ItemInfo.Identifier.LootType), ItemInfo.Identifier.LootType, $"lootType cannot be droppd by {nameof(TreasureChest)}");
			}

			var pickedUp = IsPickedUp;
			Dynamic._isOpened = pickedUp;
			Dynamic._hasDroppedLoot = pickedUp;
			hasDroppedLoot = pickedUp;
			if (pickedUp)
				((Appendage)Dynamic._lidAppendage).AsDynamic().ChangeAnimation(Dynamic._animationIndexOffset + 5, 1, 1f, EAnimationType.None);
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasDroppedLoot || !Dynamic._hasDroppedLoot)
				return;
		
			// ReSharper disable once PossibleNullReferenceException
			if (ItemInfo.Identifier.LootType == LootType.Orb || ItemInfo.Identifier.LootType == LootType.Familiar)
				Level.GameSave.AddItem(Level, ItemInfo.Identifier);

			OnItemPickup();

			hasDroppedLoot = true;
		}
	}
}
