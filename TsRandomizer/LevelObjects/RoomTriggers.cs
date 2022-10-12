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
using Timespinner.GameObjects.Events.EnvironmentPrefabs;
using Timespinner.GameObjects.Events;
using Timespinner.GameObjects.Events.Cutscene;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects
{
	class RoomTrigger
	{
		static readonly LookupDictionary<RoomItemKey, RoomTrigger> RoomTriggers = new LookupDictionary<RoomItemKey, RoomTrigger>(rt => rt.key);

		static readonly Type TransitionWarpEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionWarpEvent");
		static readonly Type GyreType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.GyrePortalEvent");
		static readonly Type NelisteNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.AstrologerNPC");
		static readonly Type YorneNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.YorneNPC");
		static readonly Type GlowingFloorEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabVilete");
		static readonly Type PedestalType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
		static readonly Type LakeVacuumLevelEffectType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.LakeVacuumLevelEffect");
		static readonly Type CutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");
		static readonly MethodInfo CreateAndCallCutsceneMethod = typeof(CutsceneBase).GetPrivateStaticMethod("CreateAndCallCutscene", CutsceneEnumType, typeof(Level), typeof(Point));
		static readonly Type CirclePlatformType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Platforms.CirclePlatformEvent");
		static readonly Type MovingPlatformType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Platforms.MovingPlatformEvent");


		static int TargetBossId = -1;

		static void SpawnBoss(Level level, SeedOptions seedOptions, int vanillaBossId)
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

		static void CreateBossWarp(Level level, int vanillaBossId)
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

		static RoomTrigger()
		{
			RoomTriggers.Add(new RoomTrigger(0, 3, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (seedOptions.StartWithJewelryBox)
					level.AsDynamic().UnlockRelic(EInventoryRelicType.JewelryBox);

				if (seedOptions.StartWithMeyef)
				{
					level.GameSave.AddItem(level, new ItemIdentifier(EInventoryFamiliarType.Meyef));
					level.GameSave.Inventory.EquippedFamiliar = EInventoryFamiliarType.Meyef;

					var luniasObject = level.MainHero.AsDynamic();
					var familiarManager = ((object)luniasObject._familiarManager).AsDynamic();

					familiarManager.ChangeFamiliar(EInventoryFamiliarType.Meyef);
					familiarManager.AddFamiliarPoofAnimation();
				}

				if (seedOptions.StartWithTalaria)
					level.AsDynamic().UnlockRelic(EInventoryRelicType.Dash);
			}));
			RoomTriggers.Add(new RoomTrigger(17, 8, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// False boss room
				SpawnBoss(level, seedOptions, TargetBossId);
			}));
			RoomTriggers.Add(new RoomTrigger(17, 13, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// False boss room
				SpawnBoss(level, seedOptions, TargetBossId);
			}));
			RoomTriggers.Add(new RoomTrigger(1, 0, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Inverted mode triggers
				if (!seedOptions.Inverted || level.GameSave.GetSaveBool("TSRandomizerHasTeleportedPlayer")) return;

				level.GameSave.SetValue("TSRandomizerHasTeleportedPlayer", true);
				level.GameSave.SetValue("HasUsedCityTS", true);
				level.GameSave.SetCutsceneTriggered("City1_Frame", true);
				level.GameSave.SetCutsceneTriggered("City2_Spindle", true);
				level.GameSave.SetCutsceneTriggered("City3_Warp", true);
				level.GameSave.SetCutsceneTriggered("LakeDesolation1_Entrance", true); // Fixes music when returning to Lake Desolation later

				level.RequestChangeLevel(new LevelChangeRequest { LevelID = 3, RoomID = 28 }); // Waterfall cutscene
			}));
			RoomTriggers.Add(new RoomTrigger(3, 28, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.Inverted) return;
				CreateAndCallCutsceneMethod.InvokeStatic(CutsceneEnumType.GetEnumValue("Forest0_Warp"), level, new Point(200, 200));
			}));
			RoomTriggers.Add(new RoomTrigger(1, 5, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Boots
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.FelineSentry);

				if (!itemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_RoboKitty")
					&& level.AsDynamic()._newObjects.Count == 0) // Orb Pedestal event
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 29, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Varndagroth
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Varndagroth);

				if (!itemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_Varndagroth"))
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 280, 222);
			}));
			RoomTriggers.Add(new RoomTrigger(7, 0, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Azure Queen
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.AzureQueen);
			}));
			RoomTriggers.Add(new RoomTrigger(6, 15, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Aelana
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Aelana);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 7, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Maw
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				if (seedOptions.GasMaw && !level.GameSave.GetSettings().BossRando.Value)
					FillRoomWithGas(level);
				CreateBossWarp(level, (int)EBossID.Maw);
			}));
			RoomTriggers.Add(new RoomTrigger(12, 20, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Nuvius
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Nuvius);
			}));
			RoomTriggers.Add(new RoomTrigger(13, 1, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Terrilis
				if (level.GameSave.GetSettings().BossRando.Value)
					CreateAndCallCutsceneMethod.InvokeStatic(CutsceneEnumType.GetEnumValue("Alt1_Vol"), level, new Point(200, 200));
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Vol);
			}));
			RoomTriggers.Add(new RoomTrigger(13, 0, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Prince
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Prince);
			}));
			RoomTriggers.Add(new RoomTrigger(9, 7, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Xarion
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Xarion);
			}));
			RoomTriggers.Add(new RoomTrigger(14, 4, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Ravenlord
				if (level.GameSave.GetSettings().BossRando.Value)
				{
					SpawnMovingPlatform(level, 185, 520);
					SpawnMovingPlatform(level, 615, 520);
					SpawnCirclePlatform(level, 185, 415, true);
					SpawnCirclePlatform(level, 615, 415, false);
					SpawnMovingPlatform(level, 185, 220);
					SpawnMovingPlatform(level, 615, 220);
				}

				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Ravenlord);
			}));
			RoomTriggers.Add(new RoomTrigger(14, 5, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Ifrit
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Ifrit);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 4, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Sandman
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Sandman);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 5, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Clear final boss saves
				level.GameSave.SetValue("TSRando_IsBossDead_Sandman", false);
				level.GameSave.SetValue("TSRando_IsBossDead_Nightmare", false);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 26, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Nightmare
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Nightmare);
			}));

			RoomTriggers.Add(new RoomTrigger(5, 5, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Golden Idol
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.GoldenIdol);

				if (!itemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_Demon")
					&& level.GameSave.HasRelic(EInventoryRelicType.DoubleJump))
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 200);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 21, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;
				CreateBossWarp(level, (int)EBossID.Genza);

				if (!itemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_Shapeshift")
					&& level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA))
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 200);

				if (!seedOptions.Inverted && level.GameSave.HasCutsceneBeenTriggered("Alt3_Teleport"))
					CreateSimpleOneWayWarp(level, 16, 12);
			}));
			RoomTriggers.Add(new RoomTrigger(7, 5, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				SpawnBoss(level, seedOptions, TargetBossId);
				if (level.GameSave.GetSaveBool("IsFightingBoss"))
					return;

				// Set Cantoran quest active when fighting Pink Bird
				if (!level.GameSave.GetSaveBool("TSRando_IsPinkBirdDead"))
				{
					level.GameSave.SetValue("TSRando_IsPinkBirdDead", true);
					BestiaryManager.RefreshBossSaveFlags(level);
					return;
				}

				if (!seedOptions.Cantoran)
					return;

				CreateBossWarp(level, (int)EBossID.Cantoran);

				if (!itemLocation.IsPickedUp
					&& level.GameSave.GetSaveBool("TSRando_IsBossDead_Cantoran")
					&& level.AsDynamic()._newObjects.Count == 0) // Orb Pedestal event
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 170, 194);
			}));

			RoomTriggers.Add(new RoomTrigger(11, 1, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp || !level.GameSave.HasRelic(EInventoryRelicType.Dash)) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 280, 191);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 39, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp
					|| !level.GameSave.HasOrb(EInventoryOrbType.Eye)
					|| !level.GameSave.GetSaveBool("11_LabPower")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 26, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp
					|| !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear1)) return;

				SpawnTreasureChest(level, true, 136, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 52, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp
					|| !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear2)) return;

				SpawnTreasureChest(level, true, 104, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(9, 13, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp
					|| !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear3)) return;

				SpawnTreasureChest(level, false, 296, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 6, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (seedOptions.Inverted || level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				CreateSimpleOneWayWarp(level, 2, 54);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 54, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (seedOptions.Inverted
					|| level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)
					|| !level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")) return;

				CreateSimpleOneWayWarp(level, 3, 6);
			}));
			RoomTriggers.Add(new RoomTrigger(7, 30, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				SpawnTreasureChest(level, false, 296, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 0, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (itemLocation.IsPickedUp
					|| level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")
					|| !level.GameSave.HasCutsceneBeenTriggered("Forest3_Haristel")
					|| ((Dictionary<int, NPCBase>)level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == NelisteNpcType)) return;

				SpawnNeliste(level);
			}));
			// Spawn Gyre portals when applicable
			RoomTriggers.Add(new RoomTrigger(11, 4, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GyreArchives || !level.GameSave.HasFamiliar(EInventoryFamiliarType.MerchantCrow))
					return;

				SpawnGyreWarp(level, 200, 200); // Historical Documents room to Ravenlord
			}));
			RoomTriggers.Add(new RoomTrigger(14, 8, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Reset boss save flags cleared by Gyre portal
				BestiaryManager.RefreshBossSaveFlags(level);
			}));
			RoomTriggers.Add(new RoomTrigger(14, 24, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GyreArchives)
					return;

				level.JukeBox.StopSong();

				level.RequestChangeLevel(new LevelChangeRequest
				{
					LevelID = 11,
					RoomID = 4,
					IsUsingWarp = true,
					IsUsingWhiteFadeOut = true,
					FadeInTime = 0.5f,
					FadeOutTime = 0.25f
				}); // Ravenlord to Historical Documents room

				level.JukeBox.PlaySong(Timespinner.GameAbstractions.EBGM.Level11);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 51, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GyreArchives) return;
				if (level.GameSave.HasFamiliar(EInventoryFamiliarType.Kobo))
				{
					SpawnGyreWarp(level, 200, 200); // Portrait room to Ifrit
					return;
				};

				if (((Dictionary<int, NPCBase>)level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == YorneNpcType)) return;
				SpawnYorne(level); // Dialog for needing Kobo
			}));
			RoomTriggers.Add(new RoomTrigger(14, 6, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Reset boss save flags cleared by Gyre portal
				BestiaryManager.RefreshBossSaveFlags(level);
			}));
			RoomTriggers.Add(new RoomTrigger(14, 25, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GyreArchives) return;
				level.JukeBox.StopSong();
				level.RequestChangeLevel(new LevelChangeRequest
				{
					LevelID = 2,
					RoomID = 51,
					IsUsingWarp = true,
					IsUsingWhiteFadeOut = true,
					FadeInTime = 0.5f,
					FadeOutTime = 0.25f
				}); // Ifrit to Portrait room
				level.JukeBox.PlaySong(Timespinner.GameAbstractions.EBGM.Library);
			}));
			RoomTriggers.Add(new RoomTrigger(10, 0, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Spawn warp after ship crashes
				if (!seedOptions.GyreArchives || !level.GameSave.GetSaveBool("IsPastCleared"))
					return;
				SpawnGyreWarp(level, 340, 180); // Military Hangar crash site to Gyre
			}));
			RoomTriggers.Add(new RoomTrigger(14, 11, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Play Gyre music in gyre
				level.JukeBox.PlaySong(Timespinner.GameAbstractions.EBGM.Level14);
				level.AsDynamic().SetLevelSaveInt("GyreDungeonSeed", (int)level.GameSave.GetSeed().Value.Id); // Set Gyre enemies
				BestiaryManager.RefreshBossSaveFlags(level); // Reset boss save flags cleared by Gyre portal
			    // Set last warp room visited to military hangar warp (instead of crash site)
				level.GameSave.LastWarpLevel = 10;
				level.GameSave.LastWarpRoom = 12;
			}));
			RoomTriggers.Add(new RoomTrigger(14, 23, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				level.JukeBox.StopSong();
				level.RequestChangeLevel(new LevelChangeRequest
				{
					LevelID = 10,
					RoomID = 0,
					IsUsingWarp = false,
					IsUsingWhiteFadeOut = true,
					FadeInTime = 0.5f,
					FadeOutTime = 0.25f
				}); // Military Hangar crash site
				level.JukeBox.PlaySong(Timespinner.GameAbstractions.EBGM.Level10);
			}));
			RoomTriggers.Add(new RoomTrigger(12, 11, (level, itemLocation, seedOptions, gameSettings, screenManager) => // Remove Daddy's pedestal if you havent killed him yet
			{
				if (level.GameSave.GetSaveBool("TSRando_IsBossDead_Emperor")) return;

				((Dictionary<int, GameEvent>)level.AsDynamic()._levelEvents).Values
					.FirstOrDefault(obj => obj.GetType() == PedestalType)
					?.SilentKill();
			}));
			RoomTriggers.Add(new RoomTrigger(8, 6, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GasMaw) return;

				FillRoomWithGas(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 13, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (seedOptions.GasMaw)
					FillRoomWithGas(level);
				if (!level.GameSave.GetSaveBool("TSRando_IsBossDead_Maw"))
					return;
				level.GameSave.SetValue("TSRando_IsVileteSaved", true);
				if (gameSettings.BossRando.Value
					&& !level.GameSave.GetSaveBool("IsVileteSaved"))
				{
					var enumValue = CutsceneEnumType.GetEnumValue("CavesPast6_MawBoom");
					CreateAndCallCutsceneMethod.InvokeStatic(enumValue, level, new Point(200, 200));
				}
				BestiaryManager.RefreshBossSaveFlags(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 21, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (seedOptions.GasMaw) FillRoomWithGas(level);

				var levelReflected = level.AsDynamic();
				IEnumerable<Animate> eventObjects = levelReflected._levelEvents.Values;

				if (!itemLocation.IsPickedUp &&
					!eventObjects.Any(o => o.GetType().ToString() ==
						"Timespinner.GameObjects.Events.EnvironmentPrefabs.EnvPrefabCavesRadiationCrystal"))
					SpawnItemDropPickup(level, itemLocation.ItemInfo, 312, 912);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 33, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				if (!seedOptions.GasMaw) return;

				FillRoomWithGas(level);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 1, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Allow the post-Nightmare chest to spawn
				level.GameSave.SetValue("IsGameCleared", true);
				level.GameSave.SetValue("IsEndingCDCleared", true);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 21, (level, itemLocation, seedOptions, gameSettings, screenManager) => {
				// Spawn glowing floor event to give a soft-lock exit warp
				if (((Dictionary<int, NPCBase>)level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == GlowingFloorEventType)) return;
				SpawnGlowingFloor(level);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 27, (level, itemLocation, seedOptions,  gameSettings, screenManager) =>
			{
				// Post-Nightmare void
				if (level.GameSave.GetSettings().BossRando.Value)
				{
					var enumValue = CutsceneEnumType.GetEnumValue("Temple2_End");
					CreateAndCallCutsceneMethod.InvokeStatic(enumValue, level, new Point(200, 200));
				}
				level.JukeBox.StopSong();

				if (!level.GameSave.DataKeyStrings.ContainsKey(ArchipelagoItemLocationRandomizer.GameSaveServerKey)) return;
				Client.SetStatus(ArchipelagoClientState.ClientGoal);
				AskPermissionMessage(screenManager, "collect", Client.CollectPermissions);
				AskPermissionMessage(screenManager, "forfeit", Client.ForfeitPermissions);
			}));
		}

		static void AskPermissionMessage(ScreenManager screenManager, string command, Permissions permissionFlags)
		{
			if (!permissionFlags.HasFlag(Permissions.Auto) &&
			    (permissionFlags.HasFlag(Permissions.Enabled) || permissionFlags.HasFlag(Permissions.Goal)))
			{
				var messageBox = MessageBox.Create(screenManager, $"Press OK to {command} remaining item checks", _ => {
					Client.Say($"!{command}");
				});

				screenManager.AddScreen(messageBox.Screen, null);
			}
		}

		readonly RoomItemKey key;
		readonly Action<Level, ItemLocation, SeedOptions, SettingCollection, ScreenManager> trigger;

		public RoomTrigger(int levelId, int roomId, Action<Level, ItemLocation, SeedOptions, SettingCollection, ScreenManager> triggerMethod)
		{
			key = new RoomItemKey(levelId, roomId);
			trigger = triggerMethod;
		}

		public static void OnChangeRoom(
			Level level, SeedOptions seedOptions, SettingCollection gameSettings, ItemLocationMap itemLocations, ScreenManager screenManager,
			int levelId, int roomId)
		{
			var roomKey = new RoomItemKey(levelId, roomId);

			if (RoomTriggers.TryGetValue(roomKey, out var trigger))
				trigger.trigger(level, itemLocations[roomKey], seedOptions, gameSettings, screenManager);
		}

		static void SpawnItemDropPickup(Level level, ItemInfo itemInfo, int x, int y)
		{
			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = new Point(x, y);
			var itemDropPickup = Activator.CreateInstance(itemDropPickupType, itemInfo.BestiaryItemDropSpecification, level, itemPosition, -1);

			var item = itemDropPickup.AsDynamic();
			item.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((Item)itemDropPickup);
		}

		static void SpawnTreasureChest(Level level, bool flipHorizontally, int x, int y)
		{
			var itemPosition = new Point(x, y);
			var specification = new ObjectTileSpecification { IsFlippedHorizontally = flipHorizontally, Layer = ETileLayerType.Objects };
			var treasureChest = new TreasureChestEvent(level, itemPosition, -1, specification);

			var chest = treasureChest.AsDynamic();
			chest.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject(treasureChest);
		}

		static void SpawnOrbPredestal(Level level, int x, int y)
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

		static void CreateSimpleOneWayWarp(Level level, int destinationLevelId, int destinationRoomId)
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

		static void SpawnNeliste(Level level)
		{
			var position = new Point(720, 368);
			var neliste = (NPCBase)NelisteNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(neliste);
		}

		static void SpawnGlowingFloor(Level level)
		{
			var position = new Point(100, 195);
			var floor = GlowingFloorEventType.CreateInstance(false, level, position, -1, new ObjectTileSpecification(), EEnvironmentPrefabType.L0_TableCake);

			level.AsDynamic().RequestAddObject(floor);
		}

		static void SpawnYorne(Level level)
		{
			var position = new Point(240, 215);
			var yorne = (NPCBase)YorneNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(yorne);
		}

		static void SpawnGyreWarp(Level level, int x, int y)
		{
			var position = new Point(x, y);
			var gyrePortal = GyreType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(gyrePortal);
		}

		static void SpawnMovingPlatform(Level level, int x, int y)
		{
			var position = new Point(x, y);
			var platform = MovingPlatformType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(platform);
		}

		static void SpawnCirclePlatform(Level level, int x, int y, bool isClockwise)
		{
			var position = new Point(x, y);
			ObjectTileSpecification platformTile = new ObjectTileSpecification();
			if (!isClockwise)
				platformTile.Argument = 1;
			var platform = CirclePlatformType.CreateInstance(false, level, position, -1, platformTile);

			level.AsDynamic().RequestAddObject(platform);
		}


		static void FillRoomWithGas(Level level)
		{
			var gas = (GameEvent)LakeVacuumLevelEffectType.CreateInstance(false, level, new Point(), -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(gas);

			var foreground = level.Foregrounds.FirstOrDefault();

			if (foreground == null)
				return;

			foreground.AsDynamic()._baseColor = new Color(8, 16, 2, 12);
			foreground.DrawColor = new Color(8, 16, 2, 12);
		}
	}
}
