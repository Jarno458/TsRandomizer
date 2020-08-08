using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.ReplacementObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Relics.TalariaAttachment")]
	// ReSharper disable once UnusedMember.Global
	class TalariaAttachment : Replaces
	{
		int Yoffset = 31;

		protected override IEnumerable<Animate> Replace(Level level, Animate obj)
		{
			var reflected = obj.AsDynamic();

			var chest = new TreasureChestEvent(level, new Point(obj.Position.X, obj.Position.Y + Yoffset), -1, reflected._objectSpec);

			chest.AsDynamic()._lidAppendage.IsFacingLeft = false;
			chest.IsFacingLeft = false;

			return new[] { chest };
		}
	}
}
