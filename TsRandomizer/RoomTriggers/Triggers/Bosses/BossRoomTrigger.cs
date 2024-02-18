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
			SpawnBoss(roomState, TargetBossId);

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
			// Generic room - could support other bosses in the future
			if (roomKey.LevelId == 17 && roomKey.RoomId == 8)
				return EBossID.Cantoran;

			return (EBossID)(-1);
		}

		protected static void SpawnBoss(RoomState state, int vanillaBossId)
		{
			var level = state.Level;

			SpawnWaterOrGasIfNeeded(state, vanillaBossId == -1 ? (int)GetVanillaBoss(state.RoomKey) : vanillaBossId);

			if ((level.GameSave.GetSettings().BossRando.Value == "Off"
				|| TargetBossId == -1
				|| !level.GameSave.GetSaveBool("IsFightingBoss"))
				&& level.ID != 17)
					return;
			
			var vanillaBossInfo = BestiaryManager.GetBossAttributes(level, vanillaBossId);
			var replacedBossInfo = BestiaryManager.GetReplacedBoss(level, vanillaBossId);

			level.JukeBox.StopSong();
			level.PlayCue(Timespinner.GameAbstractions.ESFX.FoleyWarpGyreIn);

			if (replacedBossInfo.ShouldSpawn)
			{
				var bossTile = new ObjectTileSpecification {
					Category = EObjectTileCategory.Enemy,
					Layer = ETileLayerType.Objects,
					ObjectID = replacedBossInfo.TileId,
					Argument = replacedBossInfo.Argument,
					IsFlippedHorizontally = !replacedBossInfo.IsFacingLeft
				};

				var boss = replacedBossInfo.BossType.CreateInstance(false, replacedBossInfo.Position, level, replacedBossInfo.Sprite, -1, bossTile);
				level.AsDynamic().RequestAddObject(boss);
			}

			level.JukeBox.StopSong();
			level.JukeBox.PlaySong(vanillaBossInfo.Song);

			TargetBossId = -1;
		}

		static void SpawnWaterOrGasIfNeeded(RoomState state, int vanillaBossId)
		{
			if ((state.Seed.Options.GasMaw && vanillaBossId == (int)EBossID.Maw)
			    || (vanillaBossId == (int)EBossID.FelineSentry && state.Level.GameSave.GetSaveBool("TSRando_IsVileteSaved")))
				RoomTriggerHelper.FillRoomWithGas(state.Level);

			if ((state.Seed.FloodFlags.Maw && vanillaBossId == (int)EBossID.Maw)
			    || (state.Seed.FloodFlags.Xarion && vanillaBossId == (int)EBossID.Xarion)
			    || (state.Seed.FloodFlags.BackPyramid && (vanillaBossId == (int)EBossID.Sandman || vanillaBossId == (int)EBossID.Nightmare)))
				RoomTriggerHelper.FillRoomWithWater(state.Level);
		}

		protected static void CreateBossWarp(Level level, int vanillaBossId)
		{
			BestiaryManager.RefreshBossSaveFlags(level);
			if (level.GameSave.GetSettings().BossRando.Value == "Off")
				return;

			var vanillaBossInfo = BestiaryManager.GetBossAttributes(level, vanillaBossId);
			var replacedBossInfo = BestiaryManager.GetReplacedBoss(level, vanillaBossId);

			if (level.GameSave.GetSaveBool("TSRando_" + vanillaBossInfo.SaveName))
				return;

			TargetBossId = vanillaBossId;

			level.JukeBox.StopSong();

			var bossArena = replacedBossInfo.BossRoom;

			BestiaryManager.ClearBossSaveFlags(level, replacedBossInfo.ShouldSpawn);

			level.GameSave.SetValue("IsFightingBoss", true);

			var facing = replacedBossInfo.IsFacingLeft ? EDirection.West : EDirection.East;

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
