using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
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
		}

		public static void Draw(
			SpriteBatch spriteBatch, SpriteFont menuFont, Vector2 levelRenderCenter,
			ItemLocationMap itemLocations
		)
		{
			var itemManipulators = Objects.OfType<ItemManipulator>().ToArray();

			for (var i = 0; i < itemManipulators.Length; i++)
			{
				var obj = itemManipulators[i];
				var itemLocation = obj.itemLocation;
				var drawKeyPos = new Vector2(30, 160 + 60 * i);
				var drawRequirementPos = new Vector2(30, 160 + (60 * i) + 24);
				var color = obj.ItemInfo != null
					? obj.ItemInfo != ItemInfo.Dummy
						? Color.Green
						: Color.DarkGreen
					: Color.Red;

				spriteBatch.DrawString(menuFont, $"{itemLocation.Key}", drawKeyPos, color, 2);
				spriteBatch.DrawString(menuFont, $"Requirement: {itemLocation.Gate}", drawRequirementPos, color, 1.5f);
			}
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
