using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.TimespinnerWheelItem")]
	class TimespinnerWheel : Replaces
	{
		public const int YOffset = 48;

		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var reflected = obj.AsDynamic();
			return new[] {
				new TreasureChestEvent(level, new Point(obj.Position.X, obj.Position.Y + YOffset), -1, reflected._objectSpec)
			};
		}
	}
}
