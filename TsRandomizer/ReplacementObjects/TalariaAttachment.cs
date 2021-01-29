using System;
using System.Collections.Generic;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.TalariaAttachment")]
	// ReSharper disable once UnusedMember.Global
	class TalariaAttachment : Replaces
	{
		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var bestiarySpecification = new BestiaryItemDropSpecification();
			var itemDropPickup = (Animate)Activator.CreateInstance(itemDropPickupType, bestiarySpecification, level, obj.Position, -1);

			var item = itemDropPickup.AsDynamic();
			item.Initialize();

			return new[] { itemDropPickup };
		}
	}
}
