using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.RelicKeycardC")]
	[AlwaysSpawn(EEventTileType.TimespinnerWheelItem, 6)]
	// ReSharper disable once UnusedMember.Global
	class RelicKeycardC : ItemManipulator, ICustomSpwanMethod
	{
		bool hasDroppedLoot;
		bool hasCardC;

		public RelicKeycardC(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void Initialize(Seed seed)
		{
			if (ItemInfo == null)
				return;

			if(IsPickedUp)
				Dynamic.Kill();

			Dynamic.ChangeAnimation(ItemInfo.AnimationIndex);

			hasCardC = Level.GameSave.Inventory.RelicInventory.Inventory.ContainsKey((int)EInventoryRelicType.ScienceKeycardC);
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot || !Dynamic.IsFading)
				return;

			if(!hasCardC)
				Level.GameSave.Inventory.RelicInventory.RemoveItem((int)EInventoryRelicType.ScienceKeycardC);

			UpdateRelicOrbGetToastToItem();

			AwardContainedItem();

			hasDroppedLoot = true;
		}

		public Mobile Spawn(Level level, ObjectTileSpecification specification)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
			return (Item)Activator.CreateInstance(itemDropPickupType, ItemInfo.BestiaryItemDropSpecification, level, itemPosition, -1);
		}
	}
}
