using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	struct OrbDamageRange
    {
		public int MinValue;
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
				case EInventoryOrbType.Blue: return new OrbDamageRange { MinValue = 2, MaxValue = 6 };
				case EInventoryOrbType.Blade: return new OrbDamageRange { MinValue = 4, MaxValue = 10 };
				case EInventoryOrbType.Flame: return new OrbDamageRange { MinValue = 3, MaxValue = 9 };
				case EInventoryOrbType.Pink: return new OrbDamageRange { MinValue = 5, MaxValue = 10 };
				case EInventoryOrbType.Iron: return new OrbDamageRange { MinValue = 7, MaxValue = 15 };
				case EInventoryOrbType.Ice: return new OrbDamageRange { MinValue = 2, MaxValue = 7 };
				case EInventoryOrbType.Wind: return new OrbDamageRange { MinValue = 2, MaxValue = 6 };
				case EInventoryOrbType.Gun: return new OrbDamageRange { MinValue = 7, MaxValue = 15 };
				case EInventoryOrbType.Umbra: return new OrbDamageRange { MinValue = 3, MaxValue = 7 };
				case EInventoryOrbType.Empire: return new OrbDamageRange { MinValue = 5, MaxValue = 17 };
				case EInventoryOrbType.Eye: return new OrbDamageRange { MinValue = 2, MaxValue = 5 };
				case EInventoryOrbType.Blood: return new OrbDamageRange { MinValue = 2, MaxValue = 6 };
					//may need to revisit book if we can independently randomize spell damage
					//djinn inferno is an interesting case innit
				case EInventoryOrbType.Book: return new OrbDamageRange { MinValue = 4, MaxValue = 8 };
					//same thing here with Moon/Bombardment
				case EInventoryOrbType.Moon: return new OrbDamageRange { MinValue = 2, MaxValue = 5 };
				case EInventoryOrbType.Nether: return new OrbDamageRange { MinValue = 4, MaxValue = 8 };
				case EInventoryOrbType.Barrier: return new OrbDamageRange { MinValue = 5, MaxValue = 11 };
				default: return new OrbDamageRange { MinValue = 6, MaxValue = 6 }; //MONSKE??? But I thought you were dead???
			}
        }

		private static int GetOrbKey(GameSave save, EInventoryOrbType orbType)
        {
			return save.GetSeed().GetHashCode() + (int)orbType;
        }

		public static void RandomizeOrb(EInventoryOrbType orbType, GameSave save)
		{
			int orbKey = GetOrbKey(save, orbType);
			OrbDamageRange range = GetOrbDamageRange(orbType);
			Random r = new Random(orbKey);
			
			var newDamage = r.Next(range.MinValue, range.MaxValue);
			if (!OrbDamageLookup.ContainsKey(orbKey))
			{
				OrbDamageLookup.Add(orbKey, newDamage);
				save.Inventory.OrbInventory.Inventory[(int)orbType].BaseDamage = newDamage;
			}
				

		}

		public static void PopulateOrbLookups(GameSave save)
        {
			OrbDamageLookup.Clear();
            foreach (var orbItem in save.Inventory.OrbInventory.Inventory)
            {
				var orb = orbItem.Value;
				int orbKey = GetOrbKey(save, orb.OrbType);
				OrbDamageLookup.Add(orbKey, orb.BaseDamage);
            }
        }
		public static void ResetOrbBaseDamage(InventoryOrb orb, GameSave save)
		{
			int orbKey = GetOrbKey(save, orb.OrbType);
			if(OrbDamageLookup.TryGetValue(orbKey, out int storedOrbDamage))
            {
				orb.BaseDamage = storedOrbDamage;
			}	
		}
	}
}
