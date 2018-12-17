using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationRandomizer
	{
		ItemLocationMap itemLocations;
		readonly Random random;

		Requirement unlockableRequirements; 
		Requirement availableRequirements;

		List<ItemLocation> availableItemLocations;
		Dictionary<ItemInfo, ItemLocation> placedItems;

		readonly Dictionary<ItemInfo, Requirement> unlocks = new Dictionary<ItemInfo, Requirement>
		{
			{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), Requirement.TimespinnerWheel | Requirement.TimeStop},
			{ItemInfo.Get(EInventoryRelicType.DoubleJump), Requirement.DoubleJump | Requirement.TimeStop},
			{ItemInfo.Get(EInventoryRelicType.Dash), Requirement.ForwardDash},
			{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop}, 
			{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop},
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
			{ItemInfo.Get(EInventoryRelicType.PyramidsKey), Requirement.None}, //Set in CalculateTeleporterPickAction(),
			{ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), Requirement.TimespinnerSpindle},
		};

		public ItemLocationRandomizer(Seed seed)
		{
			random = new Random(seed);
			availableRequirements = Requirement.None;
			placedItems = new Dictionary<ItemInfo, ItemLocation>();
		}

		public void AddRandomItemsToLocationMap(ItemLocationMap itemLocationMap)
		{
			itemLocations = itemLocationMap;
			RecalculateAvailableItemLocations();

			CalculateTeleporterPickAction();
			CalculateTutorial();

			CalculatePathChain(ItemInfo.Get(EInventoryRelicType.DoubleJump), Requirement.DoubleJump | Requirement.UpwardDash);

			/*CalculateLakeDesolationPath();
			
			if (!CanTeleport)
			{
				CalculateLibraryPath();
			}
			else
			{
				if (CanAccessPast)
				{

				}
			}*/


			//if (availableRequirements.Contains(lakeDelosationBridge))
			//{
			//can cross bridge else we need to find a different path
			//}
			
			FillRemainingChests();
		}

		void CalculatePathChain(ItemInfo item)
		{
			CalculatePathChain(item, Requirement.None);
		}

		//Calculating Chain For DoubleJump@[9.5.88.496] -> TS|DoubleJump@[9.5.88.496] -> cD|ScienceKeycardA@[4.20.264.160] -> cB|ScienceKeycardA@[4.20.264.160] -> Sw|WaterMask@[4.11.344.192]

		void CalculatePathChain(ItemInfo item, Requirement additionalRequirementsToAvoid)
		{
			var unlockingRequirements = unlocks.ContainsKey(item)
				? additionalRequirementsToAvoid | unlocks[item]
				: additionalRequirementsToAvoid;

			var itemLocation = GetUnusedItemLocationThatDontRequire(unlockingRequirements);

			Console.Out.Write($"Calculating Chain For {item}@{itemLocation}");

			PutItemAtLocation(item, itemLocation);

			CalculatePathChain(itemLocation.Gate, unlockingRequirements);

			Console.Out.WriteLine();
		}

		Requirement CalculatePathChain(Gate gate, Requirement requirementChain)
		{
			switch (gate)
			{
				case Gate.RequirementGate requirementGate:
					if (requirementGate.Requirements == Requirement.None)
						return requirementChain;

					var singleRequirement = SelectSingleUnlockableRequirement(requirementGate.Requirements);
					var item = GetRandomItemThatUnlocksRequirement(singleRequirement);

					if (placedItems.ContainsKey(item))
					{
						Console.Out.Write($" -> {singleRequirement}|{item}@{placedItems[item]}");
						return requirementChain;
					}

					var itemLocation = GetUnusedItemLocationThatDontRequire(requirementChain | singleRequirement);

					PutItemAtLocation(item, itemLocation);

					Console.Out.Write($" -> {singleRequirement}|{item}@{itemLocation}");

					return requirementChain | singleRequirement;
				case Gate.OrGate orGate:
					return CalculatePathChain(SelectSingleUnlockableGate(orGate.Gates), requirementChain);
				case Gate.AndGate andGate:
					return andGate.Gates
						.Aggregate(requirementChain, (chain, singleGate) => chain | CalculatePathChain(singleGate, chain));
				default:
					throw new ArgumentOutOfRangeException($"Unknown gate type {gate}");
			}
		}

		Requirement SelectSingleUnlockableRequirement(Requirement requirement)
		{
			var requirements = ((Requirement)((ulong)requirement & (ulong)unlockableRequirements))
				.Split()
				.ToArray();
			return requirements[random.Next(requirements.Length)];
		}

		Gate SelectSingleUnlockableGate(Gate[] gates)
		{
			var unlockableGates = gates
				.Where(g => !(g is Gate.RequirementGate r)
							|| ((ulong)r.Requirements & (ulong)unlockableRequirements) > 0)
				.ToArray();
			
			return unlockableGates[random.Next(unlockableGates.Length)];
		}

		ItemInfo GetRandomItemThatUnlocksRequirement(Requirement requirement)
		{
			var unlockingItems = unlocks
				.Where(x => x.Value.Contains(requirement))
				.Select(x => x.Key)
				.ToArray();

			return unlockingItems[random.Next(unlockingItems.Length)];
		}

		ItemLocation GetUnusedItemLocationThatDontRequire(Requirement requirements)
		{
			var locations = itemLocations
				.Where(l => !l.IsUsed && GateCanBeOpenedWithoutSuppliedRequirements(l.Gate, requirements))
				.ToArray();

			var x = requirements.Contains(Requirement.TimeStop);

			var loc = locations[random.Next(locations.Length)];
			return loc;
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate gate, Requirement requirementsToCheck)
		{
			switch (gate)
			{
				case Gate.AndGate andGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(andGate, requirementsToCheck);
				case Gate.OrGate orGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(orGate, requirementsToCheck);
				case Gate.RequirementGate requirementGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(requirementGate, requirementsToCheck);
				default:
					throw new NotImplementedException($"Gate type isnt supported for gate {gate}");
			}
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.RequirementGate gate, Requirement requirementsToCheck)
		{
			var gateRequirements = (ulong)gate.Requirements & (ulong)unlockableRequirements;
			return gate.Requirements == Requirement.None || (gateRequirements & ~requirementsToCheck) > 0;
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.AndGate gate, Requirement requirementsToCheck)
		{
			return gate.Gates.All(g => GateCanBeOpenedWithoutSuppliedRequirements(g, requirementsToCheck));
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.OrGate orGate, Requirement requirementsToCheck)
		{
			return orGate.Gates.Any(g => g.Requires(requirementsToCheck));
		}

		void CalculateTeleporterPickAction()
		{
			var gateProgressionItems = new[] {
				new {Gate = Requirement.GateKittyBoss, LevelId = 2, RoomId = 55},
				new {Gate = Requirement.GateLeftLibrary, LevelId = 2, RoomId = 54},
				new {Gate = Requirement.GateLakeSirine, LevelId = 7, RoomId = 30},
				new {Gate = Requirement.GateLakeSirine, LevelId = 7, RoomId = 31},
				//new {Gate = Requirement.GateAccessToPast, LevelId = 3, RoomId = 6}, //Refugee Camp, Somehow doesnt work ¯\_(ツ)_/¯
				new {Gate = Requirement.GateAccessToPast, LevelId = 8, RoomId = 51},
			};

			var selectedGate = gateProgressionItems[random.Next(gateProgressionItems.Length)];

			ItemInfo.Get(EInventoryRelicType.PyramidsKey).SetPickupAction(
				level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId)
			);

			unlockableRequirements = ~((ulong)Requirement.TeleportationGates & ~selectedGate.Gate);
			unlocks[ItemInfo.Get(EInventoryRelicType.PyramidsKey)] = selectedGate.Gate;
		}

		void CalculateTutorial()
		{
			var orbTypes = ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType))).ToList();
			orbTypes.Remove(EInventoryOrbType.None);

			var spellOrbTypes = orbTypes
				.Where(orbType => orbType != EInventoryOrbType.Barrier) //To OP to give as starter item
				.ToArray();
			var spellOrbType = spellOrbTypes[random.Next(spellOrbTypes.Length)];
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), itemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Empire); //To annoying as each attack consumes aura power

			var meleeOrbType = orbTypes[random.Next(orbTypes.Count)];
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), itemLocations[ItemKey.TutorialMeleeOrb]);
		}

		void CalculateLakeDesolationPath()
		{
			var pyramidsKey = ItemInfo.Get(EInventoryRelicType.PyramidsKey);
			var forwardDash = ItemInfo.Get(EInventoryRelicType.Dash);
			var doubleJump = ItemInfo.Get(EInventoryRelicType.DoubleJump);
			var timestop = ItemInfo.Get(EInventoryRelicType.TimespinnerWheel);

			var items = new[]
			{
				pyramidsKey, pyramidsKey, pyramidsKey,
				//forwardDash, forwardDash,
				//doubleJump, doubleJump,
				//timestop
			};

			PutRandomItemInReachableChest(items);
		}

		void CalculateLibraryPath()
		{
			PutRandomItemInReachableChest(Requirement.CardD);
			PutRandomItemInReachableChest(Requirement.CardB, Requirement.CardC);

			//If boss is required we need the elevator key, otherwise we need it with CardB
			PutRandomItemInReachableChest(Requirement.CardE);
		}

		void FillRemainingChests()
		{
			foreach (var itemLocation in itemLocations)
				if (!itemLocation.IsUsed)
					PutItemAtLocation(ItemInfo.Dummy, itemLocation);
		}

		ItemInfo PutRandomItemInReachableChest(params ItemInfo[] items)
		{
			var item = SelectRandom(items);
			PutItemInReachableChest(item);
			return item;
		}

		void PutRandomItemInReachableChest(params Requirement[] requirements)
		{
			var requirement = SelectRandom(requirements);
			var itemsThatMeetRequirement = unlocks
				.Where(kvp => requirement.Contains(kvp.Value))
				.Select(kvp => kvp.Key)
				.ToArray();
			var item = itemsThatMeetRequirement[random.Next(itemsThatMeetRequirement.Length)];
			PutItemInReachableChest(item);
		}

		void PutItemInReachableChest(ItemInfo item)
		{
			var itemLocation = availableItemLocations[random.Next(availableItemLocations.Count)];
			PutItemAtLocation(item, itemLocation);
		}

		T SelectRandom<T>(params T[] array)
		{
			return array[random.Next(array.Length)];
		}

		void RecalculateAvailableItemLocations()
		{
			availableItemLocations = itemLocations
				.Where(c => !c.IsUsed && c.Gate.CanOpen(availableRequirements))
				.ToList();
		}

		void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);
			if (!placedItems.ContainsKey(itemInfo))
				placedItems.Add(itemInfo, itemLocation);

			if (unlocks.ContainsKey(itemInfo) && !availableRequirements.Contains(unlocks[itemInfo]))
			{
				availableRequirements |= unlocks[itemInfo];
				RecalculateAvailableItemLocations();
			}
			else
			{
				availableItemLocations.Remove(itemLocation);
			}
		}

		static void UnlockRoom(Level level, int levelId, int roomId)
		{
			var minimapRoom = level.Minimap.Areas[levelId].Rooms[roomId];

			minimapRoom.SetKnown(true);
			minimapRoom.SetVisited(true);
		}
	}
}
