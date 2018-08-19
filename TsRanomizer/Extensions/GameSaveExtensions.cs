using System;
using System.Reflection;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;

namespace TsRanodmizer.Extensions
{
	static class GameSaveExtensions
	{
		const string SeedSaveFileKey = "TsRandomizerSeed";
		const string MeleeOrbPrefixKey = "TsRandomizerHasMeleeOrb";

		static readonly MethodInfo GetAreaNameMethod = typeof(Level)
			.GetMethod("GetLevelNameFromID", BindingFlags.Static | BindingFlags.NonPublic, 
				null, new []{ typeof(int) }, null);

		public static Seed FindSeed(this GameSave gameSave)
		{
			if (gameSave.DataKeyInts.TryGetValue(SeedSaveFileKey, out var seed))
				return new Seed(seed);

			return null;
		}

		public static void SetSeed(this GameSave gameSave, Seed seed)
		{
			gameSave.DataKeyInts[SeedSaveFileKey] = seed;
		}

		public static string GetAreaName(this GameSave gameSave)
		{
			return (string)GetAreaNameMethod.Invoke(null, new object[]{ gameSave.CurrentLevel });
		}

		public static bool HasMeleeOrb(this GameSave gameSave, EInventoryOrbType orbType)
		{
			return gameSave.DataKeyBools.ContainsKey(MeleeOrbPrefixKey + (int)orbType);
		}

		public static bool HasRelic(this GameSave gameSave, EInventoryRelicType relic)
		{
			return gameSave.DataKeyBools.ContainsKey(MeleeOrbPrefixKey + (int)relic);
		}

		public static void AddOrb(this GameSave gameSave, EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			var orbCollection = gameSave.Inventory.OrbInventory.Inventory;
			var orbTypeKey = (int)orbType;

			if (!orbCollection.ContainsKey(orbTypeKey))
				orbCollection.Add(orbTypeKey, new InventoryOrb(orbType));

			switch (orbSlot)
			{
				case EOrbSlot.Melee:
					gameSave.DataKeyBools.Add(MeleeOrbPrefixKey + orbTypeKey, true);
					break;
				case EOrbSlot.Spell:
					orbCollection[orbTypeKey].IsSpellUnlocked = true;
					break;
				case EOrbSlot.Passive:
					orbCollection[orbTypeKey].IsPassiveUnlocked = true;
					break;
				case EOrbSlot.All:
					gameSave.DataKeyBools.Add(MeleeOrbPrefixKey + orbTypeKey, true);
					orbCollection[orbTypeKey].IsSpellUnlocked = true;
					orbCollection[orbTypeKey].IsPassiveUnlocked = true;
					break;
			}
		}
	}
}
