using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation
{
	class ItemLocationRandomizer
	{
		static readonly ItemInfo UpwardsDash = ItemInfo.Get(EInventoryRelicType.EssenceOfSpace);
		static readonly ItemInfo LightWall = ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell);

		ItemLocationMap itemLocations;
		readonly Random random;

		Requirement unlockableRequirements; 
		Requirement availableRequirements;

		List<ItemLocation> availableItemLocations;

		readonly Dictionary<ItemInfo, ItemLocation> placedItems;
		readonly Dictionary<ItemInfo, Requirement> paths;

		public ItemLocationRandomizer(Seed seed)
		{
			random = new Random(seed);
			availableRequirements = Requirement.None;
			unlockableRequirements = ulong.MaxValue;
			placedItems = new Dictionary<ItemInfo, ItemLocation>();
			paths = new Dictionary<ItemInfo, Requirement>();
		}

		public void AddRandomItemsToLocationMap(ItemLocationMap itemLocationMap)
		{
			itemLocations = itemLocationMap;
			RecalculateAvailableItemLocations();

			CalculateTeleporterPickAction();
			CalculateTutorial();

			foreach (var item in ItemInfo.UnlockingMap.Keys)
				CalculatePathChain(item, Requirement.None);

			foreach (var path in paths)
				Console.Out.WriteLine($"Requirements For {path.Key}@{placedItems[path.Key]} -> {path.Value}");
			
			FillRemainingChests();
		}

		void CalculatePathChain(ItemInfo item, Requirement additionalRequirementsToAvoid)
		{
			if (placedItems.ContainsKey(item))
				return;

			var unlockingRequirements = additionalRequirementsToAvoid | item.Unlocks;
			var itemLocation = GetUnusedItemLocationThatDontRequire(unlockingRequirements);
			var chain = CalculatePathChain(itemLocation.Gate, unlockingRequirements);

			PutItemAtLocation(item, itemLocation);

			paths[item] = chain;
		}

		Requirement CalculatePathChain(Gate gate, Requirement requirementChain)
		{
			switch (gate)
			{
				case Gate.RequirementGate requirementGate:
					if (requirementGate.Requirements == Requirement.None)
						return requirementChain;

					var singleRequirement = SelectSingleUnlockableRequirement(requirementGate.Requirements, requirementChain);
					var item = GetRandomItemThatUnlocksRequirement(singleRequirement);

					if (placedItems.ContainsKey(item))
						return requirementChain;

					CalculatePathChain(item, requirementChain | singleRequirement);

					return requirementChain | singleRequirement;
				case Gate.OrGate orGate:
					return CalculatePathChain(SelectSingleUnlockableGate(orGate.Gates, requirementChain), requirementChain);
				case Gate.AndGate andGate:
					return andGate.Gates
						.Aggregate(requirementChain, (chain, singleGate) => chain | CalculatePathChain(singleGate, chain));
				default:
					throw new ArgumentOutOfRangeException($"Unknown gate type {gate}");
			}
		}

		Requirement SelectSingleUnlockableRequirement(Requirement requirement, Requirement requirementToAvoid)
		{
			var requirements = ((Requirement) ((ulong) requirement & (ulong) unlockableRequirements & (~requirementToAvoid | (ulong) availableRequirements)))
				.Split();
			return requirements[random.Next(requirements.Length)];
		}

		Gate SelectSingleUnlockableGate(Gate[] gates, Requirement requirementToAvoid)
		{
			var unlockableGates = gates
				.Where(g => GateCanBeOpenedWithoutSuppliedRequirements(g, requirementToAvoid));
			
			return unlockableGates.SelectRandom(random);
		}

		ItemInfo GetRandomItemThatUnlocksRequirement(Requirement requirement)
		{
			var unlockingItems = ItemInfo.UnlockingMap
				.Where(x => x.Value.Contains(requirement))
				.Select(x => x.Key);

			if (requirement != Requirement.UpwardDash && placedItems.Count <= 10)
				// ReSharper disable PossibleUnintendedReferenceComparison
				unlockingItems = unlockingItems.Where(i =>i != UpwardsDash && i != LightWall);
				// ReSharper restore PossibleUnintendedReferenceComparison

			return unlockingItems.SelectRandom(random);
		}

		ItemLocation GetUnusedItemLocationThatDontRequire(Requirement requirements)
		{
			var locations = itemLocations
				.Where(l => !l.IsUsed && GateCanBeOpenedWithoutSuppliedRequirements(l.Gate, requirements));

			return locations.SelectRandom(random);
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate gate, Requirement requirementToAvoid)
		{
			switch (gate)
			{
				case Gate.AndGate andGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(andGate, requirementToAvoid);
				case Gate.OrGate orGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(orGate, requirementToAvoid);
				case Gate.RequirementGate requirementGate:
					return GateCanBeOpenedWithoutSuppliedRequirements(requirementGate, requirementToAvoid);
				default:
					throw new Exception($"Gate type isnt supported for gate {gate}");
			}
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.RequirementGate gate, Requirement requirementToAvoid)
		{
			if (gate.Requirements == Requirement.None)
				return true;

			var gateRequirements = (ulong)gate.Requirements & (ulong)unlockableRequirements;
			var canBeOpened = (gateRequirements & (~requirementToAvoid | (ulong) availableRequirements)) > 0;

			return canBeOpened;
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.AndGate gate, Requirement requirementToAvoid)
		{
			return gate.Gates.All(g => GateCanBeOpenedWithoutSuppliedRequirements(g, requirementToAvoid));
		}

		bool GateCanBeOpenedWithoutSuppliedRequirements(Gate.OrGate orGate, Requirement requirementToAvoid)
		{
			return orGate.Gates.Any(g => GateCanBeOpenedWithoutSuppliedRequirements(g, requirementToAvoid));
		}

		void CalculateTeleporterPickAction()
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

			ItemInfo.Get(EInventoryRelicType.PyramidsKey).SetPickupAction(
				level => UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId)
			);

			unlockableRequirements = ((ulong)unlockableRequirements & ~Requirement.TeleportationGates) | selectedGate.Gate;
			ItemInfo.UnlockingMap[ItemInfo.Get(EInventoryRelicType.PyramidsKey)] = selectedGate.Gate;
		}

		void CalculateTutorial()
		{
			var orbTypes = ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType))).ToList();

			var spellOrbTypes = orbTypes
				.Where(orbType => orbType != EInventoryOrbType.Barrier); //To OP to give as starter item

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), itemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var meleeOrbType = orbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), itemLocations[ItemKey.TutorialMeleeOrb]);
		}

		void FillRemainingChests()
		{
			foreach (var itemLocation in itemLocations)
				if (!itemLocation.IsUsed)
					PutItemAtLocation(ItemInfo.Dummy, itemLocation);
		}

		void RecalculateAvailableItemLocations()
		{
			availableItemLocations = itemLocations
				.Where(l => !l.IsUsed && l.Gate.CanOpen(availableRequirements))
				.ToList();
		}

		void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);

			if (!placedItems.ContainsKey(itemInfo))
				placedItems.Add(itemInfo, itemLocation);

			if(ItemInfo.UnlockingMap.TryGetValue(itemInfo, out Requirement unlocks) && ((ulong)availableRequirements & (ulong)unlocks) != (ulong)unlocks)
			{ 
				availableRequirements |= unlocks;
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
