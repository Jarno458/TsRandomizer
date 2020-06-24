using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.PyramidKeys")]
	class PyramidKeys : Replaces
	{
		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var reflected = obj.AsDynamic();

			return new[] {
				new TreasureChestEvent(level, new Point(296, 176), -1, reflected._objectSpec) //position based on room in the future
			};
		}
	}
}
