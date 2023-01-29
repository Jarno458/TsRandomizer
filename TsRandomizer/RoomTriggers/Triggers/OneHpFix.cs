using System;
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
				? state.Level.GCM.TsForestPastTileset
				: state.Level.GCM.TsL1Tileset;

			int[] bridgeStarts = {
				4,
				42,
				80
			};

			var random = new Random();
			var frameSources = new[] {
				spriteSheet.GetFrameSource(109),
				spriteSheet.GetFrameSource(110),
				spriteSheet.GetFrameSource(111)
			};

			foreach (var bridgeStart in bridgeStarts)
			{
				for (int i = 0; i < 16; i++)
				{
					var donutSpecification = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Event,
						Layer = ETileLayerType.Middle,
						ObjectID = (int)EEventTileType.DonutTile,
						X = bridgeStart + i,
						Y = 11
					};

					state.Level.PlaceEvent(donutSpecification, false);

					var dynamic = ((Dictionary<int, GameEvent>)state.Level.AsDynamic()._levelEvents).Last().Value.AsDynamic();

					dynamic._sprite = spriteSheet;
					dynamic._frameSource = frameSources.SelectRandom(random);
				}
			}

			RoomTriggerHelper.PlaceWater(state.Level, new Point(4,11), new Point(95, 13));
		}
	}
}
