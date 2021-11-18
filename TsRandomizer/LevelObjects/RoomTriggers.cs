using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects
{
	class RoomTrigger
	{
		static readonly LookupDictionairy<RoomItemKey, RoomTrigger> RoomTriggers = new LookupDictionairy<RoomItemKey, RoomTrigger>(rt => rt.key);

		static readonly Type TransitionWarpEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.TransitionWarpEvent");
		static readonly Type GyreType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Doors.GyrePortalEvent");
		static readonly Type NelisteNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.AstrologerNPC");
		static readonly Type PedistalType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
		static readonly Type LakeVacuumLevelEffectType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.LevelEffects.LakeVacuumLevelEffect");

		static RoomTrigger()
		{
			RoomTriggers.Add(new RoomTrigger(0, 3, (level, itemLocation, seedOptions, screenManager) =>
			{
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
			RoomTriggers.Add(new RoomTrigger(1, 0, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.Inverted || level.GameSave.GetSaveBool("TSRandomizerHasTeleportedPlayer")) return;

				level.GameSave.SetValue("TSRandomizerHasTeleportedPlayer", true);

				level.RequestChangeLevel(new LevelChangeRequest { LevelID = 3, RoomID = 6 }); //Refugee Camp
			}));
			RoomTriggers.Add(new RoomTrigger(1, 5, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.GetSaveBool("IsBossDead_RoboKitty")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(5, 5, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.HasCutsceneBeenTriggered("Keep0_Demons0")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 1, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp || !level.GameSave.HasRelic(EInventoryRelicType.Dash)) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 280, 191);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 39, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp 
					|| !level.GameSave.HasOrb(EInventoryOrbType.Eye)
				    || !level.GameSave.GetSaveBool("11_LabPower")) return;

				SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 21, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!itemLocation.IsPickedUp
				    && level.GameSave.HasRelic(EInventoryRelicType.ScienceKeycardA)
				    && level.GameSave.GetSaveBool("IsBossDead_Shapeshift")) 
						SpawnItemDropPickup(level, itemLocation.ItemInfo, 200, 208);

				if(!seedOptions.Inverted && level.GameSave.HasCutsceneBeenTriggered("Alt3_Teleport"))
					CreateSimpelOneWayWarp(level, 16, 12);
			}));
			RoomTriggers.Add(new RoomTrigger(11, 26, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear1)) return;

				SpawnTreasureChest(level, true, 136, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 52, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear2)) return;

				SpawnTreasureChest(level, true, 104, 192);
			}));
			RoomTriggers.Add(new RoomTrigger(9, 13, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp
				    || !level.GameSave.HasRelic(EInventoryRelicType.TimespinnerGear3)) return;

				SpawnTreasureChest(level, false, 296, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 6, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (seedOptions.Inverted || level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				CreateSimpelOneWayWarp(level, 2, 54);
			}));
			RoomTriggers.Add(new RoomTrigger(2, 54, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (seedOptions.Inverted 
				    || level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)
					|| !level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")) return;

				CreateSimpelOneWayWarp(level, 3, 6);
			}));
			RoomTriggers.Add(new RoomTrigger(7, 30, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!level.GameSave.HasRelic(EInventoryRelicType.PyramidsKey)) return;

				SpawnTreasureChest(level, false, 296, 176);
			}));
			RoomTriggers.Add(new RoomTrigger(3, 0, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (itemLocation.IsPickedUp
					|| level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")
					|| !level.GameSave.HasCutsceneBeenTriggered("Forest3_Haristel")
				    || ((Dictionary<int, NPCBase>)level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == NelisteNpcType)) return;

				SpawnNeliste(level);
			}));
			// Placeholder Gyre hooks TODO make objects for these
			RoomTriggers.Add(new RoomTrigger(11, 4, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GyreArchives || !level.GameSave.HasFamiliar(EInventoryFamiliarType.MerchantCrow)) return;
				SpawnGyreWarp(level, 14, 8); // lab to ravenlord
			}));
			RoomTriggers.Add(new RoomTrigger(14, 24, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GyreArchives) return;
				SpawnGyreWarp(level, 11, 6); // ravenlord to lab
			}));
			RoomTriggers.Add(new RoomTrigger(2, 51, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GyreArchives || !level.GameSave.HasFamiliar(EInventoryFamiliarType.Kobo)) return;
				SpawnGyreWarp(level, 14, 6); // backer room to Ifrit
			}));
			RoomTriggers.Add(new RoomTrigger(14, 25, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GyreArchives) return;
				SpawnGyreWarp(level, 2, 3); // Ifrit to shop
			}));
			RoomTriggers.Add(new RoomTrigger(14, 23, (level, itemLocation, seedOptions, screenManager) =>
			{
				SpawnGyreWarp(level, 14, 0); // gyre closed loop
			}));
			RoomTriggers.Add(new RoomTrigger(12, 11, (level, itemLocation, seedOptions, screenManager) => //Remove Daddy's pedistal if you havent killed him yet
			{
				if (level.GameSave.DataKeyBools.ContainsKey("IsEndingABCleared")) return;

				((Dictionary<int, GameEvent>)level.AsDynamic()._levelEvents).Values
					.FirstOrDefault(obj => obj.GetType() == PedistalType)
					?.SilentKill();
			}));
			RoomTriggers.Add(new RoomTrigger(8, 6, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GassMaw) return;

				FillRoomWithGass(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 7, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GassMaw) return;

				FillRoomWithGass(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 13, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GassMaw) return;

				FillRoomWithGass(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 21, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GassMaw) return;

				FillRoomWithGass(level);
			}));
			RoomTriggers.Add(new RoomTrigger(8, 33, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!seedOptions.GassMaw) return;

				FillRoomWithGass(level);
			}));
			RoomTriggers.Add(new RoomTrigger(16, 27, (level, itemLocation, seedOptions, screenManager) =>
			{
				if (!level.GameSave.DataKeyStrings.ContainsKey(ArchipelagoItemLocationRandomizer.GameSaveServerKey)) return;

				var forfeitFlags = Client.ForfeitPermissions;

				if (!forfeitFlags.HasFlag(Permissions.Auto) &&
				    (forfeitFlags.HasFlag(Permissions.Enabled) || forfeitFlags.HasFlag(Permissions.Goal)))
				{
					var messageBox = MessageBox.Create(screenManager, "Press OK for forfeit remaining item checks", _ => {
						Client.Forfeit();
					});

					screenManager.AddScreen(messageBox.Screen, null);
				}
			}));
		}

		readonly RoomItemKey key;
		readonly Action<Level, ItemLocation, SeedOptions, ScreenManager> trigger;

		public RoomTrigger(int levelId, int roomId, Action<Level, ItemLocation, SeedOptions, ScreenManager> triggerMethod)
		{
			key = new RoomItemKey(levelId, roomId);
			trigger = triggerMethod;
		}

		public static void OnChangeRoom(
			Level level, SeedOptions seedOptions, ItemLocationMap itemLocations, ScreenManager screenManager,
			int levelId, int roomId)
		{
			var roomKey = new RoomItemKey(levelId, roomId);

			if(RoomTriggers.TryGetValue(roomKey, out var trigger))
				trigger.trigger(level, itemLocations[roomKey], seedOptions, screenManager);
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
			var specification = new ObjectTileSpecification {IsFlippedHorizontally = flipHorizontally, Layer = ETileLayerType.Objects};
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
				ID = 480, //orb pedistal
				Layer = ETileLayerType.Objects,
			};
			var orbPedestalEvent = Activator.CreateInstance(orbPedestalEventType, level, itemPosition, -1, ObjectTileSpecification.FromTileSpecification(pedistalSpecification));

			var pedestal = orbPedestalEvent.AsDynamic();
			pedestal.DoesSpawnDespiteBeingOwned = true;
			pedestal.Initialize();

			var levelReflected = level.AsDynamic();
			levelReflected.RequestAddObject((GameEvent)orbPedestalEvent);
		}

		static void CreateSimpelOneWayWarp(Level level, int destinationLevelId, int destinationRoomId)
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

		static void SpawnGyreWarp(Level level, int LevelId, int RoomId)
		{
			level.RequestChangeLevel(new LevelChangeRequest { LevelID = LevelId, RoomID = RoomId });
		}

		static void FillRoomWithGass(Level level)
		{
			var gass = (GameEvent)LakeVacuumLevelEffectType.CreateInstance(false, level, new Point(), -1, new ObjectTileSpecification());
			
			level.AsDynamic().RequestAddObject(gass);

			var foreground = level.Foregrounds.FirstOrDefault();

			if(foreground == null)
				return;

			foreground.AsDynamic()._baseColor = new Color(8, 16, 2, 12);
			foreground.DrawColor = new Color(8, 16, 2, 12);
		}
	}
}
