using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Items.ItemDropPickup")]
	// ReSharper disable once UnusedMember.Global
	class ItemDropPickup : ItemManipulator
	{
		bool hasDroppedLoot;

		public ItemDropPickup(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			if (ItemInfo == null)
				return;

			if (IsPickedUp)
			{
				Object.Kill();
			}
			else
			{
				Object.IsFound = false;
				hasDroppedLoot = false;
			}
			
			Object._itemData = ItemInfo.BestiaryItemDropSpecification;
			Object._category = ItemInfo.Identifier.LootType.ToEInventoryCategoryType();

			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstRelic:
					Object._relicType = ItemInfo.Identifier.Relic;
					break;

				case LootType.ConstUseItem:
					Object._useItemType = ItemInfo.Identifier.UseItem;
					break;

				case LootType.ConstEquipment:
					Object._equipmentType = ItemInfo.Identifier.Enquipment;
					break;

				case LootType.ConstStat:
				case LootType.ConstOrb:
				case LootType.ConstFamiliar:
					Object._category = EInventoryCategoryType.Equipment;
					Object._equipmentType = EInventoryEquipmentType.None;
					break;

				//TODO orb 
				//TODO familier

				default:
					throw new NotImplementedException($"LoottType {ItemInfo.Identifier.LootType} is not supported by ItemDropPickup");
			}

			Object.ChangeAnimation(ItemInfo.AnimationIndex);
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasDroppedLoot || !Object.IsFound)
				return;

			// ReSharper disable once PossibleNullReferenceException
			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstOrb:
				case LootType.ConstFamiliar:
				case LootType.ConstStat:
					AwardContainedItem();
					UndoBaseGameAwardedEnquipment(gameplayScreen);
					ShowItemAwardPopup();
					break;

				case LootType.ConstRelic:
				case LootType.ConstUseItem:
				case LootType.ConstEquipment:
					OnItemPickup(); //awarding handled by base game
					break;

				default:
					throw new NotImplementedException($"LoottType {ItemInfo.Identifier.LootType} is not supported by ItemDropPickup");
			}

			hasDroppedLoot = true;
		}

		void UndoBaseGameAwardedEnquipment(GameplayScreen gameplayScreen)
		{
			Level.GameSave.Inventory.EquipmentInventory.RemoveItem((int)EInventoryEquipmentType.None, 1);
			gameplayScreen.HideItemPickupBar();
		}
	}
}
