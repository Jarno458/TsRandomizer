using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(5, 29)]
	class RoyalTowersDoor : RoomTrigger
	{
		static readonly Type TransitionDoorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionDoorEvent");
		public override void OnRoomLoad(RoomState roomState)
		{
                        if (!roomState.Seed.Options.RoyalRoadblock)
                                return;

			var transitionDoor = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values
				.FirstOrDefault(obj => obj.GetType() == TransitionDoorEventType);
			var dynamicDoor = transitionDoor.AsDynamic();
			dynamicDoor._isRoyalDoor = true;
			dynamicDoor._baseGemGlowColor = new Color(255, 200, 219, 255);
			var dynamicMainGem = ((Appendage)dynamicDoor._mainGem).AsDynamic();
                        var mainGemFrameSource = dynamicMainGem._frameSource;
			mainGemFrameSource.X = 96;
			mainGemFrameSource.Y = 49;
			dynamicMainGem._frameSource = mainGemFrameSource;
                        var gems = dynamicDoor._gems;
                        foreach (var gem in gems) {
			    var dynamicGem = ((Appendage)gem).AsDynamic();
			    var gemFrameSource = dynamicGem._frameSource;
			    gemFrameSource.X = 111;
			    gemFrameSource.Y = 66;
			    dynamicGem._frameSource = gemFrameSource;
                        }
		}
	}
}
