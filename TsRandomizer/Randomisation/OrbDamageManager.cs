using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Settings.GameSettingObjects;

namespace TsRandomizer.Randomisation
{
	struct OrbDamageRange
	{
		public int MinValue;
		public int MidValue;
		public int MaxValue;
	}

	static class OrbDamageManager
	{
		public static Dictionary<int, int> OrbDamageLookup = new Dictionary<int, int>();

		private static OrbDamageRange GetOrbDamageOptions(EInventoryOrbType orbType)
		{
			switch (orbType)
			{
				case EInventoryOrbType.Blue: return new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 8 };
				case EInventoryOrbType.Blade: return new OrbDamageRange { MinValue = 1, MidValue = 7, MaxValue = 12 };
				case EInventoryOrbType.Flame: return new OrbDamageRange { MinValue = 2, MidValue = 6, MaxValue = 16 };
				case EInventoryOrbType.Pink: return new OrbDamageRange { MinValue = 2, MidValue = 6, MaxValue = 30 };
				case EInventoryOrbType.Iron: return new OrbDamageRange { MinValue = 2, MidValue = 10, MaxValue = 20 };
				case EInventoryOrbType.Ice: return new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 12 };
				case EInventoryOrbType.Wind: return new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 };
				case EInventoryOrbType.Gun: return new OrbDamageRange { MinValue = 3, MidValue = 9, MaxValue = 30 };
				case EInventoryOrbType.Umbra: return new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 10 };
				case EInventoryOrbType.Empire: return new OrbDamageRange { MinValue = 2, MidValue = 10, MaxValue = 20 };
				case EInventoryOrbType.Eye: return new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 };
				case EInventoryOrbType.Blood: return new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 };
				case EInventoryOrbType.Book: return new OrbDamageRange { MinValue = 1, MidValue = 6, MaxValue = 12 };
				case EInventoryOrbType.Moon: return new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 };
				case EInventoryOrbType.Nether: return new OrbDamageRange { MinValue = 1, MidValue = 6, MaxValue = 12 };
				case EInventoryOrbType.Barrier: return new OrbDamageRange { MinValue = 2, MidValue = 8, MaxValue = 20 };
				default: return new OrbDamageRange { MinValue = 6, MidValue = 6, MaxValue = 6 }; //MONSKE??? But I thought you were dead???
			}
		}

		private static List<OrbDamageOdds> GetOrbOdds(string setting, List<OrbDamageOdds> overrides)
		{
			return new List<OrbDamageOdds>();
		}

		public static void RandomizeOrb(OrbDamageOdds orbOdds, Random random)
		{
			var options = GetOrbDamageOptions(orbOdds.orbType);
			int newDamage;
			switch (orbOdds.GetModifier(random))
			{
				case OrbDamageOdds.OrbDamageModifier.Minus:
					newDamage = options.MinValue;
					OverrideOrbNames(orbOdds.orbType, "(-)");
					break;
				case OrbDamageOdds.OrbDamageModifier.Plus:
					newDamage = options.MaxValue;
					OverrideOrbNames(orbOdds.orbType, "(+)");
					break;
				default:
					newDamage = options.MidValue;
					break;

			}
			if (!OrbDamageLookup.ContainsKey((int)orbOdds.orbType))
			{
				OrbDamageLookup.Add((int)orbOdds.orbType, newDamage);
			}
		}

		public static void OverrideOrbNames(EInventoryOrbType orbType, string suffix)
		{
			var Localizer = TimeSpinnerGame.Localizer;
			string locKey = $"inv_orb_{orbType}";
			string spellLocKey = $"{locKey}_spell";
			string ringLocKey = $"{locKey}_passive";
			string actualOrbName = new InventoryOrb(orbType).Name;
			string actualSpellName = new InventoryOrb(orbType).AsDynamic().SpellName;
			string actualRingName = new InventoryOrb(orbType).AsDynamic().PassiveName;
			if (!actualOrbName.EndsWith(suffix))
				Localizer.OverrideKey(locKey, $"{actualOrbName} {suffix}");
			if (!actualSpellName.EndsWith(suffix))
				Localizer.OverrideKey(spellLocKey, $"{actualSpellName} {suffix}");
			if (!actualRingName.EndsWith(suffix))
				Localizer.OverrideKey(ringLocKey, $"{actualRingName} {suffix}");
		}

		public static void PopulateOrbLookups(GameSave save, string setting, List<OrbDamageOdds> overrides)
		{
			OrbDamageLookup.Clear();
			List<OrbDamageOdds> orbOdds = GetOrbOdds(setting, overrides);
			var random = new Random((int)save.GetSeed().Value.Id);
			//foreach (EInventoryOrbType orbType in Enum.GetValues(typeof(EInventoryOrbType)))
			foreach (OrbDamageOdds odds in orbOdds)
			{
				RandomizeOrb(odds, random);
				var orbInventory = save.Inventory.OrbInventory.Inventory;
				if (orbInventory.ContainsKey((int)odds.orbType))
					SetOrbBaseDamage(orbInventory[(int)odds.orbType]);
			}
		}

		public static void SetOrbBaseDamage(InventoryOrb orb)
		{
			if (OrbDamageLookup.TryGetValue((int)orb.OrbType, out int storedOrbDamage))
			{
				orb.BaseDamage = storedOrbDamage;
			}
		}

		public static void UpdateOrbDamage(GameSave save, Protagonist lunais)
		{
			var OrbManagerType = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrbManager");
			var RefreshDamage = OrbManagerType.GetMethod("RefreshDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var orbManager = lunais.AsDynamic()._orbManager;
			var inventory = save.Inventory;
			var currentOrbAType = inventory.EquippedMeleeOrbA;
			var currentOrbBType = inventory.EquippedMeleeOrbB;
			var currentSpellType = inventory.EquippedSpellOrb;
			var currentRingType = inventory.EquippedPassiveOrb;
			var orbA = GetOrbFromType(inventory.OrbInventory, currentOrbAType);
			var orbB = GetOrbFromType(inventory.OrbInventory, currentOrbBType);
			var spell = GetOrbFromType(inventory.OrbInventory, currentSpellType);
			var ring = GetOrbFromType(inventory.OrbInventory, currentRingType);
			if (orbA != null) SetOrbBaseDamage(orbA);
			if (orbB != null) SetOrbBaseDamage(orbB);
			if (spell != null) SetOrbBaseDamage(spell);
			if (ring != null) SetOrbBaseDamage(ring);
			RefreshDamage.Invoke(orbManager, null);
		}

		private static InventoryOrb GetOrbFromType(InventoryOrbCollection inventory, EInventoryOrbType orbType)
		{
			return inventory.GetItem((int)orbType);
		}
	}
}
