using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

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
		public static Dictionary<int, int> OrbLevelLookup = new Dictionary<int, int>();
		private static OrbDamageRange GetOrbDamageRange(EInventoryOrbType orbType)
        {
			switch(orbType)
            {
				case EInventoryOrbType.Blue: return new OrbDamageRange { MinValue = 2, MidValue = 4, MaxValue = 6 };
				case EInventoryOrbType.Blade: return new OrbDamageRange { MinValue = 4, MidValue = 7, MaxValue = 10 };
				case EInventoryOrbType.Flame: return new OrbDamageRange { MinValue = 3, MidValue = 6, MaxValue = 9 };
				case EInventoryOrbType.Pink: return new OrbDamageRange { MinValue = 5, MidValue = 6, MaxValue = 10 };
				case EInventoryOrbType.Iron: return new OrbDamageRange { MinValue = 7, MidValue = 10, MaxValue = 15 };
				case EInventoryOrbType.Ice: return new OrbDamageRange { MinValue = 2, MidValue = 3, MaxValue = 7 };
				case EInventoryOrbType.Wind: return new OrbDamageRange { MinValue = 2, MidValue = 3, MaxValue = 6 };
				case EInventoryOrbType.Gun: return new OrbDamageRange { MinValue = 7, MidValue = 9, MaxValue = 15 };
				case EInventoryOrbType.Umbra: return new OrbDamageRange { MinValue = 3, MidValue = 4, MaxValue = 7 };
				case EInventoryOrbType.Empire: return new OrbDamageRange { MinValue = 5, MidValue = 10, MaxValue = 17 };
				case EInventoryOrbType.Eye: return new OrbDamageRange { MinValue = 2, MidValue = 3, MaxValue = 5 };
				case EInventoryOrbType.Blood: return new OrbDamageRange { MinValue = 2, MidValue =3, MaxValue = 6 };
					//may need to revisit book if we can independently randomize spell damage
					//djinn inferno is an interesting case innit
				case EInventoryOrbType.Book: return new OrbDamageRange { MinValue = 4, MidValue = 6, MaxValue = 8 };
					//same thing here with Moon/Bombardment
				case EInventoryOrbType.Moon: return new OrbDamageRange { MinValue = 2, MidValue = 3, MaxValue = 5 };
				case EInventoryOrbType.Nether: return new OrbDamageRange { MinValue = 4, MidValue = 6, MaxValue = 8 };
				case EInventoryOrbType.Barrier: return new OrbDamageRange { MinValue = 5, MidValue = 8, MaxValue = 11 };
				default: return new OrbDamageRange { MinValue = 6, MidValue = 6, MaxValue = 6 }; //MONSKE??? But I thought you were dead???
			}
        }

		private static OrbDamageRange GetOrbDamageOptions(EInventoryOrbType orbType)
		{
			switch (orbType)
			{
				case EInventoryOrbType.Blue: return new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 8 };
				case EInventoryOrbType.Blade: return new OrbDamageRange { MinValue = 1, MidValue = 7, MaxValue = 12 };
				case EInventoryOrbType.Flame: return new OrbDamageRange { MinValue = 2, MidValue = 6, MaxValue = 12 };
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

		public static void RandomizeOrb(EInventoryOrbType orbType, int damageSelection)
		{
			var options = GetOrbDamageOptions(orbType);
			int newDamage;
			switch (damageSelection)
            {
				case int o when (o <= 4): 
					newDamage = options.MinValue;
					OverrideOrbNames(orbType, "(-)");
					break;
				case int o when (o >= 5 && o <= 7): 
					newDamage = options.MidValue;
					break;
				default: 
					newDamage = options.MaxValue;
					OverrideOrbNames(orbType, "(+)");
					break;

            }
			if (!OrbDamageLookup.ContainsKey((int)orbType))
			{
				OrbDamageLookup.Add((int)orbType, newDamage);
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
			if(!actualOrbName.EndsWith(suffix))
				Localizer.OverrideKey(locKey, $"{actualOrbName} {suffix}");
			if(!actualSpellName.EndsWith(suffix))
				Localizer.OverrideKey(spellLocKey, $"{actualSpellName} {suffix}");
			if(!actualRingName.EndsWith(suffix))
				Localizer.OverrideKey(ringLocKey, $"{actualRingName} {suffix}");
		}

		public static void PopulateOrbLookups(GameSave save)
        {
			OrbDamageLookup.Clear();
			var random = new Random((int)save.GetSeed().Value.Id);
			foreach(EInventoryOrbType orbType in Enum.GetValues(typeof(EInventoryOrbType)))
            {
				int damageSelection = random.Next(0, 9);
				RandomizeOrb(orbType, damageSelection);
				var orbInventory = save.Inventory.OrbInventory.Inventory;
				if (orbInventory.ContainsKey((int)orbType))
					SetOrbBaseDamage(orbInventory[(int)orbType], save);
			}
        }

		public static void SetOrbBaseDamage(InventoryOrb orb, GameSave save)
		{
			if(OrbDamageLookup.TryGetValue((int)orb.OrbType, out int storedOrbDamage))
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
			var orbA = inventory.OrbInventory.GetItem((int)currentOrbAType);
			var orbB = inventory.OrbInventory.GetItem((int)currentOrbBType);
			var spell = inventory.OrbInventory.GetItem((int)currentSpellType);
			var ring = inventory.OrbInventory.GetItem((int)currentRingType);
			if (orbA != null) SetOrbBaseDamage(orbA, save);
			if (orbB != null) SetOrbBaseDamage(orbB, save);
			if (spell != null) SetOrbBaseDamage(spell, save);
			if (ring != null) SetOrbBaseDamage(ring, save);
			RefreshDamage.Invoke(orbManager, null);
		}
	}
}
