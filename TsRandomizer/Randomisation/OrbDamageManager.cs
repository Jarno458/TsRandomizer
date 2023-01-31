using System;
using System.Collections.Generic;
using System.Reflection;
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

	public class OrbDamageOdds
	{
		public enum OrbDamageModifier
		{
			Minus,
			Normal,
			Plus
		}
		public int MinusOdds { get; set; }
		public int NormalOdds { get; set; }
		public int PlusOdds { get; set; }
		public OrbDamageModifier GetModifier(Random random)
		{
			try
			{
				double sumOfWeights = MinusOdds + PlusOdds + NormalOdds;
				double minusPercent = MinusOdds / sumOfWeights;
				double normalPercent = NormalOdds / sumOfWeights;
				double choice = random.NextDouble();
				switch (choice)
				{
					case double o when (o <= minusPercent && minusPercent != 0):
						return OrbDamageModifier.Minus;
					case double o when (o > minusPercent + normalPercent):
						return OrbDamageModifier.Plus;
					default:
						return OrbDamageModifier.Normal;
				}
			}
			catch
			{
				return OrbDamageModifier.Normal;
			}
		}
	}

	static class OrbDamageManager
	{
		static Type OrbManagerType = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrbManager");
		static MethodInfo RefreshDamage = OrbManagerType.GetMethod("RefreshDamage", BindingFlags.NonPublic | BindingFlags.Instance);

		public static Dictionary<int, int> OrbDamageLookup = new Dictionary<int, int>();
		static readonly OrbDamageOdds nerfed = new OrbDamageOdds
		{
			MinusOdds = 1,
			NormalOdds = 0,
			PlusOdds = 0
		};
		static readonly OrbDamageOdds mostlyNerfed = new OrbDamageOdds
		{
			MinusOdds = 5,
			NormalOdds = 2,
			PlusOdds = 1
		};
		static readonly OrbDamageOdds balanced = new OrbDamageOdds
		{
			MinusOdds = 1,
			NormalOdds = 1,
			PlusOdds = 1
		};
		static readonly OrbDamageOdds mostlyBuffed = new OrbDamageOdds
		{
			MinusOdds = 1,
			NormalOdds = 2,
			PlusOdds = 5
		};
		static readonly OrbDamageOdds buffed = new OrbDamageOdds
		{
			MinusOdds = 0,
			NormalOdds = 0,
			PlusOdds = 1
		};

		static OrbDamageRange GetOrbDamageOptions(EInventoryOrbType orbType)
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

		static Dictionary<string, OrbDamageOdds> GetPresetOdds(OrbDamageOdds preset)
		{
			var defaultOdds = new Dictionary<string, OrbDamageOdds>();
			foreach (EInventoryOrbType orbType in Enum.GetValues(typeof(EInventoryOrbType)))
			{
				if (orbType != EInventoryOrbType.Monske && orbType != EInventoryOrbType.None)
				{
					defaultOdds.Add(orbType.ToString(), preset);
				}
			}
			return defaultOdds;
		}

		static Dictionary<string, OrbDamageOdds> GetOrbOdds(string setting, Dictionary<string, OrbDamageOdds> overrides)
		{
			Dictionary<string, OrbDamageOdds> returnOdds;
			switch (setting)
			{
				case "All Nerfs":
					returnOdds = GetPresetOdds(nerfed);
					break;
				case "Mostly Nerfs":
					returnOdds = GetPresetOdds(mostlyNerfed);
					break;
				case "Mostly Buffs":
					returnOdds = GetPresetOdds(mostlyBuffed);
					break;
				case "All Buffs":
					returnOdds = GetPresetOdds(buffed);
					break;
				case "Manual":
					returnOdds = GetPresetOdds(balanced);
					foreach (var item in overrides)
						returnOdds[item.Key] = item.Value;
					break;
				default:
					returnOdds = GetPresetOdds(balanced);
					break;
			}

			return returnOdds;
		}

		public static void RandomizeOrb(string orbTypeName, OrbDamageOdds orbOdds, Random random)
		{
			var orbType = (EInventoryOrbType)Enum.Parse(typeof(EInventoryOrbType), orbTypeName);
			var options = GetOrbDamageOptions(orbType);

			int newDamage;

			switch (orbOdds.GetModifier(random))
			{
				case OrbDamageOdds.OrbDamageModifier.Minus:
					newDamage = options.MinValue;
					OverrideOrbNames(orbType, "(-)");
					break;
				case OrbDamageOdds.OrbDamageModifier.Plus:
					newDamage = options.MaxValue;
					OverrideOrbNames(orbType, "(+)");
					break;
				default:
					newDamage = options.MidValue;
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

			if (!actualOrbName.EndsWith(suffix))
				Localizer.OverrideKey(locKey, $"{actualOrbName} {suffix}");
			if (!actualSpellName.EndsWith(suffix))
				Localizer.OverrideKey(spellLocKey, $"{actualSpellName} {suffix}");
			if (!actualRingName.EndsWith(suffix))
				Localizer.OverrideKey(ringLocKey, $"{actualRingName} {suffix}");
		}

		public static void PopulateOrbLookups(GameSave save, string setting, Dictionary<string, OrbDamageOdds> overrides)
		{
			OrbDamageLookup.Clear();

			var orbOdds = GetOrbOdds(setting, overrides);

			var random = new Random((int)save.GetSeed().Value.Id);

			foreach (var odds in orbOdds)
			{
				var orbType = (EInventoryOrbType)Enum.Parse(typeof(EInventoryOrbType), odds.Key);

				RandomizeOrb(odds.Key, odds.Value, random);

				var orbInventory = save.Inventory.OrbInventory.Inventory;
				if (orbInventory.ContainsKey((int)orbType))
					SetOrbBaseDamage(orbInventory[(int)orbType]);
			}
		}

		public static void SetOrbBaseDamage(InventoryOrb orb)
		{
			if (OrbDamageLookup.TryGetValue((int)orb.OrbType, out var storedOrbDamage))
				orb.BaseDamage = storedOrbDamage;
		}

		public static void UpdateOrbDamage(GameSave save, Protagonist lunais)
		{
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

			RefreshDamage.Invoke(lunais.AsDynamic()._orbManager, null);
		}

		static InventoryOrb GetOrbFromType(InventoryOrbCollection inventory, EInventoryOrbType orbType) =>
			inventory.GetItem((int)orbType);
	}
}
