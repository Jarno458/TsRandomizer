using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;

namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	abstract class BossRoomTrigger : RoomTrigger
	{
		protected static int TargetBossId = -1;

		public override void OnRoomLoad(RoomState roomState)
		{
			SpawnBoss(roomState.Level, roomState.Seed.Options, TargetBossId);

			if (roomState.Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			CreateBossWarp(roomState.Level, (int)GetVanillaBoss(roomState.RoomKey));
		}

		static EBossID GetVanillaBoss(Roomkey roomKey)
		{
			if (roomKey.LevelId == 1 && roomKey.RoomId == 5)
				return EBossID.FelineSentry;
			if (roomKey.LevelId == 2 && roomKey.RoomId == 29)
				return EBossID.Varndagroth;
			if (roomKey.LevelId == 5 && roomKey.RoomId == 5)
				return EBossID.GoldenIdol;
			if (roomKey.LevelId == 6 && roomKey.RoomId == 15)
				return EBossID.Aelana;
			if (roomKey.LevelId == 7 && roomKey.RoomId == 0)
				return EBossID.AzureQueen;
			if (roomKey.LevelId == 8 && roomKey.RoomId == 7)
				return EBossID.Maw;
			if (roomKey.LevelId == 9 && roomKey.RoomId == 7)
				return EBossID.Xarion;
			if (roomKey.LevelId == 11 && roomKey.RoomId == 21)
				return EBossID.Genza;
			if (roomKey.LevelId == 12 && roomKey.RoomId == 20)
				return EBossID.Nuvius;
			if (roomKey.LevelId == 13 && roomKey.RoomId == 0)
				return EBossID.Prince;
			if (roomKey.LevelId == 13 && roomKey.RoomId == 1)
				return EBossID.Vol;
			if (roomKey.LevelId == 14 && roomKey.RoomId == 4)
				return EBossID.Ravenlord;
			if (roomKey.LevelId == 14 && roomKey.RoomId == 5)
				return EBossID.Ifrit;
			if (roomKey.LevelId == 16 && roomKey.RoomId == 4)
				return EBossID.Sandman;
			if (roomKey.LevelId == 16 && roomKey.RoomId == 26)
				return EBossID.Nightmare;

			throw new ArgumentOutOfRangeException($"No vanilla boss found for room: {roomKey}");
		}

		protected static void SpawnBoss(Level level, SeedOptions seedOptions, int vanillaBossId)
		{
			if (!level.GameSave.GetSettings().BossRando.Value 
				    || TargetBossId == -1 
				    || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			BossAttributes vanillaBossInfo = BestiaryManager.GetBossAttributes(level, vanillaBossId);
			BossAttributes replacedBossInfo = BestiaryManager.GetReplacedBoss(level, vanillaBossId);

			level.JukeBox.StopSong();
			level.PlayCue(Timespinner.GameAbstractions.ESFX.FoleyWarpGyreIn);

			if (seedOptions.GasMaw && (vanillaBossId == (int)EBossID.Maw || (vanillaBossId == (int)EBossID.FelineSentry && level.GameSave.GetSaveBool("TSRando_IsVileteSaved"))))
				RoomTriggerHelper.FillRoomWithGas(level);

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

		protected static void CreateBossWarp(Level level, int vanillaBossId)
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
	}
}
