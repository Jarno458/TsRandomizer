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
		readonly Random random;

		readonly Requirement unlockableRequirements; 
		Requirement availableRequirements;

		ItemLocationMap itemLocations;

		List<ItemLocation> availableItemLocations;

		readonly Dictionary<ItemInfo, ItemLocation> placedItems;
		readonly Dictionary<ItemInfo, Gate> paths;

		public ForwardFillingItemLocationRandomizer(
			Seed seed, ItemInfoProvider itemProvider, ItemUnlockingMap unlockingMap
		) : base(seed, itemProvider, unlockingMap)
		{
			random = new Random((int)seed.Id);
			availableRequirements = Requirement.None;
			unlockableRequirements = unlockingMap.AllUnlockableRequirements;
			placedItems = new Dictionary<ItemInfo, ItemLocation>();
			paths = new Dictionary<ItemInfo, Gate>();
		}

		public override ItemLocationMap GenerateItemLocationMap(bool isProgressionOnly)
		{
			AddRandomItemsToLocationMap(isProgressionOnly);

			itemLocations = new ItemLocationMap(ItemInfoProvider, UnlockingMap, Seed.Options);

			return itemLocations;
		}

		void AddRandomItemsToLocationMap(bool isProgressionOnly)
		{
			RecalculateAvailableItemLocations();
			CalculateTutorial();

			var itemsThatUnlockProgression = UnlockingMap.AllProgressionItems
				.Select(i => new SingleItemInfo(UnlockingMap, i)).ToList();

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

			if(!isProgressionOnly)
				FillRemainingChests(random, itemLocations);
		}

		void CalculatePathChain(ItemInfo item, Requirement additionalRequirementsToAvoid)
		{
			if (placedItems.ContainsKey(item))
				return;

			var unlockingRequirements = additionalRequirementsToAvoid | UnlockingMap.GetUnlock(item.Identifier);
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

			return requirements.SelectRandom(random);
		}

		ItemInfo GetRandomItemThatUnlocksRequirement(Requirement requirement)
		{
			var upwardsDash = new ItemIdentifier(EInventoryRelicType.EssenceOfSpace);
			var lightWall = new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell);

			var unlockingItems = UnlockingMap.AllItemThatUnlockProgression(requirement);

			if (requirement != Requirement.UpwardDash && placedItems.Count <= 10)
				unlockingItems = unlockingItems.Where(i => i != upwardsDash && i != lightWall);

			return new SingleItemInfo(UnlockingMap, unlockingItems.ToArray().SelectRandom(random));
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
			PutItemAtLocation(ItemInfoProvider.Get(spellOrbType, EOrbSlot.Spell), itemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var meleeOrbType = orbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfoProvider.Get(meleeOrbType, EOrbSlot.Melee), itemLocations[ItemKey.TutorialMeleeOrb]);

			RecalculateAvailableItemLocations();
		}

		void RecalculateAvailableItemLocations()
		{
			availableItemLocations = itemLocations
				.Where(l => !l.IsUsed && l.Gate.CanBeOpenedWith(availableRequirements))
				.ToList();
		}

		protected override void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation)
		{
			itemLocation.SetItem(itemInfo);

			if (!placedItems.ContainsKey(itemInfo))
				placedItems.Add(itemInfo, itemLocation);

			if(NewRequirementIsUnlocked(itemInfo.Unlocks))
			{ 
				availableRequirements |= itemInfo.Unlocks;
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
