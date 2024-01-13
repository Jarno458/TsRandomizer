using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	public enum CustomItemType 
	{
		ArchipelagoItem,
		MeteorSparrowTrap,
		NeurotoxinTrap,
		ChaosTrap,
		PoisonTrap,
		BeeTrap,
		TimewornWarpBeacon,
		ModernWarpBeacon,
		MysteriousWarpBeacon,
		LaserAccessA,
		LaserAccessI,
		LaserAccessM
	}

	// consider beacons as starting items

	abstract class CustomItem : SingleItemInfo
	{
		const int Offset = 500;

		protected static string GetNameKey(CustomItemType itemType) => $"inv_use_{(int)itemType + Offset}";

		protected static string GetDescriptionKey(CustomItemType itemType) => $"inv_use_{(int)itemType + Offset}_desc";

		public static void SetDescription(CustomItemType type, string description, string speaker) =>
			TimeSpinnerGame.Localizer.OverrideKey(GetDescriptionKey(type), description, speaker);

		public static ItemIdentifier GetIdentifier(CustomItemType itemType) =>
			new ItemIdentifier((EInventoryUseItemType)itemType + Offset);

		public static void Initialize()
		{
			foreach (CustomItemType customItemType in Enum.GetValues(typeof(CustomItemType)))
			{
				var name = GetName(customItemType);

				TimeSpinnerGame.Localizer.OverrideKey(GetNameKey(customItemType), name);
				TimeSpinnerGame.Localizer.OverrideKey(GetDescriptionKey(customItemType), name, name);
			}
		}

		public static List<ItemInfo> GetAllCustomItems(ItemUnlockingMap unlockingMap)
		{
			var items = new List<ItemInfo>(Enum.GetValues(typeof(CustomItemType)).Length);

			var customItemType = typeof(CustomItem);

			var customItemTypes = customItemType.Assembly.GetTypes()
				.Where(t => customItemType.IsAssignableFrom(t)
				            && !t.IsAbstract
				            && !t.IsGenericType
				            && t.GetConstructors().
					            Any(c => c.GetParameters().Length == 1
					                     && c.GetParameters()[0].ParameterType == typeof(ItemUnlockingMap)));

			foreach (var itemType in customItemTypes)
				items.Add((ItemInfo)itemType.CreateInstance(args: unlockingMap));

			return items;
		}

		public override int AnimationIndex => 28; //purple star, default for custom items, otherwise they be invisible (-1)

		static string GetName(CustomItemType itemType) => string.Join(" ", Regex.Split(itemType.ToString(), @"(?<!^)(?=[A-Z])"));
		
		protected readonly CustomItemType ItemType;

		public string Name => GetName(ItemType);

		protected virtual bool RemoveFromInventory => true;

		protected CustomItem(ItemUnlockingMap unlockingMap, CustomItemType itemType) 
			: base(unlockingMap, GetIdentifier(itemType))
		{
			ItemType = itemType;
		}

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
