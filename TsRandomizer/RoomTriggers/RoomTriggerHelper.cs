using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using Timespinner.GameObjects.Events.Cutscene;
using Timespinner.GameObjects.Events.EnvironmentPrefabs;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.RoomTriggers
{
	static class RoomTriggerHelper
	{
		static readonly Type TransitionWarpEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionWarpEvent");
		static readonly Type GyreType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.GyrePortalEvent");
		static readonly Type NelisteNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.AstrologerNPC");
		static readonly Type YorneNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.YorneNPC");
		static readonly Type GlowingFloorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabVilete");
		static readonly Type PedestalType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
		static readonly Type LakeVacuumLevelEffectType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.LakeVacuumLevelEffect");
		static readonly Type CirclePlatformType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Platforms.CirclePlatformEvent");
		static readonly Type MovingPlatformType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Platforms.MovingPlatformEvent");
		static readonly Type BossDoorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.BossDoorEvent");
		static readonly Type TeleportEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TeleportEvent");
		static readonly Type BaseLanternType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Lanterns.BaseLantern");
		static readonly Type CutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");
		static readonly MethodInfo CreateAndCallCutsceneMethod = typeof(CutsceneBase).GetPrivateStaticMethod("CreateAndCallCutscene", CutsceneEnumType, typeof(Level), typeof(Point));


		public static int TargetBossId = -1;

		public static void SpawnBoss(Level level, SeedOptions seedOptions, int vanillaBossId)
		{
			if (!level.GameSave.GetSettings().BossRando.Value || TargetBossId == -1 || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			BossAttributes vanillaBossInfo = BestiaryManager.GetBossAttributes(level, vanillaBossId);
			BossAttributes replacedBossInfo = BestiaryManager.GetReplacedBoss(level, vanillaBossId);

			level.JukeBox.StopSong();
			level.PlayCue(Timespinner.GameAbstractions.ESFX.FoleyWarpGyreIn);

			if (seedOptions.GasMaw && (vanillaBossId == (int)EBossID.Maw || (vanillaBossId == (int)EBossID.FelineSentry && level.GameSave.GetSaveBool("TSRando_IsVileteSaved"))))
				FillRoomWithGas(level);

			if (replacedBossInfo.ShouldSpawn)
			{
				ObjectTileSpecification bossTile = new ObjectTileSpecification();
				bossTile.Category = EObjectTileCategory.Enemy;
				bossTile.Layer = ETileLayerType.Objects;
				bossTile.ObjectID = replacedBossInfo.TileId;
				bossTile.Argument = replacedBossInfo.Argument;
				bossTile.IsFlippedHorizontally = !replacedBossInfo.IsFacingLeft;

				var boss = replacedBossInfo.BossType.CreateInstance(false, replacedBossInfo.Position, level, replacedBossInfo.Sprite, -1, bossTile);
				level.AsDynamic().RequestAddObject(boss);
			}

			level.JukeBox.StopSong();
			level.JukeBox.PlaySong(vanillaBossInfo.Song);
			TargetBossId = -1;
		}

		public static void CreateBossWarp(Level level, int vanillaBossId)
		{
			if (!level.GameSave.GetSettings().BossRando.Value)
				return;

			BestiaryManager.RefreshBossSaveFlags(level);
			BossAttributes vanillaBossInfo = BestiaryManager.GetBossAttributes(level, vanillaBossId);
			BossAttributes replacedBossInfo = BestiaryManager.GetReplacedBoss(level, vanillaBossId);
			if (level.GameSave.GetSaveBool("TSRando_" + vanillaBossInfo.SaveName))
				return;

			TargetBossId = vanillaBossId;

			level.JukeBox.StopSong();
			RoomItemKey bossArena = replacedBossInfo.BossRoom;
			BestiaryManager.ClearBossSaveFlags(level, replacedBossInfo.ShouldSpawn);
			level.GameSave.SetValue("IsFightingBoss", true);

			EDirection facing = replacedBossInfo.IsFacingLeft ? EDirection.West : EDirection.East;

			level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = bossArena.LevelId,
				RoomID = bossArena.RoomId,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f,
				EnterDirection = facing,
				AdditionalBlackScreenTime = 0.5f,
			});
		}

		public static void SpawnItemDropPickup(Level level, ItemInfo itemInfo, int x, int y)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = new Point(x, y);
			var itemDropPickup = Activator.CreateInstance(itemDropPickupType, itemInfo.BestiaryItemDropSpecification, level, itemPosition, -1);

			var item = itemDropPickup.AsDynamic();
			item.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((Item)itemDropPickup);
		}

		public static void SpawnTreasureChest(Level level, bool flipHorizontally, int x, int y)
		{
			var itemPosition = new Point(x, y);
			var specification = new ObjectTileSpecification { IsFlippedHorizontally = flipHorizontally, Layer = ETileLayerType.Objects };
			var treasureChest = new TreasureChestEvent(level, itemPosition, -1, specification);

			var chest = treasureChest.AsDynamic();
			chest.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject(treasureChest);
		}

		public static void SpawnOrbPredestal(Level level, int x, int y)
		{
			var orbPedestalEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
			var itemPosition = new Point(x, y);
			var pedistalSpecification = new TileSpecification
			{
				Argument = (int)EInventoryOrbType.Monske,
				ID = 480, //orb pedestal
				Layer = ETileLayerType.Objects,
			};
			var orbPedestalEvent = Activator.CreateInstance(orbPedestalEventType, level, itemPosition, -1, ObjectTileSpecification.FromTileSpecification(pedistalSpecification));

			var pedestal = orbPedestalEvent.AsDynamic();
			pedestal.DoesSpawnDespiteBeingOwned = true;
			pedestal.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((GameEvent)orbPedestalEvent);
		}

		public static void CreateSimpleOneWayWarp(Level level, int destinationLevelId, int destinationRoomId)
		{
			var dynamicLevel = level.AsDynamic();

			Dictionary<int, GameEvent> events = dynamicLevel._levelEvents;
			var warpTrigger = events.Values.FirstOrDefault(e => e.GetType() == TransitionWarpEventType);
			if (warpTrigger == null)
			{
				var specification = new ObjectTileSpecification
				{
					Category = EObjectTileCategory.Event,
					ID = 468,
					Layer = ETileLayerType.Objects,
					ObjectID = 13,
					X = 12,
					Y = 12
				};
				var point = new Point(specification.X * 16 + 8, specification.Y * 16 + 16);
				warpTrigger = (GameEvent)TransitionWarpEventType.CreateInstance(false, level, point, -1, specification);

				dynamicLevel.RequestAddObject(warpTrigger);
			}

			var dynamicWarpTrigger = warpTrigger.AsDynamic();

			var backToTheFutureWarp =
				new RequestButtonPressTrigger(level, warpTrigger.Position, dynamicWarpTrigger._objectSpec, (Action)delegate
				{
					dynamicWarpTrigger.StartWarpSequence(new LevelChangeRequest
					{
						LevelID = destinationLevelId,
						PreviousLevelID = level.ID,
						RoomID = destinationRoomId,
						IsUsingWarp = true,
						IsUsingWhiteFadeOut = true,
						AdditionalBlackScreenTime = 0.25f,
						FadeOutTime = 0.25f,
						FadeInTime = 1f
					});
				});

			dynamicLevel.RequestAddObject(backToTheFutureWarp);
		}

		public static void SpawnGlowingFloor(Level level, Point position)
		{
			var floor = GlowingFloorEventType.CreateInstance(false, level, position, -1, new ObjectTileSpecification(), EEnvironmentPrefabType.L0_TableCake);

			level.AsDynamic().RequestAddObject(floor);
		}

		public static void SpawnGyreWarp(Level level, int x, int y)
		{
			var position = new Point(x, y);
			var gyrePortal = GyreType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(gyrePortal);
		}

		public static void SpawnMovingPlatform(Level level, int x, int y)
		{
			var position = new Point(x, y);
			var platform = MovingPlatformType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(platform);
		}

		public static void SpawnCirclePlatform(Level level, int x, int y, bool isClockwise)
		{
			var position = new Point(x, y);
			ObjectTileSpecification platformTile = new ObjectTileSpecification();
			if (!isClockwise)
				platformTile.Argument = 1;
			var platform = CirclePlatformType.CreateInstance(false, level, position, -1, platformTile);

			level.AsDynamic().RequestAddObject(platform);
		}

		public static void FillRoomWithGas(Level level)
		{
			var gas = (GameEvent)LakeVacuumLevelEffectType.CreateInstance(false, level, new Point(), -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(gas);

			var foreground = level.Foregrounds.FirstOrDefault();

			if (foreground == null)
				return;

			foreground.AsDynamic()._baseColor = new Color(8, 16, 2, 12);
			foreground.DrawColor = new Color(8, 16, 2, 12);
		}

		public static void FillRoomWithWater(Level level) => PlaceWater(level, new Point(0, 0), level.RoomSize16);

		public static void PlaceWater(Level level, Point topLeftTilePos, Point bottomLeftTilePos)
		{
			var topLeft = new WaterFillerEvent(level, new Point(topLeftTilePos.X * 16, topLeftTilePos.Y * 16), topLeftTilePos,
				-1, true, new ObjectTileSpecification());
			var bottomLeft = new WaterFillerEvent(level, new Point(bottomLeftTilePos.X * 16, bottomLeftTilePos.Y * 16), bottomLeftTilePos,
				-1, false, new ObjectTileSpecification());

			var dynamicLevel = level.AsDynamic();

			var waterFillerTiles = (List<WaterFillerEvent>)dynamicLevel._waterFillerTiles;
			waterFillerTiles.Add(topLeft);
			waterFillerTiles.Add(bottomLeft);

			dynamicLevel.PlaceWaterTiles();

			var tileSize = new Point(16, 16);
			DestroyLanternsInArea(level, topLeftTilePos * tileSize, bottomLeftTilePos * tileSize);
		}

		public static void DestroyLanternsInArea(Level level, Point topLeft, Point bottomRight)
		{
			IEnumerable<GameEvent> events = level.AsDynamic()._levelEvents.Values;

			foreach (var gameEvent in events)
			{
				if (gameEvent.GetType().IsSubclassOf(BaseLanternType))
				{
					var x = gameEvent.OuterBbox.X;
					var y = gameEvent.OuterBbox.Y;

					if (x >= topLeft.X && x <= bottomRight.X && y >= topLeft.Y && y <= bottomRight.Y)
						gameEvent.SilentKill();
				}
			}
		}

		public static void CreateAndCallCutScene(RoomState roomStat, string cutSceneName)
		{
			var enumValue = CutsceneEnumType.GetEnumValue(cutSceneName);
			CreateAndCallCutsceneMethod.InvokeStatic(enumValue, roomStat.Level, new Point(200, 200));
		}
	}
}
