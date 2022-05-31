using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events.Cutscene;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.RoboKitty.RoboKittyBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Varndagroth.VarndagrothBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Bird.GodBirdBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.DemonBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.AelanaBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.MawBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.CantoranBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.ShapeshifterBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.XarionBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Z_Raven.RavenBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.ZelBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.SandmanBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss")]
	// ReSharper disable once UnusedMember.Global
	class BossEnemy: LevelObject
	{
		bool songHasRun;
		bool clearedHasRun;
		bool saveHasRun;
		bool warpHasRun;
		bool isRandomized;
		BossAttributes currentBoss;
		BossAttributes vanillaBoss;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");
		static readonly Type TimeBreakEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.BreakLevelEffect");
		static readonly Type CutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");
		static readonly MethodInfo CreateAndCallCutsceneMethod = typeof(CutsceneBase).GetPrivateStaticMethod("CreateAndCallCutscene", CutsceneEnumType, typeof(Level), typeof(Point));

		protected override void Initialize(SeedOptions options)
		{
			isRandomized = Level.GameSave.GetSettings().BossRando.Value;
			int argument = 0;
			if (Dynamic.EnemyType == EEnemyTileType.EmperorBoss)
			{
				if (Dynamic._isPrinceEmperor)
					argument = 2;
				else if (Dynamic._isViletianEmperor)
					argument = 1;
			}

			var bestiaryEntry = Level.GCM.Bestiary.GetEntry(Dynamic.EnemyType, argument);
			int bossId = bestiaryEntry.Index;

			currentBoss = BestiaryManager.GetBossAttributes(Level, bossId);
			if (isRandomized)
				vanillaBoss = BestiaryManager.GetVanillaBoss(Level, bossId);
			else
				vanillaBoss = currentBoss;

			if (!isRandomized)
				return;

			Level.ToggleExits(false);
			Level.OpenAllBossDoors(-1f);
			Level.LockAllBossDoors(0.5f);

			Level.JukeBox.StopSong();
			Level.JukeBox.PlaySong(vanillaBoss.Song);
		}

		public BossEnemy(Mobile typedObject) : base(typedObject)
		{
			isRandomized = Level.GameSave.GetSettings().BossRando.Value;
			if (!isRandomized || !Level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			var boss = typedObject.AsDynamic();

			switch (boss.EnemyType)
			{
				case EEnemyTileType.EmperorBoss:
					if (boss._isPrinceEmperor)
						CreateAndCallCutsceneMethod.InvokeStatic(CutsceneEnumType.GetEnumValue("Alt0_Nuvius"), Level, new Point(200, 200));
					break;
				case EEnemyTileType.MawBoss:
					boss.DoIntroCloseMouth();
					break;
				case EEnemyTileType.BirdBoss:
					boss.InitializeMob();
					boss.EndBossIntroCutscene();
					break;
				case EEnemyTileType.VarndagrothBoss:
					Level.MainHero.TeleportToPoint(new Point(200, 200));
					boss._spindleItem.SilentKill();
					boss.StartBattle();
					break;
				default:
					break;
			}
		}

		void TeleportPlayer()
		{
			EDirection facing = vanillaBoss.IsFacingLeft ? EDirection.East : EDirection.West;
			Level.PlayCue(ESFX.FoleyWarpGyreOut);

			Level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = vanillaBoss.ReturnRoom.LevelId,
				RoomID = vanillaBoss.ReturnRoom.RoomId,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				AdditionalBlackScreenTime = 0.5f,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f,
				ShouldPlayLevelSong = true,
				EnterDirection = facing
			});
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (isRandomized && !songHasRun && Scripts.Count == 0 && Level.JukeBox.CurrentSongEnum != vanillaBoss.Song)
			{
				Level.JukeBox.StopSong();
				Level.JukeBox.PlaySong(vanillaBoss.Song);
				songHasRun = true;
			}

			if (warpHasRun || Dynamic._deathScriptTimer <= 0) return;

			if (!clearedHasRun && vanillaBoss.Index == 80)
			{
				// Boss is Nightmare
				var fillingMethod = Level.GameSave.GetFillingMethod();

				if (fillingMethod == FillingMethod.Archipelago)
					Client.SetStatus(ArchipelagoClientState.ClientGoal);

				clearedHasRun = true;
			};

			if (!saveHasRun)
			{
				BestiaryManager.SetBossKillSave(Level, vanillaBoss.Index);
				saveHasRun = true;
			}


			if (!isRandomized)
			{
				warpHasRun = true;
				return;
			}

			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
			if (eventTypes.Contains(EScriptType.GiveItem.GetType()))
				Scripts.RemoveGiveItem();

			if (currentBoss.Index != 70 && !eventTypes.Contains(SandStreamerEventType) && !eventTypes.Contains(TimeBreakEventType))
				return;

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
			Level.JukeBox.StopAllSFX();
			Level.JukeBox.StopSong();
			if (Level.GameSave.GetSettings().BossHealing.Value)
				LevelReflected.FullyHealPlayer();

			// Cause Time break
			if (vanillaBoss.ReturnRoom.LevelId == 15 && currentBoss.Index != 70)
			{
				warpHasRun = true;
				var enumValue = CutsceneEnumType.GetEnumValue("Alt2_Win");
				CreateAndCallCutsceneMethod.InvokeStatic(enumValue, Level, new Point(200, 200));
				return;
			}

			TeleportPlayer();
			warpHasRun = true;
		}
	}
}
