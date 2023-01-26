using System;
using System.Text.RegularExpressions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	public enum CustomItemType 
	{
		Archipelago,
		MeteorSparrowTrap,
		NeurotoxinTrap,
		ChaosTrap,
		PoisonTrap,
		BeeTrap,
		TimewornWarpBeacon,
		ModernWarpBeacon,
		MysteriousWarpBeacon
	}

	abstract class CustomItem : SingleItemInfo
	{
		const int Offset = 500;

		protected static string GetNameKey(CustomItemType itemType) => $"inv_use_{(int)itemType + Offset}";

		protected static string GetDescriptionKey(CustomItemType itemType) => $"inv_use_{(int)itemType + Offset}_desc";

		public static ItemIdentifier GetIdentifier(CustomItemType itemType) =>
			new ItemIdentifier((EInventoryUseItemType)itemType + Offset);

		static CustomItem()
		{
			foreach (CustomItemType customItemType in Enum.GetValues(typeof(CustomItemType)))
			{
				var name = GetName(customItemType);

				TimeSpinnerGame.Localizer.OverrideKey(GetNameKey(customItemType), name);
				TimeSpinnerGame.Localizer.OverrideKey(GetDescriptionKey(customItemType), name, name);
			}
		}

		static string GetName(CustomItemType itemType) => string.Join(" ", Regex.Split(itemType.ToString(), @"(?<!^)(?=[A-Z])"));
		
		protected readonly CustomItemType ItemType;

		public virtual string Name => GetName(ItemType);

		protected virtual bool RemoveFromInventory => true;

		protected CustomItem(ItemUnlockingMap unlockingMap, CustomItemType itemType) 
			: base(unlockingMap, GetIdentifier(itemType))
		{
			ItemType = itemType;
		}

		public static void SetDescription(CustomItemType type, string description, string speaker) =>
			TimeSpinnerGame.Localizer.OverrideKey(GetDescriptionKey(type), description, speaker);

		protected void SetDescription(string description, string speaker) => SetDescription(ItemType, description, speaker);

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			gameplayScreen.ShowItemPickupBar(Name);

			if (RemoveFromInventory)
				level.GameSave.Inventory.UseItemInventory.RemoveItem((int)Identifier.UseItem, 999);
		}
	}
}
