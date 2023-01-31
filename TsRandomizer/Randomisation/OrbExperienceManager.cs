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
		public static HashSet<int> MainOrbDamagedEnemies = new HashSet<int>();
		public static HashSet<int> SubOrbDamagedEnemies = new HashSet<int>();
		public static HashSet<int> SpellDamagedEnemies = new HashSet<int>();

		private static readonly Type LunaisOrbManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrbManager");
		private static readonly Type LunaisOrbAbility = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Lunais.BaseClasses.LunaisOrbAbility");
		private static readonly Type LunaisSpellManager = TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Spells.LunaisSpellManager");
		private static readonly PropertyInfo mainOrbProperty = LunaisOrbManager.GetProperty("MainOrb", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo subOrbProperty = LunaisOrbManager.GetProperty("SubOrb", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly PropertyInfo spellProperty = LunaisSpellManager.GetProperty("EquippedSpell", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

		public static void UpdateHitRegistry(Protagonist lunais)
		{
			var hitEnemyRegistryProperty = LunaisOrbAbility.GetProperty("HitEnemyRegistry", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			var orbManager = lunais.AsDynamic()._orbManager;
			var spellManager = lunais.AsDynamic()._spellManager;
			var mainOrb = mainOrbProperty.GetValue(orbManager, null);
			MainOrbDamagedEnemies.UnionWith(hitEnemyRegistryProperty.GetValue(mainOrb, null));

			var subOrb = subOrbProperty.GetValue(orbManager, null);
			SubOrbDamagedEnemies.UnionWith(hitEnemyRegistryProperty.GetValue(subOrb, null));

			var spell = spellProperty.GetValue(spellManager, null);
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

			var levelReflected = level.AsDynamic();
			dynamic orbManager = lunais.AsDynamic()._orbManager;
			var spellManager = lunais.AsDynamic()._spellManager;
			var ringManager = lunais.AsDynamic()._passiveManager;
			List<Mobile> deadThings = levelReflected._deadObjects;

			foreach (Mobile deadThing in deadThings)
			{
				if (deadThing.BaseType == EGameObjectBaseType.Monster)
				{
					//we do extra XP -1 and then fire the vanilla GiveOrbExperience for the remaining 1XP to trigger level ups
					if (MainOrbDamagedEnemies.Contains(deadThing.ID))
					{
						var mainOrb = mainOrbProperty.GetValue(orbManager, null);
						var mainOrbInv = inventory.OrbInventory.GetItem((int)mainOrb.OrbColor);
						mainOrbInv.Experience += (int)extraXp - 1;
						inventory.OrbInventory.GiveOrbExperience(mainOrb.OrbColor, false);
						MainOrbDamagedEnemies.RemoveWhere(x => x == deadThing.ID);
					}
					if (SubOrbDamagedEnemies.Contains(deadThing.ID))
					{
						var subOrb = subOrbProperty.GetValue(orbManager, null);
						var subOrbInv = inventory.OrbInventory.GetItem((int)subOrb.OrbColor);
						subOrb.Experience += (int)extraXp - 1;
						inventory.OrbInventory.GiveOrbExperience(subOrb.OrbColor, false);
						SubOrbDamagedEnemies.RemoveWhere(x => x == deadThing.ID);
					}
					if (SpellDamagedEnemies.Contains(deadThing.ID))
					{
						var spell = spellProperty.GetValue(spellManager, null);
						var spellOrb = inventory.OrbInventory.GetItem((int)spell.SpellType);
						spell.Experience += extraXp - 1;
						inventory.OrbInventory.GiveOrbExperience(spell.SpellType, false);
						SpellDamagedEnemies.RemoveWhere(x => x == deadThing.ID);
					}
					var ring = ringManager.AsDynamic()._equippedPassive;
					if (ring != null)
					{
						var relatedOrb = inventory.OrbInventory.GetItem((int)ring.PassiveType);
						relatedOrb.Experience += (int)extraXp - 1;
						inventory.OrbInventory.GiveOrbExperience(ringManager._equippedPassive.PassiveType, false);
					}

				}
			}
		}
	}
}
