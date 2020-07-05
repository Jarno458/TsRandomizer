using System;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Randomisation.ItemPlacers
{
	abstract class ItemLocationRandomizer
	{
		protected readonly ItemLocationMap ItemLocations;

		protected ItemLocationRandomizer(ItemLocationMap itemLocations)
		{
			ItemLocations = itemLocations;
		}

		protected void CalculateTutorial(Random random)
		{
			var orbTypes = ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType))).ToList();

			var spellOrbTypes = orbTypes
				.Where(orbType => orbType != EInventoryOrbType.Barrier //To OP to give as starter item
				                  && orbType != EInventoryOrbType.None //not an actual orb to use
                                  && orbType != EInventoryOrbType.Monske); //no implemented, yields None orb

			var spellOrbType = spellOrbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(spellOrbType, EOrbSlot.Spell), ItemLocations[ItemKey.TutorialSpellOrb]);

			orbTypes.Remove(EInventoryOrbType.Pink); //To annoying as each attack consumes aura power

			var meleeOrbType = orbTypes.SelectRandom(random);
			PutItemAtLocation(ItemInfo.Get(meleeOrbType, EOrbSlot.Melee), ItemLocations[ItemKey.TutorialMeleeOrb]);
		}

		protected void PlaceGassMaskInALegalSpot(Random random)
		{
			var minimalMawRequirements =
				Requirement.DoubleJump | Requirement.GateAccessToPast | Requirement.Swimming;

			var posableGassMaskLocations = ItemLocations
				.Where(l => l.Key.LevelId != 1 && !l.IsUsed && l.Gate.CanBeOpenedWith(minimalMawRequirements))
				.ToArray();

			PutItemAtLocation(ItemInfo.Get(EInventoryRelicType.AirMask), posableGassMaskLocations.SelectRandom(random));
		}

		protected void FillRemainingChests()
		{
			foreach (var itemLocation in ItemLocations.Where(l => !l.IsUsed))
				PutItemAtLocation(ItemInfo.Dummy, itemLocation);
		}

		protected abstract void PutItemAtLocation(ItemInfo itemInfo, ItemLocation itemLocation);
	}
}
