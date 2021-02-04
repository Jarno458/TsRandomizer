using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.PyramidKeys")]
	// ReSharper disable once UnusedMember.Global
	class PyramidKeys : Replaces
	{
		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var bestiarySpecification = new BestiaryItemDropSpecification();
			var itemDropPickup = (Animate)Activator.CreateInstance(itemDropPickupType, bestiarySpecification, level, new Point(296, 176), -1);

			var item = itemDropPickup.AsDynamic();
			item.Initialize();

			return new[] { itemDropPickup };
		}
	}
}
