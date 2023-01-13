using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(8, 37)]
	[RoomTriggerTrigger(9, 37)]
	class OneHpFix : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			var spriteSheet = state.RoomKey.LevelId == 8
				? state.ScreenManager.GameContentManager.TsForestPastTileset
				: state.ScreenManager.GameContentManager.TsL1Tileset;

			int[] bridgeStarts = new[] {
				4,
				42,
				80
			};

			foreach (var bridgeStart in bridgeStarts)
			{
				for (int i = 0; i < 16; i++)
				{
					var donotSpecification = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Event,
						Layer = ETileLayerType.Middle,
						ObjectID = (int)EEventTileType.DonutTile,
						X = bridgeStart + i,
						Y = 11
					};

					state.Level.PlaceEvent(donotSpecification, false);

					var dynamic = ((Dictionary<int, GameEvent>)state.Level.AsDynamic()._levelEvents).Last().Value.AsDynamic();

					dynamic._sprite = spriteSheet;
					dynamic.ChangeAnimation(state.Level.NextRandomInt(109, 111), 0, 1f, EAnimationType.None);
				}
			}

			RoomTriggerHelper.PlaceWater(state.Level, new Point(4,11), new Point(95, 13));
		}
	}
}
