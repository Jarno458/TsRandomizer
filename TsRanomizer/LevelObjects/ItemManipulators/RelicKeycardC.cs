using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;
using TsRanodmizer.Screens;

namespace TsRanodmizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.RelicKeycardC")]
	[AlwaysSpawn(EEventTileType.TimespinnerWheelItem, 6)]
	// ReSharper disable once UnusedMember.Global
	class RelicKeycardC : ItemManipulator, ICustomSpwanMethod
	{
		bool hasDroppedLoot;
		bool hasCardC;

		public RelicKeycardC(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			if(IsPickedUp)
				Object.Kill();

			Object.ChangeAnimation(ItemInfo.AnimationIndex);

			hasCardC = Level.GameSave.Inventory.RelicInventory.Inventory.ContainsKey((int)EInventoryRelicType.ScienceKeycardC);
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasDroppedLoot || !Object.IsFading)
				return;

			if(!hasCardC)
				Level.GameSave.Inventory.RelicInventory.RemoveItem((int)EInventoryRelicType.ScienceKeycardC);

			Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);

			AwardContainedItem();

			hasDroppedLoot = true;
		}

		public GameEvent Spawn(Level level, ObjectTileSpecification specification)
		{
			var timeSpinnerType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Relics.RelicKeycardC");

			var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
			
			return (GameEvent) Activator.CreateInstance(timeSpinnerType, level, point, -1, specification, level.GCM.SpMenuIcons);
		}
	}
}
