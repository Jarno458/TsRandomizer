using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation.ItemPlacers
{
	class ForwardFillingItemLocationRandomizer : ItemLocationRandomizer
	{
		readonly ItemUnlockingMap unlockingMap;
		static readonly ItemInfo UpwardsDash = ItemInfo.Get(EInventoryRelicType.EssenceOfSpace);
		static readonly ItemInfo LightWall = ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell);

		readonly Random random;

		readonly Requirement unlockableRequirements; 
		Requirement availableRequirements;

		List<ItemLocation> availableItemLocations;

		readonly Dictionary<ItemInfo, ItemLocation> placedItems;
		readonly Dictionary<ItemInfo, Gate> paths;

		ForwardFillingItemLocationRandomizer(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap) : base(itemLocationMap, unlockingMap)
		{
			this.unlockingMap = unlockingMap;

			random = new Random(seed);
			availableRequirements = Requirement.None;
			unlockableRequirements = unlockingMap.AllUnlockableRequirements;
			placedItems = new Dictionary<ItemInfo, ItemLocation>();
			paths = new Dictionary<ItemInfo, Gate>();
		}

		public static void AddRandomItemsToLocationMap(Seed seed, ItemUnlockingMap unlockingMap, ItemLocationMap itemLocationMap)
		{
			new ForwardFillingItemLocationRandomizer(seed, unlockingMap, itemLocationMap)
				.AddRandomItemsToLocationMap();
		}

		void AddRandomItemsToLocationMap()
		{
			RecalculateAvailableItemLocations();
			CalculateTutorial();

			var itemsThatUnlockProgression = unlockingMap.Map.Keys.ToList();

			while (itemsThatUnlockProgression.Count > 0)
			{
				var item = itemsThatUnlockProgression.SelectRandom(random);

				if (placedItems.ContainsKey(item))
					return;

				CalculatePathChain(item, Requirement.None);
				itemsThatUnlockProgression.Remove(item);
			}

			foreach (var path in paths)
				Console.Out.WriteLine($"Requirements For {path.Key}@{placedItems[path.Key]} -> {path.Value}");

			FillRemainingChests(random);
		}

		void CalculatePathChain(ItemInfo item, Requirement additionalRequirementsToAvoid)
		{
			if (placedItems.ContainsKey(item))
				return;

			var unlockingRequirements = additionalRequirementsToAvoid | unlockingMap.GetUnlock(item);
			var itemLocation = GetUnusedItemLocationThatDontRequire(unlockingRequirements);
			
			CalculatePathChain(itemLocation.Gate, unlockingRequirements);

			PutItemAtLocation(item, itemLocation);

			paths[item] = itemLocation.Gate;
		}

		void CalculatePathChain(Gate gate, Requirement requirementChain)
		{
			switch (gate)
			{
				case Gate.RequirementGate requirementGate:
					CalculatePathChainForRequirementGate(requirementGate, requirementChain);
					break;
				case Gate.OrGate orGate:
					CalculatePathChainForOrGate(orGate, requirementChain);
					break;
				case Gate.AndGate andGate:
					CalculatePathChainForAndGate(andGate, requirementChain);
					break;
				default:
					throw new ArgumentOutOfRangeException($"Unknown gate type {gate}");
			}
		}

		void CalculatePathChainForRequirementGate(Gate.RequirementGate requirementGate, Requirement requirementChain)
		{
			if (requirementGate.Requirements == Requirement.None)
				return;

			var singleRequirement = SelectSingleUnlockableRequirement(requirementGate.Requirements, requirementChain);
			var item = GetRandomItemThatUnlocksRequirement(singleRequirement);

			if (placedItems.ContainsKey(item))
				return;

			CalculatePathChain(item, singleRequirement | requirementChain);
		}

		void CalculatePathChainForOrGate(Gate.OrGate orGate, Requirement requirementToAvoid)
		{
			var unlockableGates = orGate.Gates
				.Where(g => GateCanBeOpenedWithoutSuppliedRequirements(g, requirementToAvoid))
				.ToArray();

			var unlockableGate = unlockableGates[random.Next(unlockableGates.Length)]; //SelectRandom

			CalculatePathChain(unlockableGate, requirementToAvoid);
		}

		void CalculatePathChainForAndGate(Gate.AndGate andGate, Requirement requirementChain)
		{
/*			return andGate.Gates
				//.InRandomOrder(random) //Aggregate always goes in order of definition it doesnt go in random order
				.Aggregate(requirementChain, (chain, singleGate) => chain | CalculatePathChain(singleGate, chain));*/


			foreach (var gate in andGate.Gates)
			{
				CalculatePathChain(gate, requirementChain);
			}
		}

		Requirement SelectSingleUnlockableRequirement(Requirement requirement, Requirement requirementToAvoid)
		{
			var requirements = ((Requirement)((ulong)requirement & (ulong)unlockableRequirements & (~requirementToAvoid | (ulong)availableRequirements)))
				.Split();
			//return requirements.SelectRandom(random);
			return requirements[random.Next(requirements.Length)];
		}

		ItemInfo GetRandomItemThatUnlocksRequirement(Requirement requirement)
		{
			var unlockingItems = unlockingMap.Map
				.Where(x => x.Value.AllUnlocks.Contains(requirement))
				.Select(x => x.Key);

			if (requirement != Requirement.UpwardDash && placedItems.Count <= 10)
				// ReSharper disable PossibleUnintendedReferenceComparison
				unlockingItems = unlockingItems.Where(i => i != UpwardsDash && i != LightWall);
				// ReSharper restore PossibleUnintendedReferenceComparison

			var unlockingItemsArray = unlockingItems.ToArray();

			//return unlockingItems.SelectRandom(random);
			return unlockingItemsArray[random.Next(unlockingItemsArray.Length)];
		}

		ItemLocation GetUnusedItemLocationThatDontRequire(Requirement requirements)
		{
			var locations = ItemLocations
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
			var canBeOpened = (gateRequirements & (~requirementToAvoid | (ulong)availableRequirements)) > 0;

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
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), ItemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var meleeOrbType = orbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), ItemLocations[ItemKey.TutorialMeleeOrb]);

			RecalculateAvailableItemLocations();
		}

		void RecalculateAvailableItemLocations()
		{
			availableItemLocations = ItemLocations
				.Where(l => !l.IsUsed && l.Gate.CanBeOpenedWith(availableRequirements))
				.ToList();
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			var itemUnlocks = unlockingMap.GetAllUnlock(itemInfo);

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
			return ((ulong)availableRequirements | (ulong)itemUnlocks) != (ulong)availableRequirements;
		}
	}
}
