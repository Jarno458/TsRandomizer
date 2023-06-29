using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers
{
	[RoomTriggerTrigger(16, 1)]
	[RoomTriggerTrigger(16, 5)]
	class SandmanSoftlockPrevention : RoomTrigger
	{
		readonly static Type SpikeType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Sandman.SandmanBossCeilingSpikes");

		public override void OnRoomLoad(RoomState state)
		{
			var position = (state.RoomKey.RoomId == 1)
				? new Point(56, 1392)
				: new Point(56, 32);

			var sprite = state.Level.GCM.SpSandmanBoss;
			var spikes = (Mobile)SpikeType.CreateInstance(false, state.Level, position, sprite, new ObjectTileSpecification());

			spikes.Position = position;

			List<Appendage> appendages = spikes.AsDynamic().Appendages;
			appendages.RemoveRange(19, 5);

			state.Level.AsDynamic().RequestAddObject(spikes);
		}
	}
}
