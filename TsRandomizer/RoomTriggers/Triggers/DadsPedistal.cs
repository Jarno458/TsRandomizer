using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(12, 11)]
	class DadsPedistal : RoomTrigger
	{
		static readonly Type PedestalType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");

		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Emperor"))
				return;

			((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values
				.FirstOrDefault(obj => obj.GetType() == PedestalType)
					?.SilentKill();
		}
	}
}
