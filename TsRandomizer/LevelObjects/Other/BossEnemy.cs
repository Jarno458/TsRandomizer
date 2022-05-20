using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Timespinner.Core.Specifications;
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
		bool hasRun;
		BossAttributes currentBoss;
		BossAttributes vanillaBoss;

		static readonly Type SandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");
		static readonly Type TimeBreakEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.BreakLevelEffect");
		static readonly Type MawSuckType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.BreakLevelEffect");

		protected override void Initialize(SeedOptions options)
		{
			Level level = (Level)Dynamic._level;
			if (level.RoomID != 2)
			{
				// TODO fix this check for room 0, 2
				return;
			}
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

			vanillaBoss = BestiaryManager.GetVanillaBoss(level, bossId);
		}

		public BossEnemy(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (hasRun || Dynamic._deathScriptTimer <= 0) return;

			Level level = (Level)Dynamic._level;
			if (vanillaBoss.Index == 80)
			{
				// Boss is Nightmare
				var fillingMethod = Level.GameSave.GetFillingMethod();

				if (fillingMethod == FillingMethod.Archipelago)
					Client.SetStatus(ArchipelagoClientState.ClientGoal);

				//hasRun = true;
				//return;
			};

			if (level.RoomID != 2)
				return;

			// TODO: handle these transitions cleaner
			var eventTypes = ((Dictionary<int, GameEvent>)LevelReflected._levelEvents).Values.Select(e => e.GetType());
			if (currentBoss.Index != 70 && !eventTypes.Contains(SandStreamerEventType) && !eventTypes.Contains(TimeBreakEventType))
				return;

			BestiaryManager.SetBossKillSave(level, vanillaBoss.Index);

			level.RequestChangeLevel(new LevelChangeRequest
			{
				LevelID = vanillaBoss.ReturnRoom.LevelId,
				RoomID = vanillaBoss.ReturnRoom.RoomId,
				IsUsingWarp = true,
				IsUsingWhiteFadeOut = true,
				FadeInTime = 0.5f,
				FadeOutTime = 0.25f
			});
			level.PlayLevelSong();

			hasRun = true;
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