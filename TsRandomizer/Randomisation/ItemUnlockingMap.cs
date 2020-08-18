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

		public ItemUnlockingMap(Seed seed)
		{
			var random = new Random(seed);

			Map = new Dictionary<ItemInfo, UnlockingSpecificaiton>(ProgressionItemCount)
			{
				{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), new UnlockingSpecificaiton {Unlocks = R.TimespinnerWheel, AdditionalUnlocks = R.TimeStop}},
				{ItemInfo.Get(EInventoryRelicType.DoubleJump), new UnlockingSpecificaiton {Unlocks = R.DoubleJump, AdditionalUnlocks = R.TimeStop}},
				{ItemInfo.Get(EInventoryRelicType.Dash), new UnlockingSpecificaiton {Unlocks = R.ForwardDash}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive), new UnlockingSpecificaiton {Unlocks = R.AntiWeed}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = R.AntiWeed}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.AntiWeed}},
				{ItemInfo.Get(EInventoryOrbType.Book, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.AntiWeed}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardA), new UnlockingSpecificaiton {Unlocks = R.CardA, AdditionalUnlocks = R.CardB | R.CardC | R.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardB), new UnlockingSpecificaiton {Unlocks = R.CardB, AdditionalUnlocks = R.CardC | R.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardC), new UnlockingSpecificaiton {Unlocks = R.CardC, AdditionalUnlocks = R.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardD), new UnlockingSpecificaiton {Unlocks = R.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ElevatorKeycard), new UnlockingSpecificaiton {Unlocks = R.CardE}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardV), new UnlockingSpecificaiton {Unlocks = R.CardV}},
				{ItemInfo.Get(EInventoryRelicType.WaterMask), new UnlockingSpecificaiton {Unlocks = R.Swimming}},
				{ItemInfo.Get(EInventoryRelicType.PyramidsKey), new UnlockingSpecificaiton {Unlocks = R.Teleport, AdditionalUnlocks = SelectTeleporterPickupAction(random)}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), new UnlockingSpecificaiton {Unlocks = R.TimespinnerSpindle}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear1), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece1}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear2), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece2}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear3), new UnlockingSpecificaiton {Unlocks = R.TimespinnerPiece3}},
				{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), new UnlockingSpecificaiton {Unlocks = R.UpwardDash, AdditionalUnlocks = R.DoubleJump | R.TimeStop}},
				{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.UpwardDash, AdditionalUnlocks = R.DoubleJump | R.TimeStop}},
				{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = R.PinkOrb}},
				{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = R.PinkOrb}},
				{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Passive), new UnlockingSpecificaiton {Unlocks = R.PinkOrb}},
				{ItemInfo.Get(EInventoryRelicType.AirMask), new UnlockingSpecificaiton {Unlocks = R.GassMask}},
			};
		}

		R SelectTeleporterPickupAction(Random random)
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
			};

			var selectedGate = gateProgressionItems.SelectRandom(random);

			var pyramidKeys = ItemInfo.Get(EInventoryRelicType.PyramidsKey);
			pyramidKeys.SetPickupAction(level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId));

			return selectedGate.Gate;
		}

		public R GetUnlock(ItemInfo itemInfo)
		{
			if (Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton))
				return specificaiton.Unlocks;
			return R.None;
		}

		public R GetAllUnlock(ItemInfo itemInfo)
		{
			if (Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton))
				return specificaiton.AllUnlocks;
			return R.None;
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
