using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
			Level.GameSave.AddItem(Level, ItemInfo);

			OnItemPickup();
		}

		protected void OnItemPickup()
		{
			ItemInfo.OnPickup(Level);
			itemLocation.SetPickedUp();

			if (itemLocation.Unlocks != Requirement.None)
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
	}
}
