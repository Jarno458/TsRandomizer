using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Randomisation
{
	class ItemUnlockingMap
	{
		static readonly TeleporterGate[] PresentTeleporterGates =
		{
			new TeleporterGate{Gate = R.GateKittyBoss, LevelId = 2, RoomId = 55},
			new TeleporterGate{Gate = R.GateLeftLibrary, LevelId = 2, RoomId = 54},
			new TeleporterGate{Gate = R.GateMilitaryGate, LevelId = 10, RoomId = 12},
			new TeleporterGate{Gate = R.GateSealedCaves, LevelId = 9, RoomId = 50},
			//new TeleporterGate{Gate = R.GateXarion, LevelId = 9, RoomId = 49}, //dont want to spawn infront of xarion
			new TeleporterGate{Gate = R.GateSealedSirensCave, LevelId = 9, RoomId = 51},
			new TeleporterGate{Gate = R.GateLakeDesolation, LevelId = 1, RoomId = 25},
		};

		static readonly TeleporterGate[] PastTeleporterGates =
		{
			//new TeleporterGate{Gate = Requirement.GateLakeSirineLeft, LevelId = 7, RoomId = 30}, //you dont want to spawn with a boss in your face
			new TeleporterGate{Gate = R.GateLakeSereneRight, LevelId = 7, RoomId = 31},
			new TeleporterGate{Gate = R.GateAccessToPast, LevelId = 8, RoomId = 51},
			//new TeleporterGate{Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6}, //Refugee Camp, Somehow doesnt work ¯\_(ツ)_/¯
			new TeleporterGate{Gate = R.GateCastleRamparts, LevelId = 4, RoomId = 23},
			new TeleporterGate{Gate = R.GateCastleKeep, LevelId = 5, RoomId = 24},
			new TeleporterGate{Gate = R.GateRoyalTowers, LevelId = 6, RoomId = 0},
			new TeleporterGate{Gate = R.GateMaw, LevelId = 8, RoomId = 49},
			new TeleporterGate{Gate = R.GateCavesOfBanishment, LevelId = 8, RoomId = 50},
		};

		readonly LookupDictionairy<ItemIdentifier, UnlockingSpecification> unlockingSpecifications;

		public R AllUnlockableRequirements => unlockingSpecifications.Aggregate(R.None, (a, b) => a | b.AllUnlocks);
		public IEnumerable<ItemIdentifier> AllProgressionItems => unlockingSpecifications.Select(us => us.Item);
		public R PyramidKeysUnlock => unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.PyramidsKey)].Unlocks;

		public ItemUnlockingMap(Seed seed)
		{
			var random = new Random((int)seed.Id);

			unlockingSpecifications = new LookupDictionairy<ItemIdentifier, UnlockingSpecification>(26, s => s.Item)
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
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.AirMask), R.GassMask),
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.Tablet), R.Tablet),
				new UnlockingSpecification(new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Passive), R.OculusRift),
				new UnlockingSpecification(new ItemIdentifier(EInventoryFamiliarType.Kobo), R.Kobo),
				new UnlockingSpecification(new ItemIdentifier(EInventoryFamiliarType.MerchantCrow), R.MerchantCrow),
			};

			if (seed.Options.SpecificKeys)
				MakeKeyCardUnlocksCardSpecific();

			var pyramidUnlockingSpecification =
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.PyramidsKey), R.None, R.Teleport);

			SetTeleporterPickupAction(random, pyramidUnlockingSpecification, seed.Options);

			unlockingSpecifications.Add(pyramidUnlockingSpecification);
		}

		void MakeKeyCardUnlocksCardSpecific()
		{
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardA)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardB)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardC)].AdditionalUnlocks = R.None;
			unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.ScienceKeycardD)].AdditionalUnlocks = R.None;
		}

		static void SetTeleporterPickupAction(Random random, UnlockingSpecification unlockingSpecification, SeedOptions options)
		{
			var teleporterGates = (!options.Inverted)
				? PresentTeleporterGates.Union(PastTeleporterGates)
				: PresentTeleporterGates;

			var selectedGate = teleporterGates.SelectRandom(random);

			unlockingSpecification.OnPickup = level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);
			unlockingSpecification.Unlocks = selectedGate.Gate;
		}

		public void SetTeleporterPickupAction(R requirement)
		{
			var selectedGate = PresentTeleporterGates
				.Union(PastTeleporterGates)
				.First(g => g.Gate == requirement);

			var unlockingSpecification = unlockingSpecifications[new ItemIdentifier(EInventoryRelicType.PyramidsKey)];

			unlockingSpecification.OnPickup = level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);
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
		}
	}
}
