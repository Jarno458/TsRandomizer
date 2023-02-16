using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects
{
	abstract class ItemManipulator<T> : ItemManipulator where T : Mobile
	{
		public readonly new T TypedObject;

		protected ItemManipulator(T typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation)
			: base(typedObject, gameplayScreen, itemLocation)
		{
			TypedObject = typedObject;
		}
	}

	abstract class ItemManipulator : LevelObject
	{
		static ItemLocationMap itemLocationMap;

		public bool IsPickedUp => ItemLocation.IsPickedUp;

		public readonly ItemInfo ItemInfo;
		public readonly ItemLocation ItemLocation;

		protected ItemManipulator(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen)
		{
			ItemInfo = itemLocation?.ItemInfo;
			ItemLocation = itemLocation;
		}

		protected void AwardContainedItem()
		{
			Level.GameSave.AddItem(Level, ItemInfo.Identifier);

			if (ItemInfo.Identifier.LootType == LootType.ConstRelic)
				LevelReflected.UnlockRelic(ItemInfo.Identifier.Relic);

			OnItemPickup();
		}

		protected void OnItemPickup()
		{
			ItemInfo.OnPickup(Level, GameplayScreen);
			ItemLocation.SetPickedUp(Level);

			if (ItemInfo.IsProgression)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(itemLocationMap));
		}

		public static void Initialize(ItemLocationMap itemLocations) => itemLocationMap = itemLocations;

		public static ItemManipulator GenerateShadowObject(
			Type levelObjectType, Mobile obj, GameplayScreen gameplayScreen, ItemLocationMap itemLocations)
		{
			var itemKey = GetKey(obj);
			var itemLocation = itemLocations[itemKey];

			if (itemLocation == null)
			{
				Console.Out.WriteLine($"UnmappedItem: {itemKey}");
				return null;
			}

			return (ItemManipulator)Activator.CreateInstance(levelObjectType, obj, gameplayScreen, itemLocation);
		}

		protected void ShowItemAwardPopup()
		{
			if (ItemInfo is CustomItem customItem)
				GameplayScreen.ShowItemPickupBar(customItem.Name);
			else if (ItemInfo is ProgressiveItemInfo progressiveItem)
				Level.ShowItemAwardPopup(progressiveItem.PreviousItem.Identifier); //since this script runs delayed, the item will already have updated
			else
				Level.ShowItemAwardPopup(ItemInfo.Identifier);
		}

		protected void UpdateRelicOrbGetToastToItem()
		{
			var scripts = (Queue<ScriptAction>)LevelReflected._waitingScripts;

			var giveOrbScript = scripts.LastOrDefault(s => s.AsDynamic().ScriptType == EScriptType.RelicOrbGetToast);
			if (giveOrbScript == null)
				return;

			var reflectedScript = giveOrbScript.AsDynamic();

			reflectedScript.ScriptType = EScriptType.Delegate;
			reflectedScript.Delegate = (Action)ShowItemAwardPopup;
			reflectedScript.Arguments = ScriptActionQueueExtensions.ReplacedArguments;
		}
	}
}
