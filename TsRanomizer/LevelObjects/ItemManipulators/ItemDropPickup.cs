using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;
using TsRanodmizer.Screens;

namespace TsRanodmizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Items.ItemDropPickup")]
	// ReSharper disable once UnusedMember.Global
	class ItemDropPickup : ItemManipulator
	{
		bool hasDroppedLoot;

		public ItemDropPickup(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize()
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
			Object._category = ItemInfo.LootType.ToEInventoryCategoryType();

			switch (ItemInfo.LootType)
			{
				case LootType.ConstRelic:
					Object._relicType = ItemInfo.Relic;
					break;

				case LootType.ConstUseItem:
					Object._useItemType = ItemInfo.UseItem;
					break;

				case LootType.ConstEquipment:
					Object._equipmentType = ItemInfo.Enquipment;
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
					throw new NotImplementedException($"LoottType {ItemInfo.LootType} is not supported by ItemDropPickup");
			}

			Object.ChangeAnimation(ItemInfo.AnimationIndex);
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasDroppedLoot || !Object.IsFound)
				return;

			// ReSharper disable once PossibleNullReferenceException
			switch (ItemInfo.LootType)
			{
				case LootType.ConstOrb:
				case LootType.ConstFamiliar:
					AwardContainedItem();
					UndoBaseGameAwardedEnquipment(gameplayScreen);
					Level.AddScript(RelicOrOrbGetPopup());
					break;

				case LootType.ConstStat:
					AwardContainedItem();
					UndoBaseGameAwardedEnquipment(gameplayScreen);
					Level.RequestToastPopupForStats(ItemInfo);
					break;

				case LootType.ConstRelic:
				case LootType.ConstUseItem:
				case LootType.ConstEquipment:
					OnItemPickup(); //awarding handled by base game
					break;

				default:
					throw new NotImplementedException($"LoottType {ItemInfo.LootType} is not supported by ItemDropPickup");
			}

			hasDroppedLoot = true;
		}

		ScriptAction RelicOrOrbGetPopup()
		{
			switch (ItemInfo.LootType)
			{
				case LootType.ConstOrb:
					return (ScriptAction) typeof(ScriptAction).CreateInstance(true, ItemInfo.OrbType, ItemInfo.OrbSlot);
				case LootType.ConstFamiliar:
					return (ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Familiar);
				case LootType.ConstRelic:
					return (ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Relic);
				case LootType.ConstEquipment:
					return (ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Enquipment);
				case LootType.ConstUseItem:
					return (ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.UseItem, 1);
				default:
					throw new NotImplementedException($"Script action is not implemented for LootType {ItemInfo.LootType}");
			}
		}

		void UndoBaseGameAwardedEnquipment(GameplayScreen gameplayScreen)
		{
			Level.GameSave.Inventory.EquipmentInventory.RemoveItem((int)EInventoryEquipmentType.None, 1);
			gameplayScreen.HideItemPickupBar();
		}
	}
}
