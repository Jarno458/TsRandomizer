using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizer.LevelObjects
{
	abstract class ItemManipulator<T> : ItemManipulator where T : Mobile
	{
		public readonly T TypedObject;

		protected ItemManipulator(T typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
			TypedObject = typedObject;
		}
	}

	abstract class ItemManipulator : LevelObject
	{
		static ItemLocationMap itemLocationMap;

		public bool IsPickedUp => itemLocation.IsPickedUp;

		public readonly ItemInfo ItemInfo;
		readonly ItemLocation itemLocation;

		protected ItemManipulator(Mobile typedObject, ItemLocation itemLocation) : base(typedObject)
		{
			this.itemLocation = itemLocation;
			ItemInfo = itemLocation?.ItemInfo;
		}

		protected void AwardContainedItem()
		{
			Level.GameSave.AddItem(Level, ItemInfo.Identifier);

			OnItemPickup();
		}

		protected void OnItemPickup()
		{
			ItemInfo.OnPickup(Level);
			itemLocation.SetPickedUp();

			if (ItemInfo.Unlocks != Requirement.None)
				ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(itemLocationMap));
		}

		public static void Initialize(ItemLocationMap itemLocations)
		{
			itemLocationMap = itemLocations;
		}

		public static ItemManipulator GenerateShadowObject(Type levelObjectType, Mobile obj, ItemLocationMap itemLocations)
		{
			var itemKey = GetKey(obj);
			var itemLocation = itemLocations[itemKey];

			if (itemLocation == null)
			{
				Console.Out.WriteLine($"UnmappedItem: {itemKey}");
				return null;
			}

			return (ItemManipulator)Activator.CreateInstance(levelObjectType, obj, itemLocation);
		}

		protected void ShowItemAwardPopup()
		{
			switch (ItemInfo.Identifier.LootType)
			{
				case LootType.ConstOrb:
					Level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Identifier.OrbType, ItemInfo.Identifier.OrbSlot));
					break;
				case LootType.ConstFamiliar:
					Level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Identifier.Familiar));
					break;
				case LootType.ConstRelic:
					Level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Identifier.Relic));
					break;
				case LootType.ConstEquipment:
					Level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Identifier.Enquipment));
					break;
				case LootType.ConstUseItem:
					Level.AddScript((ScriptAction)typeof(ScriptAction).CreateInstance(true, ItemInfo.Identifier.UseItem, 1));
					break;
				case LootType.ConstStat:
					Level.RequestToastPopupForStats(ItemInfo);
					break; 
				default:
					throw new NotImplementedException($"RelicOrOrbGetPopup is not implemented for LootType {ItemInfo.Identifier.LootType}");
			}
		}
	}
}
