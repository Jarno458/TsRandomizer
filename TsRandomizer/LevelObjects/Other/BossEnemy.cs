using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
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
	// [TimeSpinnerType("Timespinner.GameObjects.Bosses.ShapeshifterBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Emperor.EmperorBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.XarionBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.Z_Raven.RavenBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.ZelBoss")]
	[TimeSpinnerType("Timespinner.GameAbstractions.GameObjects.SandmanBoss")]
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss")]
	// ReSharper disable once UnusedMember.Global
	class BossEnemy: LevelObject
	{
		bool hasRun;
		BossAttributes currentBoss;
		BossAttributes vanillaBoss;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");

		protected override void Initialize(SeedOptions options)
		{
			Level level = (Level)Dynamic._level;
			// TODO account for enemy argument
			var bestiaryEntry = level.GCM.Bestiary.GetEntry(Dynamic.EnemyType, 0);
			int bossId = bestiaryEntry.Index;
			currentBoss = BestiaryManager.GetBossAttributes(level, bossId);

			vanillaBoss = BestiaryManager.GetVanillaBoss(level, options, bossId);
		}

		public BossEnemy(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (hasRun || Dynamic._deathScriptTimer <= 0) return;

			Level level = (Level)Dynamic._level;
			// TODO: alter this check for being in a debug room
			if (level.RoomID == 26)
			{
				// Boss is Nightmare
				var fillingMethod = Level.GameSave.GetFillingMethod();

				if (fillingMethod == FillingMethod.Archipelago)
					Client.SetStatus(ArchipelagoClientState.ClientGoal);

				hasRun = true;
				return;
			};

			if (level.RoomID != 2)
				return;

			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
			if (!eventTypes.Contains(SandStreamerEventType))
				return;

			BestiaryManager.SetBossKillSave(level, vanillaBoss.Index);

			level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = vanillaBoss.RoomKey.LevelId,
				RoomID = vanillaBoss.RoomKey.RoomId,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f
			});

			hasRun = true;
		}
	}
}
