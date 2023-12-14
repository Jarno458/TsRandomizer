using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events.Cutscene;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
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
	class BossEnemy: LevelObject<Monster>
	{
		bool songHasRun;
		bool clearedHasRun;
		bool saveHasRun;
		bool warpHasRun;
		bool isRandomized;
		bool isDadFinalBoss;
		bool nightmareRemoved;
		bool isFinalBoss;
		int lastWarpLevel;
		int lastWarpRoom;
		BossAttributes currentBoss;
		BossAttributes vanillaBoss;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");
		static readonly Type TimeBreakEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.BreakLevelEffect");
		static readonly Type VarndagrothEyeEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Varndagroth.VarndagrothBoss+EEyelidAction");
		static readonly Type CutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");
		static readonly MethodInfo CreateAndCallCutsceneMethod = typeof(CutsceneBase).GetPrivateStaticMethod("CreateAndCallCutscene", CutsceneEnumType, typeof(Level), typeof(Point));

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			isRandomized = Level.GameSave.GetSettings().BossRando.Value != "Off" || Level.ID == 17;
			isDadFinalBoss = seed.Options.DadPercent;
			int argument = 0;
			if (TypedObject.EnemyType == EEnemyTileType.EmperorBoss)
			{
				if (Dynamic._isPrinceEmperor)
					argument = 2;
				else if (Dynamic._isViletianEmperor)
					argument = 1;
			}
			lastWarpLevel = Level.GameSave.LastWarpLevel;
			lastWarpRoom = Level.GameSave.LastWarpRoom;

			var bestiaryEntry = Level.GCM.Bestiary.GetEntry(TypedObject.EnemyType, argument);
			int bossId = bestiaryEntry.Index;

			currentBoss = BestiaryManager.GetBossAttributes(Level, bossId);
			vanillaBoss = isRandomized 
				? BestiaryManager.GetVanillaBoss(Level, bossId) 
				: currentBoss;

			isFinalBoss = ((isDadFinalBoss && vanillaBoss.Index == (int)EBossID.Nuvius) || (!isDadFinalBoss && vanillaBoss.Index == (int)EBossID.Nightmare));

			if (!isRandomized)
				return;

			Level.ToggleExits(false);
			Level.OpenAllBossDoors(-1f);
			Level.LockAllBossDoors(0.5f);

			Level.JukeBox.StopSong();
			Level.JukeBox.PlaySong(vanillaBoss.Song);

			switch (TypedObject.EnemyType)
			{
				case EEnemyTileType.EmperorBoss:
					if (Dynamic._isPrinceEmperor)
						CreateAndCallCutsceneMethod.InvokeStatic(CutsceneEnumType.GetEnumValue("Alt0_Nuvius"), Level, new Point(200, 200));
					break;
				case EEnemyTileType.MawBoss:
					Dynamic.DoIntroCloseMouth();
					break;
				case EEnemyTileType.BirdBoss:
					Dynamic.InitializeMob();
					Dynamic.EndBossIntroCutscene();
					break;
				case EEnemyTileType.CantoranBoss:
					var dialogScript = Scripts.FirstOrDefault(s => s.AsDynamic().ScriptType == EScriptType.Dialogue);
					if (dialogScript == null)
						Dynamic.InitializeMob();
					break;
				case EEnemyTileType.VarndagrothBoss:
					Level.MainHero.TeleportToPoint(new Point(200, 200));
					Dynamic._spindleItem.SilentKill();
					Dynamic.ChangeEyelidAnimation(VarndagrothEyeEnumType.GetEnumValue("Open"));
					Dynamic.StartBattle();
					break;
			}
		}

		public BossEnemy(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{	
		}

		void TeleportPlayer()
		{
			EDirection facing = vanillaBoss.IsFacingLeft ? EDirection.East : EDirection.West;
			Level.PlayCue(ESFX.FoleyWarpGyreOut);

			// Return room is credits if the final boss was defeated
			RoomItemKey returnRoom = isFinalBoss ? 
				new RoomItemKey(16, 27) : new RoomItemKey(vanillaBoss.ReturnRoom.LevelId, vanillaBoss.ReturnRoom.RoomId);

			Level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = returnRoom.LevelId,
				RoomID = returnRoom.RoomId,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				AdditionalBlackScreenTime = 0.5f,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f,
				ShouldPlayLevelSong = true,
				EnterDirection = facing
			});
		}

		protected override void OnUpdate()
		{
			if (isRandomized && !songHasRun && Scripts.Count == 0 && Level.JukeBox.CurrentSongEnum != vanillaBoss.Song)
			{
				Level.JukeBox.StopSong();
				Level.JukeBox.PlaySong(vanillaBoss.Song);
				songHasRun = true;
			}

			if (warpHasRun || Dynamic._deathScriptTimer <= 0) return;

			if (!clearedHasRun)
			{
				// Set AP goal state if this was the final boss
				if (isFinalBoss)
				{
					var fillingMethod = Level.GameSave.GetFillingMethod();

					if (fillingMethod == FillingMethod.Archipelago)
						Client.SetStatus(ArchipelagoClientState.ClientGoal);
				}
				clearedHasRun = true;
			};

			if (!saveHasRun)
			{
				BestiaryManager.SetBossKillSave(Level, vanillaBoss.Index);
				saveHasRun = true;
			}
			
			if (!isRandomized)
			{
				// Trigger teleport during Dad Percent to ensure "!" cutscene at game completion
				if (isDadFinalBoss && (vanillaBoss.Index == (int)EBossID.Nuvius || vanillaBoss.Index == (int)EBossID.Nightmare))
					TeleportPlayer();
				warpHasRun = true;
				return;
			}

			var visibleItems = (Dictionary<int, Item>)LevelReflected._items;
			if (visibleItems.Count > 0)
				foreach (var itemKey in visibleItems)
					itemKey.Value.Kill();

			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
			if (currentBoss.Index != (int)EBossID.Maw && !eventTypes.Contains(SandStreamerEventType) && !eventTypes.Contains(TimeBreakEventType))
				return;

			if (!nightmareRemoved)
			{
				if (currentBoss.Index == (int)EBossID.Nightmare)
					Dynamic.SilentKill();
				nightmareRemoved = true;
			}

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
			Level.JukeBox.StopAllSFX();
			Level.JukeBox.StopSong();
			if (Level.GameSave.GetSettings().BossHealing.Value)
			{
				LevelReflected.FullyHealPlayer();
				Level.MainHero.MP = Level.MainHero.MaxMP;
			}

			Level.GameSave.LastWarpLevel = lastWarpLevel;
			Level.GameSave.LastWarpRoom = lastWarpRoom;

			// Cause Time break
			if (vanillaBoss.ReturnRoom.LevelId == 15)
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
