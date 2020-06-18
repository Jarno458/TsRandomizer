using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation.ItemPlacers
{
	class ForwardFillingItemLocationRandomizer
	{
		readonly ItemUnlockingMap unlockingMap;
		static readonly ItemInfo UpwardsDash = ItemInfo.Get(EInventoryRelicType.EssenceOfSpace);
		static readonly ItemInfo LightWall = ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell);

		readonly ItemLocationMap itemLocations;
		readonly Random random;

		readonly Requirement unlockableRequirements; 
		Requirement availableRequirements;

		List<ItemLocation> availableItemLocations;

		readonly Dictionary<ItemInfo, ItemLocation> placedItems;
		readonly Dictionary<ItemInfo, Requirement> paths;

		ForwardFillingItemLocationRandomizer(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap)
		{
			this.unlockingMap = unlockingMap;
			itemLocations = itemLocationMap;

			random = new Random(seed);
			availableRequirements = Requirement.None;
			unlockableRequirements = unlockingMap.AllUnlockableRequirements;
			placedItems = new Dictionary<ItemInfo, ItemLocation>();
			paths = new Dictionary<ItemInfo, Requirement>();
		}

		public static void AddRandomItemsToLocationMap(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap)
		{
			var instance = new ForwardFillingItemLocationRandomizer(seed, unlockingMap, itemLocationMap);

			instance.RecalculateAvailableItemLocations();
			instance.CalculateTutorial();


			foreach (var item in unlockingMap.Map.Keys)
				instance.CalculatePathChain(item, Requirement.None);

			foreach (var path in instance.paths)
				Console.Out.WriteLine($"Requirements For {path.Key}@{instance.placedItems[path.Key]} -> {path.Value}");

			instance.FillRemainingChests();
		}

		void CalculatePathChain(ItemInfo item, Requirement additionalRequirementsToAvoid)
		{
			if (placedItems.ContainsKey(item))
				return;

			var unlockingRequirements = additionalRequirementsToAvoid | unlockingMap.Get(item);
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
			var unlockingItems = unlockingMap.Map
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

			RecalculateAvailableItemLocations();
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
				.Where(l => !l.IsUsed && l.Gate.CanBeOpenedWith(availableRequirements))
				.ToList();
		}

		void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			var itemUnlocks = unlockingMap.Get(itemInfo);

			itemLocation.SetItem(itemInfo, itemUnlocks);

			if (!placedItems.ContainsKey(itemInfo))
				placedItems.Add(itemInfo, itemLocation);

			if(NewRequirementIsUnlocked(itemUnlocks))
			{ 
				availableRequirements |= itemUnlocks;
				RecalculateAvailableItemLocations();
			}
			else
			{
				availableItemLocations.Remove(itemLocation);
			}
		}

		bool NewRequirementIsUnlocked(Requirement itemUnlocks)
		{
			return ((ulong)availableRequirements & (ulong)itemUnlocks) != (ulong)itemUnlocks;
		}
	}
}
