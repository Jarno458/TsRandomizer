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
		public const int ProgressionItemCount = 22;

		public Dictionary<ItemInfo, UnlockingSpecificaiton> Map;

		public R AllUnlockableRequirements => Map.Values.Aggregate(R.None, (a, b) => a | b.AllUnlocks);
		public IEnumerable<ItemInfo> ItemsThatUnlockProgression => Map.Keys;

		public ItemUnlockingMap(ItemInfoProvider itemProvider, Seed seed)
		{
			var random = new Random(seed);

			Map = new Dictionary<ItemInfo, UnlockingSpecificaiton>(ProgressionItemCount);
			Map.Add(itemProvider.Get(EInventoryRelicType.TimespinnerWheel), new UnlockingSpecificaiton {Unlocks = R.TimespinnerWheel, AdditionalUnlocks = R.TimeStop});
			Map.Add(itemProvider.Get(EInventoryRelicType.DoubleJump), new UnlockingSpecificaiton {Unlocks = R.DoubleJump, AdditionalUnlocks = R.TimeStop});
			Map.Add(itemProvider.Get(EInventoryRelicType.Dash), new UnlockingSpecificaiton {Unlocks = R.ForwardDash});
			Map.Add(itemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Passive), new UnlockingSpecificaiton {Unlocks = R.AntiWeed});
			Map.Add(itemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = R.AntiWeed});
			Map.Add(itemProvider.Get(EInventoryOrbType.Flame, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.AntiWeed});
			Map.Add(itemProvider.Get(EInventoryOrbType.Book, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.AntiWeed});
			Map.Add(itemProvider.Get(EInventoryRelicType.ScienceKeycardA), new UnlockingSpecificaiton {Unlocks = R.CardA, AdditionalUnlocks = R.CardB | R.CardC | R.CardD});
			Map.Add(itemProvider.Get(EInventoryRelicType.ScienceKeycardB), new UnlockingSpecificaiton {Unlocks = R.CardB, AdditionalUnlocks = R.CardC | R.CardD});
			Map.Add(itemProvider.Get(EInventoryRelicType.ScienceKeycardC), new UnlockingSpecificaiton {Unlocks = R.CardC, AdditionalUnlocks = R.CardD});
			Map.Add(itemProvider.Get(EInventoryRelicType.ScienceKeycardD), new UnlockingSpecificaiton {Unlocks = R.CardD});
			Map.Add(itemProvider.Get(EInventoryRelicType.ElevatorKeycard), new UnlockingSpecificaiton {Unlocks = R.CardE});
			Map.Add(itemProvider.Get(EInventoryRelicType.ScienceKeycardV), new UnlockingSpecificaiton {Unlocks = R.CardV});
			Map.Add(itemProvider.Get(EInventoryRelicType.WaterMask), new UnlockingSpecificaiton {Unlocks = R.Swimming});
			Map.Add(itemProvider.Get(EInventoryRelicType.PyramidsKey), new UnlockingSpecificaiton {Unlocks = R.Teleport, AdditionalUnlocks = SelectTeleporterPickupAction(itemProvider, random)});
			Map.Add(itemProvider.Get(EInventoryRelicType.TimespinnerSpindle), new UnlockingSpecificaiton {Unlocks = R.TimespinnerSpindle});
			Map.Add(itemProvider.Get(EInventoryRelicType.TimespinnerGear1), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece1});
			Map.Add(itemProvider.Get(EInventoryRelicType.TimespinnerGear2), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece2});
			Map.Add(itemProvider.Get(EInventoryRelicType.TimespinnerGear3), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece3});
			Map.Add(itemProvider.Get(EInventoryRelicType.EssenceOfSpace), new UnlockingSpecificaiton {Unlocks = R.UpwardDash, AdditionalUnlocks = R.DoubleJump | R.TimeStop});
			Map.Add(itemProvider.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.UpwardDash, AdditionalUnlocks = R.DoubleJump | R.TimeStop});
			Map.Add(itemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = R.PinkOrb});
			Map.Add(itemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.PinkOrb});
			Map.Add(itemProvider.Get(EInventoryOrbType.Pink, EOrbSlot.Passive), new UnlockingSpecificaiton {Unlocks = R.PinkOrb});
			Map.Add(itemProvider.Get(EInventoryRelicType.AirMask), new UnlockingSpecificaiton {Unlocks = R.GassMask});
		}

		R SelectTeleporterPickupAction(ItemInfoProvider itemProvider, Random random)
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

			var pyramidKeys = itemProvider.Get(EInventoryRelicType.PyramidsKey);
			pyramidKeys.SetPickupAction(level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId));

			return selectedGate.Gate;
		}

		public R GetUnlock(ItemInfo itemInfo)
		{
			return Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton) 
				? specificaiton.Unlocks 
				: R.None;
		}

		public R GetAllUnlock(ItemInfo itemInfo)
		{
			return Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton) 
				? specificaiton.AllUnlocks 
				: R.None;
		}

		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}

		public class UnlockingSpecificaiton
		{
			public R Unlocks { get; set; }
			public R AdditionalUnlocks { get; set; }
			public R AllUnlocks => Unlocks | AdditionalUnlocks;
		}
	}
}
