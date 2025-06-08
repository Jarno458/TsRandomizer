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
	struct OrbNames
	{
		public string OrbName;
		public string SpellName;
		public string RingName;
	}

	struct OrbAttributes
    {
		public OrbDamageRange DamageRange;
		public OrbNames Names;
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

		static OrbAttributes GetOrbDetails(EInventoryOrbType orbType)
		{
			switch (orbType)
			{
				case EInventoryOrbType.Blue: 
					return new OrbAttributes {
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 8 },
						Names = new OrbNames { OrbName = "Blue Orb", SpellName = "Aura Blast", RingName = "Bleak Ring" }
					};
				case EInventoryOrbType.Blade:
					return new OrbAttributes {
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 7, MaxValue = 12 },
						Names = new OrbNames { OrbName = "Blade Orb", SpellName = "Colossal Blade", RingName = "Scythe Ring" }
					};
				case EInventoryOrbType.Flame:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 2, MidValue = 6, MaxValue = 16 }, 
						Names = new OrbNames { OrbName = "Fire Orb", SpellName = "Infernal Flames", RingName = "Pyro Ring" }
					};
				case EInventoryOrbType.Pink:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 2, MidValue = 6, MaxValue = 30 }, 
						Names = new OrbNames { OrbName = "Plasma Orb", SpellName = "Plasma Geyser", RingName = "Royal Ring" }
					};
				case EInventoryOrbType.Iron:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 2, MidValue = 10, MaxValue = 20 }, 
						Names = new OrbNames { OrbName = "Iron Orb", SpellName = "Colossal Hammer", RingName = "Shield Ring" }
					};
				case EInventoryOrbType.Ice:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 12 }, 
						Names = new OrbNames { OrbName = "Ice Orb", SpellName = "Frozen Spires", RingName = "Icicle Ring" }};
				case EInventoryOrbType.Wind:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 }, 
						Names = new OrbNames { OrbName = "Wind Orb", SpellName = "Storm Eye", RingName = "Tailwind Ring" }};
				case EInventoryOrbType.Gun:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 3, MidValue = 9, MaxValue = 30 }, 
						Names = new OrbNames { OrbName = "Gun Orb", SpellName = "Arm Cannon", RingName = "Economizer Ring" }};
				case EInventoryOrbType.Umbra:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 4, MaxValue = 10 }, 
						Names = new OrbNames { OrbName = "Umbra Orb", SpellName = "Dark Flames", RingName = "Tailwind Ring" }};
				case EInventoryOrbType.Empire:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 2, MidValue = 10, MaxValue = 20 }, 
						Names = new OrbNames { OrbName = "Empire Orb", SpellName = "Aura Serpent", RingName = "Star of Lachiem" }};
				case EInventoryOrbType.Eye:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 }, 
						Names = new OrbNames { OrbName = "Eye Orb", SpellName = "Chaos Blades", RingName = "Oculus Ring" }};
				case EInventoryOrbType.Blood:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 }, 
						Names = new OrbNames { OrbName = "Blood Orb", SpellName = "Crimson Vortex", RingName = "Sanguine Ring" }};
				case EInventoryOrbType.Book:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 6, MaxValue = 12 }, 
						Names = new OrbNames { OrbName = "Forbidden Tome", SpellName = "Djinn Inferno", RingName = "Sun Ring" }};
				case EInventoryOrbType.Moon:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 3, MaxValue = 8 }, 
						Names = new OrbNames { OrbName = "Shattered Orb", SpellName = "Bombardment", RingName = "Silence Ring" }};
				case EInventoryOrbType.Nether:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 1, MidValue = 6, MaxValue = 12 }, 
						Names = new OrbNames { OrbName = "Nether Orb", SpellName = "Corruption", RingName = "Shadow Seal" }};
				case EInventoryOrbType.Barrier:
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 2, MidValue = 8, MaxValue = 20 }, 
						Names = new OrbNames { OrbName = "Radiant Orb", SpellName = "Lightwall", RingName = "Hope Ring" }};
				default: //MONSKE??? But I thought you were dead???
					return new OrbAttributes { 
						DamageRange = new OrbDamageRange { MinValue = 6, MidValue = 6, MaxValue = 6 }, 
						Names = new OrbNames { OrbName = "Monkse Orb", SpellName = "None Spell", RingName = "Left Ring" }};
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
			OrbAttributes orbDetails = GetOrbDetails(orbType);

			int newDamage;

			switch (orbOdds.GetModifier(random))
			{
				case OrbDamageOdds.OrbDamageModifier.Minus:
					newDamage = orbDetails.DamageRange.MinValue;
					OverrideOrbNames(orbType, orbDetails, "(-)");
					break;
				case OrbDamageOdds.OrbDamageModifier.Plus:
					newDamage = orbDetails.DamageRange.MaxValue;
					OverrideOrbNames(orbType, orbDetails, "(+)");
					break;
				default:
					newDamage = orbDetails.DamageRange.MidValue;
					OverrideOrbNames(orbType, orbDetails, "");
					break;
			}
			if (!OrbDamageLookup.ContainsKey((int)orbType))
			{
				OrbDamageLookup.Add((int)orbType, newDamage);
			}
		}

		public static void OverrideOrbNames(EInventoryOrbType orbType, OrbAttributes orbDetails, string suffix)
		{
			string locKey = $"inv_orb_{orbType}";
			string spellLocKey = $"{locKey}_spell";
			string ringLocKey = $"{locKey}_passive";
			string actualOrbName = orbDetails.Names.OrbName;
			string actualSpellName = orbDetails.Names.SpellName;
			string actualRingName = orbDetails.Names.RingName;

			TimeSpinnerGame.Localizer.OverrideKey(locKey, $"{actualOrbName} {suffix}");
			TimeSpinnerGame.Localizer.OverrideKey(spellLocKey, $"{actualSpellName} {suffix}");
			TimeSpinnerGame.Localizer.OverrideKey(ringLocKey, $"{actualRingName} {suffix}");
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
