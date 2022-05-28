using System;
using System.Collections.Generic;
using System.Linq;
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
		bool clearedHasRun;
		bool saveHasRun;
		bool warpHasRun;
		bool isRandomized;
		BossAttributes currentBoss;
		BossAttributes vanillaBoss;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");
		static readonly Type TimeBreakEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.BreakLevelEffect");
		static readonly Type MawSpitType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneCavesPast6");

		protected override void Initialize(SeedOptions options)
		{
			Level level = (Level)Dynamic._level;
			isRandomized = level.GameSave.GetSettings().BossRando.Value;
			int argument = 0;
			if (Dynamic.EnemyType == EEnemyTileType.EmperorBoss)
			{
				if (Dynamic._isPrinceEmperor)
					argument = 2;
				else if (Dynamic._isViletianEmperor)
					argument = 1;
			}

			var bestiaryEntry = level.GCM.Bestiary.GetEntry(Dynamic.EnemyType, argument);
			int bossId = bestiaryEntry.Index;

			currentBoss = BestiaryManager.GetBossAttributes(level, bossId);
			if (isRandomized)
				vanillaBoss = BestiaryManager.GetVanillaBoss(level, bossId);
			else
				vanillaBoss = currentBoss;

			if (!isRandomized)
				return;
			level.ToggleExits(false);
			level.OpenAllBossDoors(-1f);
			level.LockAllBossDoors(0.5f);
			level.AsDynamic().IsInBossRoom = true;

			level.JukeBox.StopSong();
			level.JukeBox.PlaySong(vanillaBoss.Song);
		}

		public BossEnemy(Mobile typedObject) : base(typedObject)
		{
			Level level = (Level)Dynamic._level;
			isRandomized = level.GameSave.GetSettings().BossRando.Value;
			if (!isRandomized || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			var boss = typedObject.AsDynamic();

			switch (boss.EnemyType)
			{
				case EEnemyTileType.EmperorBoss:
					if (boss._isPrinceEmperor)
					{
						boss.IsSpawnedForCutscene = false;
						boss.InitializeMob();
						boss.EndBossIntroCutscene();
					}	
					break;
				case EEnemyTileType.MawBoss:
					boss.InitializeMob();
					boss.EndBossIntroCutscene();
					boss.DoIntroCloseMouth();
					break;
				case EEnemyTileType.BirdBoss:
					boss.InitializeMob();
					boss.EndBossIntroCutscene();
					break;
				case EEnemyTileType.VarndagrothBoss:
					level.SetPlayerPosition(new Point(100, 200));
					boss._spindleItem.SilentKill();
					boss.StartBattle();
					break;
				default:
					break;
			}
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (warpHasRun || Dynamic._deathScriptTimer <= 0) return;

			Level level = (Level)Dynamic._level;
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
				BestiaryManager.SetBossKillSave(level, vanillaBoss.Index);
				saveHasRun = true;
			}

			if (!isRandomized)
			{
				warpHasRun = true;
				return;
			}

			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());

			if (currentBoss.Index != 70 && !eventTypes.Contains(SandStreamerEventType) && !eventTypes.Contains(TimeBreakEventType))
				return;

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
			level.JukeBox.StopAllSFX();
			level.JukeBox.StopSong();

			// Cause Time break
			if (vanillaBoss.ReturnRoom.LevelId == 15)
			{
				warpHasRun = true;
				var cutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");
				var createAndCallCutsceneMethod = typeof(CutsceneBase).GetPrivateStaticMethod("CreateAndCallCutscene", cutsceneEnumType, typeof(Level), typeof(Point));

				var enumValue = cutsceneEnumType.GetEnumValue("Alt2_Win");
				createAndCallCutsceneMethod.InvokeStatic(enumValue, level, new Point(200, 200));
				return;
			}

			EDirection facing = vanillaBoss.IsFacingLeft ? EDirection.East : EDirection.West;
			level.PlayCue(ESFX.FoleyWarpGyreOut);

			level.RequestChangeLevel(new LevelChangeRequest
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

			warpHasRun = true;
		}
	}
}

// Genza information to fully incorporate
/*
[TimeSpinnerType("Timespinner.GameObjects.Bosses.ShapeshifterBoss")]
// ReSharper disable once UnusedMember.Global
class ShapeshifterBoss : ItemManipulator
{
	bool hasReplacedItemScript;

	static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");

	public ShapeshifterBoss(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
	{
	}

	protected override void OnUpdate(GameplayScreen gameplayScreen)
	{
		if (ItemInfo == null || hasReplacedItemScript || (Point)Dynamic.DeathPosition == Point.Zero)
			return;

		var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
		if (!eventTypes.Contains(SandStreamerEventType))
			return;

		if (((Dictionary<int, Item>)LevelReflected._items).Count == 0)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = (Point) Dynamic.Position;
			var itemDropPickup = Activator.CreateInstance(itemDropPickupType, ItemInfo.BestiaryItemDropSpecification, Level, itemPosition, -1);

			LevelReflected.RequestAddObject((Item)itemDropPickup);
		}

		hasReplacedItemScript = true;
	}
}
*/