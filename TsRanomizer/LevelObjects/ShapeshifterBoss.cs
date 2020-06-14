using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.ShapeshifterBoss")]
	// ReSharper disable once UnusedMember.Global
	class ShapeshifterBoss : LevelObject
	{
		bool hasReplacedItemScript;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");

		public ShapeshifterBoss(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, ItemInfo.Get(EInventoryEquipmentType.AdvisorRobe))
		{
		}

		//ShapeshifterBoss.PlaceKeycard
		//TODO spawn item if boss is dead but you havent lewted it yet

		protected override void Initialize()
		{
			var v = (Point) Object.DeathPosition;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasReplacedItemScript || (Point)Object.DeathPosition == Point.Zero)
				return;

			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
			if (!eventTypes.Contains(SandStreamerEventType))
				return;

			if (((Dictionary<int, Item>)LevelReflected._items).Count == 0)
			{
				var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
				var itemPosition = (Point) Object.Position;
				var itemDropPickup = Activator.CreateInstance(itemDropPickupType, ItemInfo.BestiaryItemDropSpecification, Level, itemPosition, -1);

				LevelReflected.RequestAddObject((Item)itemDropPickup);
			}

			hasReplacedItemScript = true;
		}
	}
}
