using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemUnlockingMap
	{
		public Dictionary<ItemInfo, UnlockingSpecificaiton> Map;

		public Requirement AllUnlockableRequirements => Map.Values.Aggregate(Requirement.None, (a, b) => a | b.AllUnlocks);

		public ItemUnlockingMap(Seed seed)
		{
			Map = new Dictionary<ItemInfo, UnlockingSpecificaiton>
			{
				{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), new UnlockingSpecificaiton {Unlocks = Requirement.TimeStop}},
				{ItemInfo.Get(EInventoryRelicType.DoubleJump), new UnlockingSpecificaiton {Unlocks = Requirement.DoubleJump, AdditionalUnlocks = Requirement.TimeStop}},
				{ItemInfo.Get(EInventoryRelicType.Dash), new UnlockingSpecificaiton {Unlocks = Requirement.ForwardDash}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive), new UnlockingSpecificaiton {Unlocks = Requirement.AntiWeed}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = Requirement.AntiWeed}},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = Requirement.AntiWeed}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardA), new UnlockingSpecificaiton {Unlocks = Requirement.CardA, AdditionalUnlocks = Requirement.CardB | Requirement.CardC | Requirement.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardB), new UnlockingSpecificaiton {Unlocks = Requirement.CardB, AdditionalUnlocks = Requirement.CardC | Requirement.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardC), new UnlockingSpecificaiton {Unlocks = Requirement.CardC, AdditionalUnlocks = Requirement.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardD), new UnlockingSpecificaiton {Unlocks = Requirement.CardD}},
				{ItemInfo.Get(EInventoryRelicType.ElevatorKeycard), new UnlockingSpecificaiton {Unlocks = Requirement.CardE}},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardV), new UnlockingSpecificaiton {Unlocks = Requirement.CardV}},
				{ItemInfo.Get(EInventoryRelicType.WaterMask), new UnlockingSpecificaiton {Unlocks = Requirement.Swimming}},
				{ItemInfo.Get(EInventoryRelicType.PyramidsKey), new UnlockingSpecificaiton {Unlocks = Requirement.None}}, //Set in ForwardFillingItemLocationRandomizer.CalculateTeleporterPickAction(),
				{ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), new UnlockingSpecificaiton {Unlocks = Requirement.TimespinnerSpindle}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear1), new UnlockingSpecificaiton {Unlocks = Requirement.TimespinnerPiece1}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear2), new UnlockingSpecificaiton {Unlocks = Requirement.TimespinnerPiece2}},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear3), new UnlockingSpecificaiton {Unlocks = Requirement.TimespinnerPiece3}},
				{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), new UnlockingSpecificaiton {Unlocks = Requirement.UpwardDash, AdditionalUnlocks = Requirement.DoubleJump | Requirement.TimeStop}},
				{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), new UnlockingSpecificaiton {Unlocks = Requirement.UpwardDash, AdditionalUnlocks = Requirement.DoubleJump | Requirement.TimeStop}},
				{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), new UnlockingSpecificaiton {Unlocks = Requirement.PinkOrb}},
			};

			var random = new Random(seed);

			CalculateTeleporterPickAction(random);
		}

		void CalculateTeleporterPickAction(Random random)
		{
			var gateProgressionItems = new[] {
				new {Gate = Requirement.GateKittyBoss, LevelId = 2, RoomId = 55},
				new {Gate = Requirement.GateLeftLibrary, LevelId = 2, RoomId = 54},
				new {Gate = Requirement.GateLakeSirineLeft, LevelId = 7, RoomId = 30},
				new {Gate = Requirement.GateLakeSirineRight, LevelId = 7, RoomId = 31},
				//new {Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6}, //Refugee Camp, Somehow doesnt work ¯\_(ツ)_/¯
				new {Gate = Requirement.GateAccessToPast, LevelId = 8, RoomId = 51},
			};

			var selectedGate = gateProgressionItems[random.Next(gateProgressionItems.Length)];

			var pyramidKeys = ItemInfo.Get(EInventoryRelicType.PyramidsKey);

			pyramidKeys.SetPickupAction(level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId));

			Map[pyramidKeys].Unlocks = selectedGate.Gate;
		}

		public Requirement GetUnlock(ItemInfo itemInfo)
		{
			if (Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton))
				return specificaiton.Unlocks;
			return Requirement.None;
		}

		public Requirement GetAllUnlock(ItemInfo itemInfo)
		{
			if (Map.TryGetValue(itemInfo, out UnlockingSpecificaiton specificaiton))
				return specificaiton.AllUnlocks;
			return Requirement.None;
		}

		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}

		public class UnlockingSpecificaiton
		{
			public Requirement Unlocks { get; set; }
			public Requirement AdditionalUnlocks { get; set; }
			public Requirement AllUnlocks => Unlocks | AdditionalUnlocks;
		}
	}
}
