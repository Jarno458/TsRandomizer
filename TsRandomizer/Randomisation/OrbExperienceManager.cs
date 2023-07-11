using System;
using System.Collections.Generic;
using System.Reflection;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Randomisation
{
	static class OrbExperienceManager
	{
		enum XpAwardingOrb
		{
			Main,
			Sub,
			Spell
		}

		static readonly Dictionary<XpAwardingOrb, HashSet<int>> DamagedEnemies = new Dictionary<XpAwardingOrb, HashSet<int>>(3) {
			{ XpAwardingOrb.Main, new HashSet<int>() },
			{ XpAwardingOrb.Sub, new HashSet<int>() },
			{ XpAwardingOrb.Spell, new HashSet<int>() },
		};

		static readonly HashSet<int> KilledEnemies = new HashSet<int>();
		
		static readonly Type LunaisOrbManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrbManager");
		static readonly Type LunaisOrb = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrb");
		static readonly Type LunaisSpell = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Spells.LunaisSpell");
		static readonly Type LunaisOrbAbility = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Lunais.BaseClasses.LunaisOrbAbility");
		static readonly Type LunaisSpellManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Spells.LunaisSpellManager");

		static readonly FieldInfo MainOrbProperty = LunaisOrbManager.GetField("_mainOrb", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		static readonly FieldInfo SubOrbProperty = LunaisOrbManager.GetField("_subOrb", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		static readonly FieldInfo SpellProperty = LunaisSpellManager.GetField("_equippedSpell", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		
		static readonly FieldInfo HitEnemyRegistryProperty = LunaisOrbAbility.GetField("_hitEnemyRegistry", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

		static readonly FieldInfo OrbColorProperty = LunaisOrb.GetField("_color", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		static readonly PropertyInfo SpellTypeProperty = LunaisSpell.GetProperty("SpellType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

		static readonly HashSet<string> OrbXpBlacklist = new HashSet<string> {
			"Enemy_XarionBoss_1", 
			"Enemy_IncubusBoss_1", 
			"Enemy_IncubusBoss_2", 
			"Enemy_IncubusBoss_3", 
			"Enemy_IncubusBoss_4"
		};
	
		public static void UpdateHitRegistry(Protagonist lunais)
		{
			UpdateHitRegistry(lunais, XpAwardingOrb.Main);
			UpdateHitRegistry(lunais, XpAwardingOrb.Sub);
			UpdateHitRegistry(lunais, XpAwardingOrb.Spell);
		}

		static void UpdateHitRegistry(Protagonist lunais, XpAwardingOrb orbType)
		{
			var orb = GetOrb(lunais, orbType);

			if (orb == null) 
				return;

			var enemiesHitByOrb = (HashSet<int>)HitEnemyRegistryProperty.GetValue(orb);
			DamagedEnemies[orbType].UnionWith(enemiesHitByOrb);
		}

		public static void ResetHitRegistry()
		{
			foreach (var damagedEnemiesSet in DamagedEnemies.Values)
				damagedEnemiesSet.Clear();

			KilledEnemies.Clear();
		}

		public static void UpdateOrbXp(Level level, Protagonist lunais, double extraXp)
		{
			var inventory = level.GameSave.Inventory;
			var hasEarringsEquipped = inventory.EquippedTrinketA == EInventoryEquipmentType.NelisteEarring
					|| inventory.EquippedTrinketB == EInventoryEquipmentType.NelisteEarring;

			if (!hasEarringsEquipped) 
				return;

			// minus the xp from the base game, minus the xp from the call to GiveOrbExperience to run the level up logic
			var extraXpToAdd = (int)(extraXp - 2);

			Dictionary<int, Monster> enemies = level.AsDynamic()._enemies;
			foreach (var monster in enemies)
			{
				if(monster.Value.HP > 0 || KilledEnemies.Contains(monster.Key))
					continue;

				KilledEnemies.Add(monster.Key);

				var spec = monster.Value.CharacterSpecification;
				if (
					monster.Value.HP > 0
					|| (spec != null && OrbXpBlacklist.Contains(spec.Name)) 
				    || monster.Value.AsDynamic().IsABoss)
					continue;

				AwardOrbXpIfEnemyWasHitByOrb(level, lunais, XpAwardingOrb.Main, monster.Key, extraXpToAdd);
				AwardOrbXpIfEnemyWasHitByOrb(level, lunais, XpAwardingOrb.Sub, monster.Key, extraXpToAdd);
				AwardOrbXpIfEnemyWasHitByOrb(level, lunais, XpAwardingOrb.Spell, monster.Key, extraXpToAdd);
			}
		}

		static void AwardOrbXpIfEnemyWasHitByOrb(Level level, Protagonist lunais, XpAwardingOrb orbType, int enemyId, int xpToAdd)
		{
			if (!DamagedEnemies[orbType].Contains(enemyId)) 
				return;

			AwardOrbXp(level, lunais, orbType, xpToAdd);

			DamagedEnemies[orbType].Remove(enemyId);
		}

		static void AwardOrbXp(Level level, Protagonist lunais, XpAwardingOrb orbType, int xpToAdd)
		{
			var orbColor = GetOrbColor(orbType, GetOrb(lunais, orbType));
			
			var inventoryOrb = level.GameSave.Inventory.OrbInventory.GetItem((int)orbColor);

			inventoryOrb.Experience += xpToAdd;

			level.GameSave.Inventory.OrbInventory.GiveOrbExperience(orbColor, false);
		}

		static object GetOrb(Protagonist lunais, XpAwardingOrb orb)
		{
			switch (orb)
			{
				case XpAwardingOrb.Main:
					return MainOrbProperty.GetValue(lunais.AsDynamic()._orbManager);
				case XpAwardingOrb.Sub:
					return SubOrbProperty.GetValue(lunais.AsDynamic()._orbManager);
				case XpAwardingOrb.Spell:
					return SpellProperty.GetValue(lunais.AsDynamic()._spellManager);
				default:
					throw new ArgumentOutOfRangeException(nameof(orb), orb, null);
			}
		}

		static EInventoryOrbType GetOrbColor(XpAwardingOrb orbType, object orb)
		{
			switch (orbType)
			{
				case XpAwardingOrb.Main:
				case XpAwardingOrb.Sub:
					return (EInventoryOrbType)OrbColorProperty.GetValue(orb);
				case XpAwardingOrb.Spell:
					return (EInventoryOrbType)SpellTypeProperty.GetValue(orb, null);
				default:
					throw new ArgumentOutOfRangeException(nameof(orb), orb, null);
			}
		}
	}
}
