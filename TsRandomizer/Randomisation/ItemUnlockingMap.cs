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
		public const int ProgressionItemCount = 25;

		public List<UnlockingSpecification> UnlockingSpecifications;

		public R AllUnlockableRequirements => UnlockingSpecifications.Aggregate(R.None, (a, b) => a | b.AllUnlocks);
		public IEnumerable<ItemIdentifier> ItemsThatUnlockProgression => UnlockingSpecifications.Select(us => us.Item);
		public R PyramidKeysUnlock => UnlockingSpecifications.Single(us => us.Item == new ItemIdentifier(EInventoryRelicType.PyramidsKey)).Unlocks;

		public ItemUnlockingMap(Seed seed)
		{
			var random = new Random((int)seed.Id);

			UnlockingSpecifications = new List<UnlockingSpecification>(ProgressionItemCount)
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
			};

			var pyramidUnlockingSpecification = 
				new UnlockingSpecification(new ItemIdentifier(EInventoryRelicType.PyramidsKey), R.None, R.Teleport);

			SetTeleporterPickupAction(random, pyramidUnlockingSpecification);

			UnlockingSpecifications.Add(pyramidUnlockingSpecification);
		}

		void SetTeleporterPickupAction(Random random, UnlockingSpecification unlockingSpecification)
		{
			var gateProgressionItems = new[] {
				new {Gate = R.GateKittyBoss, LevelId = 2, RoomId = 55},
				new {Gate = R.GateLeftLibrary, LevelId = 2, RoomId = 54},
				//new {Gate = Requirement.GateLakeSirineLeft, LevelId = 7, RoomId = 30}, //you dont want to spawn with a boss in your face
				new {Gate = R.GateLakeSirineRight, LevelId = 7, RoomId = 31},
				//new {Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6}, //Refugee Camp, Somehow doesnt work ¯\_(ツ)_/¯
				new {Gate = R.GateAccessToPast, LevelId = 8, RoomId = 51},
				new {Gate = R.GateMilitairyGate, LevelId = 10, RoomId = 12},
				new {Gate = R.GateCastleRamparts, LevelId = 4, RoomId = 23},
				new {Gate = R.GateCastleKeep, LevelId = 5, RoomId = 24},
				new {Gate = R.GateRoyalTowers, LevelId = 6, RoomId = 0},
				new {Gate = R.GateMaw, LevelId = 8, RoomId = 49},
				new {Gate = R.GateCavesOfBanishment, LevelId = 8, RoomId = 50},
				//new {Gate = R.GateSealedCaves, LevelId = 9, RoomId = 49}, // dont want to spawn infront of xarion
				new {Gate = R.GateSealedCaves, LevelId = 9, RoomId = 50},
			};

			var selectedGate = gateProgressionItems.SelectRandom(random);

			unlockingSpecification.OnPickup = level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);
			unlockingSpecification.Unlocks = selectedGate.Gate;
		}

		public R GetUnlock(ItemIdentifier identifier) =>
			UnlockingSpecifications.FirstOrDefault(us => us.Item == identifier)?.Unlocks ?? R.None;

		public R GetAllUnlock(ItemIdentifier identifier) =>
			UnlockingSpecifications.FirstOrDefault(us => us.Item == identifier)?.AllUnlocks ?? R.None;

		public Action<Level> GetPickupAction(ItemIdentifier identifier) =>
			UnlockingSpecifications.FirstOrDefault(us => us.Item == identifier)?.OnPickup;

		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}

		public class UnlockingSpecification
		{
			public ItemIdentifier Item { get; }
			public R Unlocks { get; internal set; }
			public R AdditionalUnlocks { get; }
			public R AllUnlocks => Unlocks | AdditionalUnlocks;
			public Action<Level> OnPickup { get; internal set; }

			public UnlockingSpecification(ItemIdentifier item, R unlocks) : this(item, unlocks, R.None)
			{
			}

			public UnlockingSpecification(ItemIdentifier item, R unlocks, R additionalUnlocks)
			{
				Item = item;
				Unlocks = unlocks;
				AdditionalUnlocks = additionalUnlocks;
			}
		}
	}
}
