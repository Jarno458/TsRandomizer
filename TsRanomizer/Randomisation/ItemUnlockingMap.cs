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
		public Dictionary<ItemInfo, Requirement> Map;

		public Requirement AllUnlockableRequirements => Map.Values.Aggregate(Requirement.None, (a, b) => a | b);

		public ItemUnlockingMap(Seed seed)
		{
			Map = new Dictionary<ItemInfo, Requirement>
			{
				{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), Requirement.TimespinnerWheel | Requirement.TimeStop},
				{ItemInfo.Get(EInventoryRelicType.DoubleJump), Requirement.DoubleJump | Requirement.TimeStop},
				{ItemInfo.Get(EInventoryRelicType.Dash), Requirement.ForwardDash},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive), Requirement.AntiWeed},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), Requirement.AntiWeed},
				{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell), Requirement.AntiWeed},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardA), Requirement.CardA | Requirement.CardB | Requirement.CardC | Requirement.CardD},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardB), Requirement.CardB | Requirement.CardC | Requirement.CardD},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardC), Requirement.CardC | Requirement.CardD},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardD), Requirement.CardD},
				{ItemInfo.Get(EInventoryRelicType.ElevatorKeycard), Requirement.CardE},
				{ItemInfo.Get(EInventoryRelicType.ScienceKeycardV), Requirement.CardV},
				{ItemInfo.Get(EInventoryRelicType.WaterMask), Requirement.Swimming},
				{ItemInfo.Get(EInventoryRelicType.PyramidsKey), Requirement.None}, //Set in ForwardFillingItemLocationRandomizer.CalculateTeleporterPickAction(),
				{ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), Requirement.TimespinnerSpindle},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear1), Requirement.TimespinnerPiece1},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear2), Requirement.TimespinnerPiece2},
				{ItemInfo.Get(EInventoryRelicType.TimespinnerGear3), Requirement.TimespinnerPiece3},
				{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop},
				{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop}
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

			Map[pyramidKeys] = selectedGate.Gate;
		}

		public Requirement Get(ItemInfo itemInfo)
		{
			if (Map.TryGetValue(itemInfo, out Requirement requirement))
				return requirement;
			return Requirement.None;
		}
		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}
	}
}
