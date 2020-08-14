using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.TimespinnerGearItem")]
	// ReSharper disable once UnusedMember.Global
	class TimespinnerGearItem : Replaces
	{
		const int Yoffset = 42;

		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var reflected = obj.AsDynamic();

			var chest = new TreasureChestEvent(level, new Point(obj.Position.X, obj.Position.Y + Yoffset), -1, reflected._objectSpec);

			chest.AsDynamic()._lidAppendage.IsFacingLeft = obj.IsFacingLeft;
			chest.IsFacingLeft = obj.IsFacingLeft;

			return new[] { chest };
		}
	}
}
