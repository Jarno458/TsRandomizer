using System;
using System.Collections.Generic;
using System.Linq;
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
		public static HashSet<int> MainOrbDamagedEnemies = new HashSet<int>();
		public static HashSet<int> SubOrbDamagedEnemies = new HashSet<int>();
		public static HashSet<int> SpellDamagedEnemies = new HashSet<int>();

		private static readonly Type LunaisOrbManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrbManager");
		private static readonly Type LunaisOrb = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrb");
		private static readonly Type LunaisSpell = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Spells.LunaisSpell");
		private static readonly Type LunaisOrbAbility = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Lunais.BaseClasses.LunaisOrbAbility");
		private static readonly Type LunaisSpellManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Spells.LunaisSpellManager");
		private static readonly PropertyInfo mainOrbProperty = LunaisOrbManager.GetProperty("MainOrb", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo subOrbProperty = LunaisOrbManager.GetProperty("SubOrb", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo spellProperty = LunaisSpellManager.GetProperty("EquippedSpell", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo orbColorProperty = LunaisOrb.GetProperty("OrbColor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo spellTypeProperty = LunaisSpell.GetProperty("SpellType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

		public static void UpdateHitRegistry(Protagonist lunais)
		{
			var hitEnemyRegistryProperty = LunaisOrbAbility.GetProperty("HitEnemyRegistry", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			var orbManager = lunais.AsDynamic()._orbManager;
			var spellManager = lunais.AsDynamic()._spellManager;
			var mainOrb = mainOrbProperty.GetValue(orbManager, null);
			if (mainOrb != null)
				MainOrbDamagedEnemies.UnionWith(hitEnemyRegistryProperty.GetValue(mainOrb, null));

			var subOrb = subOrbProperty.GetValue(orbManager, null);
			if (subOrb != null)
				SubOrbDamagedEnemies.UnionWith(hitEnemyRegistryProperty.GetValue(subOrb, null));

			var spell = spellProperty.GetValue(spellManager, null);
			if (spell != null)
				SpellDamagedEnemies.UnionWith(hitEnemyRegistryProperty.GetValue(spell, null));
		}

		public static void ResetHitRegistry()
		{
			MainOrbDamagedEnemies.Clear();
			SubOrbDamagedEnemies.Clear();
			SpellDamagedEnemies.Clear();
		}

		public static void UpdateOrbXp(Level level, Protagonist lunais, double extraXp)
		{
			var inventory = level.GameSave.Inventory;
			var hasEarringsEquipped = inventory.EquippedTrinketA == EInventoryEquipmentType.NelisteEarring
					|| inventory.EquippedTrinketB == EInventoryEquipmentType.NelisteEarring;

			if (!hasEarringsEquipped) return;
			// minus the xp from the base game, minus the xp from the call to GiveOrbExperience to run the level up logic
			int extraXpToAdd = (int)(extraXp - 2);
			var levelReflected = level.AsDynamic();
			dynamic orbManager = lunais.AsDynamic()._orbManager;
			var spellManager = lunais.AsDynamic()._spellManager;
			var ringManager = lunais.AsDynamic()._passiveManager;
			Dictionary<int, Monster> enemies = levelReflected._enemies;
			var deadMonsters = enemies.Where(e => e.Value.HP <= 0).Select(e => e.Value);

			foreach (Monster deadMonster in deadMonsters)
			{
				if (MainOrbDamagedEnemies.Contains(deadMonster.ID))
				{
					var mainOrb = mainOrbProperty.GetValue(orbManager, null);
					EInventoryOrbType orbColor = orbColorProperty.GetValue(mainOrb, null);
					var mainOrbInv = inventory.OrbInventory.GetItem((int)orbColor);
					mainOrbInv.Experience += extraXpToAdd;
					inventory.OrbInventory.GiveOrbExperience(orbColor, false);
					MainOrbDamagedEnemies.RemoveWhere(x => x == deadMonster.ID);
				}
				if (SubOrbDamagedEnemies.Contains(deadMonster.ID))
				{
					var subOrb = subOrbProperty.GetValue(orbManager, null);
					EInventoryOrbType orbColor = orbColorProperty.GetValue(subOrb, null);
					var subOrbInv = inventory.OrbInventory.GetItem((int)orbColor);
					subOrbInv.Experience += extraXpToAdd;
					inventory.OrbInventory.GiveOrbExperience(orbColor, false);
					SubOrbDamagedEnemies.RemoveWhere(x => x == deadMonster.ID);
				}
				if (SpellDamagedEnemies.Contains(deadMonster.ID))
				{
					var spell = spellProperty.GetValue(spellManager, null);
					EInventoryOrbType spellType = spellTypeProperty.GetValue(spell, null);
					var spellOrb = inventory.OrbInventory.GetItem((int)spellType);
					spellOrb.Experience += extraXpToAdd;
					inventory.OrbInventory.GiveOrbExperience(spellType, false);
					SpellDamagedEnemies.RemoveWhere(x => x == deadMonster.ID);
				}

			}
		}
	}
}
