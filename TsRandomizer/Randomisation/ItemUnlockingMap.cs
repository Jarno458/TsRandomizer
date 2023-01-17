using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.RoomTriggers.Triggers;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation
{
	class ItemUnlockingMap
	{
		static readonly TeleporterGate[] PresentTeleporterGates =
		{
			new TeleporterGate{Gate = R.GateKittyBoss, LevelId = 2, RoomId = 55, Name = "Sewers"},
			new TeleporterGate{Gate = R.GateLeftLibrary, LevelId = 2, RoomId = 54, Name = "Library"},
			new TeleporterGate{Gate = R.GateMilitaryGate, LevelId = 10, RoomId = 12, Name = "Military Hangar"},
			new TeleporterGate{Gate = R.GateSealedCaves, LevelId = 9, RoomId = 50, Name = "Xarion's Cave Entrance"},
			//new TeleporterGate{Gate = R.GateXarion, LevelId = 9, RoomId = 49}, //dont want to spawn infront of xarion
			new TeleporterGate{Gate = R.GateSealedSirensCave, LevelId = 9, RoomId = 51, Name = "Sirens' Cave"},
			new TeleporterGate{Gate = R.GateLakeDesolation, LevelId = 1, RoomId = 25, Name = "Lake Desolation"},
		};

		static readonly TeleporterGate[] PastTeleporterGates =
		{
			//new TeleporterGate{Gate = Requirement.GateLakeSirineLeft, LevelId = 7, RoomId = 30}, //you dont want to spawn with a boss in your face
			new TeleporterGate{Gate = R.GateLakeSereneRight, LevelId = 7, RoomId = 31, Name = "East Lake Serene"},
			new TeleporterGate{Gate = R.GateAccessToPast, LevelId = 8, RoomId = 51, Name = "Upper Caves of Banishment"},
			//new TeleporterGate{Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6}, //Refugee Camp, Somehow doesnt work ¯\_(ツ)_/¯
			new TeleporterGate{Gate = R.GateCastleRamparts, LevelId = 4, RoomId = 23, Name = "Castle Ramparts"},
			new TeleporterGate{Gate = R.GateCastleKeep, LevelId = 5, RoomId = 24, Name = "Castle Keep"},
			new TeleporterGate{Gate = R.GateRoyalTowers, LevelId = 6, RoomId = 0, Name = "Royal Towers"},
			new TeleporterGate{Gate = R.GateMaw, LevelId = 8, RoomId = 49, Name = "Maw's Lair"},
			new TeleporterGate{Gate = R.GateCavesOfBanishment, LevelId = 8, RoomId = 50, Name = "Maw's Cave Entrance"},
		};

		static readonly TeleporterGate[] PyramidTeleporterGates =
		{
			new TeleporterGate{Gate = R.GateGyre, LevelId = 14, RoomId = 1, Name = "Temporal Gyre Entrance"},
			new TeleporterGate{Gate = R.GateLeftPyramid, LevelId = 16, RoomId = 12, Name = "Ancient Pyramid Entrance"},
			new TeleporterGate{Gate = R.GateRightPyramid, LevelId = 16, RoomId = 19, Name = "Inner Ancient Pyramid"},
		};

		readonly LookupDictionary<ItemIdentifier, UnlockingSpecification> unlockingSpecifications;

		public R AllUnlockableRequirements => unlockingSpecifications.Aggregate(R.None, (a, b) => a | b.AllUnlocks);
		public IEnumerable<ItemIdentifier> AllProgressionItems => unlockingSpecifications.Select(us => us.Item);
		public R PyramidKeysUnlock => unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.PyramidsKey)].Unlocks;

		public ItemUnlockingMap(Seed seed)
		{
			var random = new Random((int)seed.Id);

			unlockingSpecifications = new LookupDictionary<ItemIdentifier, UnlockingSpecification>(29, s => s.Item)
			{
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.TimespinnerWheel), R.TimespinnerWheel, R.TimeStop),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.DoubleJump), R.DoubleJump, R.TimeStop),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.Dash), R.ForwardDash),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Passive), R.AntiWeed),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Melee), R.AntiWeed),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Spell), R.AntiWeed),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell), R.AntiWeed),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ScienceKeycardA), R.CardA, R.CardB | R.CardC | R.CardD),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ScienceKeycardB), R.CardB, R.CardC | R.CardD),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ScienceKeycardC), R.CardC, R.CardD),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ScienceKeycardD), R.CardD),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ElevatorKeycard), R.CardE),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.ScienceKeycardV), R.CardV),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.WaterMask), R.Swimming),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.TimespinnerSpindle), R.TimespinnerSpindle),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.TimespinnerGear1), R.TimespinnerPiece1),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.TimespinnerGear2), R.TimespinnerPiece2),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.TimespinnerGear3), R.TimespinnerPiece3),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.EssenceOfSpace), R.UpwardDash, R.DoubleJump | R.TimeStop),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell), R.UpwardDash, R.DoubleJump | R.TimeStop),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Melee), R.PinkOrb),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Spell), R.PinkOrb),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Passive), R.PinkOrb),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.AirMask), R.GasMask),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.Tablet), R.Tablet),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Passive), R.OculusRift),
				new UnlockingSpecification(new ItemIdentifier(EInventoryFamiliarType.Kobo), R.Kobo),
				new UnlockingSpecification(new ItemIdentifier(EInventoryFamiliarType.MerchantCrow), R.MerchantCrow),
			};

			if (seed.Options.SpecificKeys)
				MakeKeyCardUnlocksCardSpecific();

			var pyramidUnlockingSpecification =
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.PyramidsKey), R.None, R.Teleport);

			SetTeleporterPickupAction(random, pyramidUnlockingSpecification, seed);

			unlockingSpecifications.Add(pyramidUnlockingSpecification);

			if (seed.Options.UnchainedKeys)
				SetMapRevealPickupAction(random, seed.Options);
		}

		void SetMapRevealPickupAction(Random random, SeedOptions seedOptions) {
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal0", "Timeworn Warp Beacon");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal0_desc", "Attunes warps to a gate in the past");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal1", "Modern Warp Beacon");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal1_desc", "Attunes warps gate within the present");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal2", "Mysterious Warp Beacon");
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal2_desc", "Attunes warps to a gate beyond time");

			var pastWarpUnlockingSpecification = new UnlockingSpecification(new ItemIdentifier(EInventoryUseItemType.MapReveal0), R.PastWarp);
			var presentWarpUnlockingSpecification = new UnlockingSpecification(new ItemIdentifier(EInventoryUseItemType.MapReveal1), R.PresentWarp);

			var pastGate = PastTeleporterGates.SelectRandom(random);
			var presentGate = PresentTeleporterGates.SelectRandom(random);

			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal0_desc", "You feel the pyramid keys attune to: " + pastGate.Name);
			TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal1_desc", "You feel the pyramid keys attune to: " + presentGate.Name);

			pastWarpUnlockingSpecification.OnPickup = level => {
				UnlockRoom(level, pastGate.LevelId, pastGate.RoomId);
				level.ShowGhostDialogueMessage("inv_use_MapReveal0_desc");
			};
			pastWarpUnlockingSpecification.Unlocks = pastGate.Gate;

			presentWarpUnlockingSpecification.OnPickup = level => {
				UnlockRoom(level, presentGate.LevelId, presentGate.RoomId);
				level.ShowGhostDialogueMessage("inv_use_MapReveal1_desc");
			};
			presentWarpUnlockingSpecification.Unlocks = presentGate.Gate;

			unlockingSpecifications.Add(pastWarpUnlockingSpecification);
			unlockingSpecifications.Add(presentWarpUnlockingSpecification);


			if (seedOptions.EnterSandman)
			{
				var pyramidWarpUnlockingSpecification = new UnlockingSpecification(new ItemIdentifier(EInventoryUseItemType.MapReveal2), R.PyramidWarp);
				var pyramidGate = PyramidTeleporterGates.SelectRandom(random);
				TimeSpinnerGame.Localizer.OverrideKey("inv_use_MapReveal2_desc", "You feel the pyramid keys attune to: " +  pyramidGate.Name);


				pyramidWarpUnlockingSpecification.OnPickup = level => {
					UnlockRoom(level, pyramidGate.LevelId, pyramidGate.RoomId);
					level.ShowGhostDialogueMessage("inv_use_MapReveal2_desc");
				};
				pyramidWarpUnlockingSpecification.Unlocks = pyramidGate.Gate;
				unlockingSpecifications.Add(pyramidWarpUnlockingSpecification);
			}
			
			
		}

		void MakeKeyCardUnlocksCardSpecific()
		{
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardA)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardB)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardC)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardD)].AdditionalUnlocks = R.None;
		}

		static void SetTeleporterPickupAction(Random random, UnlockingSpecification unlockingSpecification, Seed seed)
		{
			if (seed.Options.UnchainedKeys)
				return;
			IEnumerable<TeleporterGate> teleporterGates = PresentTeleporterGates;

			if (!seed.Options.Inverted)
			{
				IEnumerable<TeleporterGate> pastTeleporterGates = PastTeleporterGates;

				if (seed.FloodFlags.Maw)
					pastTeleporterGates = pastTeleporterGates.Where(g => g.Gate != R.GateMaw);

				teleporterGates = teleporterGates.Union(pastTeleporterGates);
			}

			var selectedGate = teleporterGates.SelectRandom(random);

			unlockingSpecification.OnPickup = level => {
				UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);

				if (seed.Options.EnterSandman) 
					UnlockFirstPyramidPortal(level);
					unlockingSpecification.AdditionalUnlocks = PyramidTeleporterGates[1].Gate;
				}
					
			};
			unlockingSpecification.Unlocks = selectedGate.Gate;
		}

		public void SetTeleporterPickupAction(R requirement, SeedOptions seedOptions)
		{
			if (seedOptions.UnchainedKeys)
				return;
			var selectedGate = PresentTeleporterGates
				.Union(PastTeleporterGates)
				.First(g => g.Gate == requirement);

			var unlockingSpecification = unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.PyramidsKey)];

			unlockingSpecification.OnPickup = level => {
				UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);
				if (seedOptions.EnterSandman)
				{
					UnlockFirstPyramidPortal(level);
					unlockingSpecification.AdditionalUnlocks = PyramidTeleporterGates[1].Gate;
				}
			};
			unlockingSpecification.Unlocks = selectedGate.Gate;
		}

		public R GetUnlock(ItemIdentifier identifier) =>
			unlockingSpecifications.TryGetValue(identifier, out var value)
				? value.Unlocks
				: R.None;

		public R GetAllUnlock(ItemIdentifier identifier) =>
			unlockingSpecifications.TryGetValue(identifier, out var value)
				? value.AllUnlocks
				: R.None;
		public Action<Level> GetPickupAction(ItemIdentifier identifier) =>
			unlockingSpecifications.TryGetValue(identifier, out var value)
				? value.OnPickup
				: null;

		public IEnumerable<ItemIdentifier> AllItemThatUnlockProgression(R requirement) =>
			unlockingSpecifications
				.Where(s => s.AllUnlocks.Contains(requirement))
				.Select(s => s.Item);

		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}

		static void UnlockFirstPyramidPortal(Level level)
		{
			UnlockRoom(level, 16, 12);
		}

		class UnlockingSpecification
		{
			public ItemIdentifier Item { get; }
			public R Unlocks { get; internal set; }
			public R AdditionalUnlocks { get; internal set; }
			public R AllUnlocks => Unlocks | AdditionalUnlocks;
			public Action<Level> OnPickup { get; internal set; }

			public UnlockingSpecification(ItemIdentifier item, R unlocks, R? additionalUnlocks = null)
			{
				Item = item;
				Unlocks = unlocks;
				AdditionalUnlocks = additionalUnlocks ?? R.None;
			}
		}

		class TeleporterGate
		{
			public R Gate { get; internal set; }
			public int LevelId { get; internal set; }
			public int RoomId { get; internal set; }
			public string Name { get; internal set; }
		}
	}
}
