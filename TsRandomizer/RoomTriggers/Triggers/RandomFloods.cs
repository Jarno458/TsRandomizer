using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;

namespace TsRandomizer.RoomTriggers.Triggers
{
	//castleloop
	[RoomTriggerTrigger(5, 1)]
	[RoomTriggerTrigger(5, 3)]
	[RoomTriggerTrigger(5, 9)]
	[RoomTriggerTrigger(5, 10)]
	[RoomTriggerTrigger(5, 11)]
	[RoomTriggerTrigger(5, 13)]
	[RoomTriggerTrigger(5, 15)]
	[RoomTriggerTrigger(5, 16)]
	[RoomTriggerTrigger(5, 33)]
	[RoomTriggerTrigger(5, 35)]
	[RoomTriggerTrigger(5, 37)]
	[RoomTriggerTrigger(5, 38)]
	[RoomTriggerTrigger(5, 43)]
	[RoomTriggerTrigger(5, 44)]
	[RoomTriggerTrigger(5, 45)]

	//xarion
	[RoomTriggerTrigger(9, 8)]
	[RoomTriggerTrigger(9, 13)]
	[RoomTriggerTrigger(9, 14)]
	[RoomTriggerTrigger(9, 21)]
	[RoomTriggerTrigger(9, 33)]
	[RoomTriggerTrigger(9, 36)]
	[RoomTriggerTrigger(9, 43)]
	[RoomTriggerTrigger(9, 49)]

	//maw
	[RoomTriggerTrigger(8, 6)]
	[RoomTriggerTrigger(8, 8)]
	[RoomTriggerTrigger(8, 9)]
	[RoomTriggerTrigger(8, 12)]
	[RoomTriggerTrigger(8, 13)]
	[RoomTriggerTrigger(8, 14)]
	[RoomTriggerTrigger(8, 15)]
	[RoomTriggerTrigger(8, 16)]
	[RoomTriggerTrigger(8, 19)]
	[RoomTriggerTrigger(8, 21)]
	[RoomTriggerTrigger(8, 31)]
	[RoomTriggerTrigger(8, 33)]
	[RoomTriggerTrigger(8, 34)]
	[RoomTriggerTrigger(8, 36)]
	[RoomTriggerTrigger(8, 38)]
	[RoomTriggerTrigger(8, 40)]
	[RoomTriggerTrigger(8, 42)]
	[RoomTriggerTrigger(8, 43)]
	[RoomTriggerTrigger(8, 44)]
	[RoomTriggerTrigger(8, 45)]
	[RoomTriggerTrigger(8, 46)]
	[RoomTriggerTrigger(8, 47)]
	[RoomTriggerTrigger(8, 48)]
	[RoomTriggerTrigger(8, 49)]

	//pyramid back
	[RoomTriggerTrigger(16, 1)]
	[RoomTriggerTrigger(16, 5)]
	//pyramid shaft
	[RoomTriggerTrigger(16, 6)]
	[RoomTriggerTrigger(16, 8)]
	[RoomTriggerTrigger(16, 16)]
	[RoomTriggerTrigger(16, 20)]
	[RoomTriggerTrigger(16, 21)]
	[RoomTriggerTrigger(16, 22)]
	[RoomTriggerTrigger(16, 23)]

	//moat
	[RoomTriggerTrigger(4, 0)]
	[RoomTriggerTrigger(4, 18)]
	[RoomTriggerTrigger(4, 19)]
	[RoomTriggerTrigger(4, 20)]

	//castle courtyard
	[RoomTriggerTrigger(6, 1)]

	//lake desolation
	[RoomTriggerTrigger(1, 2)]
	[RoomTriggerTrigger(1, 10)]
	[RoomTriggerTrigger(1, 23)]

	//lake serene
	[RoomTriggerTrigger(7, 1)]
	[RoomTriggerTrigger(7, 2)]
	[RoomTriggerTrigger(7, 3)]
	[RoomTriggerTrigger(7, 6)]
	[RoomTriggerTrigger(7, 10)]
	[RoomTriggerTrigger(7, 29)]
	class RandomFloods : RoomTrigger
	{
		public override void OnRoomLoad(RoomState state)
		{
			switch (state.RoomKey.LevelId)
			{
				case 1 when state.Seed.FloodFlags.LakeDesolation:
					HandleLakeDesolationFlood(state);
					break;
				case 4 when state.Seed.FloodFlags.CastleMoat:
					HandleCastleMoatFlood(state);
					break;
				case 5 when state.Seed.FloodFlags.Basement:
					HandleCastleLoopFlood(state, state.Seed.FloodFlags.BasementHigh);
					break;
				case 6 when state.Seed.FloodFlags.CastleCourtyard:
					HandleCastleCourtyardFlood(state);
					break;
				case 7:
					HandleLakeSereneDroughts(state);
					break;
				case 8 when state.Seed.FloodFlags.Maw:
					HandleMawFlood(state);
					break;
				case 9 when state.Seed.FloodFlags.Xarion:
					HandleXarionFlood(state);
					break;
				case 16:
					if (state.Seed.FloodFlags.PyramidShaft)
						HandleFloodPyramidShaft(state);
					if (state.Seed.FloodFlags.BackPyramid)
						HandleFloodPyramidBack(state);
					break;
			}
		}

		static void HandleCastleLoopFlood(RoomState state, bool high)
		{
			if (high)
			{
				switch (state.RoomKey.RoomId)
				{
					case 3:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 20), state.Level.RoomSize16);
						break;
					case 1:
					case 13:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 4), state.Level.RoomSize16);
						break;
					case 9:
					case 11:
					case 38:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 3), state.Level.RoomSize16);
						break;
					case 10:
						var waterLeftOffset = state.Level.GameSave.GetSaveBool("BW_5_10_0") ? 0 : 4;
						RoomTriggerHelper.PlaceWater(state.Level, new Point(waterLeftOffset, 3),
							state.Level.RoomSize16);
						break;
					default:
						RoomTriggerHelper.FillRoomWithWater(state.Level);
						break;
				}
			}
			else
			{
				switch (state.RoomKey.RoomId)
				{
					case 3:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 24), state.Level.RoomSize16);
						break;
					case 1:
					case 13:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 8), state.Level.RoomSize16);
						break;
					case 9:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 5), state.Level.RoomSize16);
						break;
					case 11:
					case 38:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 7), state.Level.RoomSize16);
						break;
					case 37:
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 9), state.Level.RoomSize16);
						break;
					case 10:
						var waterLeftOffset = state.Level.GameSave.GetSaveBool("BW_5_10_0") ? 0 : 4;
						RoomTriggerHelper.PlaceWater(state.Level, new Point(waterLeftOffset, 7),
							state.Level.RoomSize16);
						break;
					default:
						RoomTriggerHelper.FillRoomWithWater(state.Level);
						break;
				}
			}
		}

		static void HandleXarionFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 8:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 23), state.Level.RoomSize16);
					break;
				case 21:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 42), state.Level.RoomSize16);
					break;
				default:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		static void HandleMawFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 9:
					var isBreakableWallBroken = state.Level.GameSave.GetSaveBool("BW_8_9_0");

					if (isBreakableWallBroken)
					{
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0), state.Level.RoomSize16);
					}
					else
					{
						RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0),
							new Point(state.Level.RoomSize16.X - 4, state.Level.RoomSize16.Y));
						RoomTriggerHelper.PlaceWater(state.Level, new Point(state.Level.RoomSize16.X - 4, 40),
							state.Level.RoomSize16);
						RoomTriggerHelper.PlaceWater(state.Level, new Point(state.Level.RoomSize16.X - 4, 0),
							new Point(state.Level.RoomSize16.X, 10));
					}

					break;
				case 16:
					RoomTriggerHelper.RemoveWotah(state.Level);
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 17), state.Level.RoomSize16);

					state.Level.Foregrounds.RemoveAll(fg =>
						fg.AsDynamic().TextureType == EBackgroundTextureType.Cave_Backdrops1);

					var bgpTiles = (Dictionary<Point, List<Tile>>)state.Level.AsDynamic()._backgroundTiles;
					foreach (var bgpTileArray in bgpTiles)
					{
						if (bgpTileArray.Key.Y < 114)
							continue;

						var targetTile = bgpTiles[bgpTileArray.Key][0].AsDynamic();
						var sourceTile = bgpTiles[new Point(bgpTileArray.Key.X, bgpTileArray.Key.Y - 9)][0].AsDynamic();

						targetTile._drawSource = sourceTile._drawSource;
						targetTile.IsFlippedVertically = sourceTile.IsFlippedVertically;
						targetTile.IsFlippedHorizontally = sourceTile.IsFlippedHorizontally;
					}

					var siren = ((Dictionary<int, Monster>)state.Level.AsDynamic()._enemies).First().Value;
					siren.Position = new Point(580, 280);
					break;
				case 19:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 53), state.Level.RoomSize16);
					break;
				case 31:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 73), state.Level.RoomSize16);
					break;
				case 38:
				case 40:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 6), state.Level.RoomSize16);
					break;
				case 44:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 30), state.Level.RoomSize16);
					break;
				default:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		static void HandleFloodPyramidShaft(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 6:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 10), state.Level.RoomSize16);
					break;
				case 8:
					var waterRightOffset8 = state.Level.GameSave.GetSaveBool("BW_16_8_0")
						? state.Level.RoomSize16.X
						: state.Level.RoomSize16.X - 3;
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0),
						new Point(waterRightOffset8, state.Level.RoomSize16.Y));
					break;
				case 22:
					var waterRightOffset22 = state.Level.GameSave.GetSaveBool("BW_16_22_0")
						? state.Level.RoomSize16.X
						: state.Level.RoomSize16.X - 3;
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 0),
						new Point(waterRightOffset22, state.Level.RoomSize16.Y));
					break;
				case 16:
				case 20:
				case 21:
				case 23:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		static void HandleFloodPyramidBack(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 1:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 50), state.Level.RoomSize16);
					break;
				case 4:
				case 5:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		static void HandleCastleMoatFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 0:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 13), state.Level.RoomSize16);
					break;
				case 18:
					RoomTriggerHelper.RemoveWotah(state.Level);
					RoomTriggerHelper.FillRoomWithWater(state.Level);

					state.Level.Backgrounds.RemoveAll(bg => {
						var textureType = bg.AsDynamic().TextureType;
						return textureType == EBackgroundTextureType.Curtain_Backdrops1 ||
						       textureType == EBackgroundTextureType.Forest_Rocks_Near;
					});
					break;
				case 19:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
				case 20:
					RoomTriggerHelper.FillRoomWithWater(state.Level);
					break;
			}
		}

		static void HandleCastleCourtyardFlood(RoomState state) =>
			RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 7), state.Level.RoomSize16);

		static void HandleLakeDesolationFlood(RoomState state)
		{
			switch (state.RoomKey.RoomId)
			{
				case 2:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 39), state.Level.RoomSize16);
					break;
				case 10:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(0, 11), state.Level.RoomSize16);
					break;
				case 23:
					RoomTriggerHelper.PlaceWater(state.Level, new Point(17, 7), state.Level.RoomSize16);
					break;
			}
		}

		static void HandleLakeSereneDroughts(RoomState state)
		{
			RoomTriggerHelper.RemoveWotah(state.Level);

			switch (state.RoomKey.RoomId)
			{
				case 1:
					state.Level.Backgrounds.RemoveAll(bg => {
						var background = bg.AsDynamic();
						var textureType = background.TextureType;
						return textureType == EBackgroundTextureType.Fog
							|| (textureType == EBackgroundTextureType.L7_Backdrop2
							    && background._frameSource == new Rectangle(0, 0, 16, 144));
					});

					foreach (var background in state.Level.Backgrounds)
					{
						var bg = background.AsDynamic();
						var textureType = bg.TextureType;

						if (textureType == EBackgroundTextureType.L7_Backdrop3)
						{
							background.Position = new Vector2(background.Position.X, background.Position.Y + 109);
							bg._isInitialized = false;
							background.RefreshCameraValues();

						}
						else if (textureType == EBackgroundTextureType.L7_Backdrop1)
						{
							background.Position = new Vector2(background.Position.X, background.Position.Y + 80);
							bg._isInitialized = false;
							background.RefreshCameraValues();
						}
					}

					var dynamic = state.Level.AsDynamic();
					dynamic.BackgroundWipeColor = new Color(122, 155, 145);
					
					foreach (var enemy in dynamic._enemies.Values)
						if (enemy.EnemyType == EEnemyTileType.LakeAnemone)
							enemy.SilentKill();

					var anemone1 = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.LakeAnemone,
						Argument = 1,
						X = 64,
						Y = 15
					};
					var anemone2 = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.LakeAnemone,
						Argument = 1,
						X = 87,
						Y = 16
					};
					var anemone3 = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.LakeAnemone,
						Argument = 1,
						X = 115,
						Y = 14
					};

					state.Level.PlaceEvent(anemone1, true);
					state.Level.PlaceEvent(anemone2, true);
					state.Level.PlaceEvent(anemone3, true);

					break;
				case 2:
					var specification = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.KickstarterFoe,
						Argument = 0,
						IsFlippedHorizontally = state.Level.MainHero.Position.X > 50,
						X = state.Level.MainHero.Position.X > 50 ? 58 : 57,
						Y = 38,
					};

					state.Level.PlaceEvent(specification, false);

					break;
				case 3:
					var specification1 = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.ForestBabyCheveux,
						IsFlippedHorizontally = state.Level.MainHero.Position.X > 50,
						X = 23,
						Y = 68,
					};
					var specification2 = new ObjectTileSpecification
					{
						Category = EObjectTileCategory.Enemy,
						Layer = ETileLayerType.Objects,
						ObjectID = (int)EEnemyTileType.ForestBabyCheveux,
						IsFlippedHorizontally = state.Level.MainHero.Position.X > 50,
						X = 30,
						Y = 50,
					};

					state.Level.PlaceEvent(specification1, false);
					state.Level.PlaceEvent(specification2, false);

					break;
			}
		}
	}
}