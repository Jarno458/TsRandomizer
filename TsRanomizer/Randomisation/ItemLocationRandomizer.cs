using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	static class ItemLocationRandomizer
	{
		static ItemLocationMap itemLocations;
		static Requirement availableRequirements;
		static Random random;

		static readonly Requirement DoubleJump = Requirement.DoubleJump | Requirement.TimeStop;
		static readonly Requirement UpwardDash = Requirement.UpwardDash | DoubleJump;
		
		static readonly Dictionary<ItemInfo, Requirement> Unlocks = new Dictionary<ItemInfo, Requirement>
		{
			{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), Requirement.TimeStop},
			{ItemInfo.Get(EInventoryRelicType.DoubleJump), DoubleJump},
			{ItemInfo.Get(EInventoryRelicType.Dash), Requirement.ForwardDash},
			{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), UpwardDash}, 
			{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), UpwardDash},
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
		};

		public static void AddRandomItemsToLocationMap(ItemLocationMap itemLocationMap, Seed seed)
		{
			random = new Random(seed);
			itemLocations = itemLocationMap;
			availableRequirements = Requirement.None;

			CalculateTeleporterPickAction();
			CalculateTutorial();

			if (!availableRequirements.Contains(Requirement.AntiWeed))
			{
				CalculateLakeDesolationPath();

				var LakeDelosationBridge = Requirement.DoubleJump | Requirement.ForwardDash | Requirement.TimeStop;
				if (availableRequirements.Contains(LakeDelosationBridge))
				{

				}
			
			}
				
			CalculateLibraryPath();
			FillRemainingChests();
		}

		static void CalculateTeleporterPickAction()
		{
			var gateProgressionItems = new[] {
				new {Gate = Requirement.GateKittyBoss, LevelId = 2, RoomId = 55},
				new {Gate = Requirement.GateLeftLibrary, LevelId = 2, RoomId = 54},
				new {Gate = Requirement.GateLakeSirine, LevelId = 7, RoomId = 30},
				new {Gate = Requirement.GateLakeSirine, LevelId = 7, RoomId = 31},
				new {Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6},
				new {Gate = Requirement.GateAccessToPast, LevelId = 8, RoomId = 51},
			};

			var selectedGate = gateProgressionItems[random.Next(gateProgressionItems.Length)];

			ItemInfo.Get(EInventoryRelicType.PyramidsKey).SetPickupAction(
				level => TeleportPickupAction(level, selectedGate.LevelId, selectedGate.RoomId)
			);

			Unlocks[ItemInfo.Get(EInventoryRelicType.PyramidsKey)] = selectedGate.Gate;
		}

		static void CalculateTutorial()
		{
			var orbTypes = ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType))).ToList();
			orbTypes.Remove(EInventoryOrbType.None);

			var spellOrbTypes = orbTypes
				.Where(orbType => orbType != EInventoryOrbType.Barrier)
				.ToArray();
			var spellOrbType = spellOrbTypes[random.Next(spellOrbTypes.Length)];
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), itemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Empire);

			var meleeOrbType = orbTypes[random.Next(orbTypes.Count)];
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), itemLocations[ItemKey.TutorialMeleeOrb]);
		}

		static void CalculateLakeDesolationPath()
		{
			var progressionItems = new[]
			{
				Requirement.Teleport, Requirement.Teleport, Requirement.Teleport,
				//Requirement.ForwardDash, Requirement.ForwardDash,
				//Requirement.DoubleJump,  Requirement.DoubleJump,
				//Requirement.TimeStop,
			};

			PutRandomItemInReachableChest(progressionItems);
		}

		static void CalculateLibraryPath()
		{
			PutItemMatchitngRequirementInReachableChest(Requirement.CardD);
			PutRandomItemInReachableChest(Requirement.CardB, Requirement.CardC);

			//If boss is required we need the elevator key, otherwise we need it with CardB
			PutItemMatchitngRequirementInReachableChest(Requirement.CardE);
		}

		static void FillRemainingChests()
		{
			foreach (var itemLocation in itemLocations)
				if (!itemLocation.IsUsed)
					PutItemAtLocation(ItemInfo.Dummy, itemLocation);
		}

		static void PutRandomItemInReachableChest(params Requirement[] items)
		{
			var progressionItem = SelectRandomRequirement(items);
			PutItemMatchitngRequirementInReachableChest(progressionItem);
		}

		static void PutItemMatchitngRequirementInReachableChest(Requirement requirement)
		{
			var itemsThatMeetRequirement = Unlocks
				.Where(kvp => requirement.Contains(kvp.Value))
				.Select(kvp => kvp.Key)
				.ToArray();
			var item = itemsThatMeetRequirement[random.Next(itemsThatMeetRequirement.Length)];

			var reachableItemLocations = GetUnusedReachableItemLocations();
			var itemLocation = reachableItemLocations[random.Next(reachableItemLocations.Length)];

			PutItemAtLocation(item, itemLocation);
		}

		static Requirement SelectRandomRequirement(params Requirement[] items)
		{
			return items[random.Next(items.Length)];
		}

		static ItemLocation[] GetUnusedReachableItemLocations()
		{
			return itemLocations
				.Where(c => !c.IsUsed && c.Gate.CanOpen(availableRequirements))
				.ToArray();
		}

		static void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);

			if (Unlocks.ContainsKey(itemInfo))
				availableRequirements |= Unlocks[itemInfo];
		}

		static void TeleportPickupAction(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}
	}
}
