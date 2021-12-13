using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Extensions
{
	static class GameSaveExtensions
	{
		const string ConcussionCountFileKey = "TsRandomizerBonks";
		const string SeedSaveFileKey = "TsRandomizerSeed";
		const string FillMethodSaveFileKey = "TsRandomizerFillMethod";
		const string MeleeOrbPrefixKey = "TsRandomizerHasMeleeOrb";
		const string FamilierPrefixKey = "TsRandomizerHasFamiliar";


		internal static Seed? GetSeed(this GameSave gameSave)
		{
			if (gameSave.DataKeyStrings.TryGetValue(SeedSaveFileKey, out var seedString)
				&& Seed.TryParse(seedString, out var seed))
					return seed;

			return null;
		}

		internal static void SetSeed(this GameSave gameSave, Seed seed)
		{
			gameSave.DataKeyStrings[SeedSaveFileKey] = seed.ToString();
			gameSave.DataKeyInts[ConcussionCountFileKey] = 0;
		}

		internal static FillingMethod GetFillingMethod(this GameSave gameSave)
		{
			if(!gameSave.DataKeyStrings.ContainsKey(FillMethodSaveFileKey))
				return FillingMethod.Forward;

			if(!Enum.TryParse(gameSave.DataKeyStrings[FillMethodSaveFileKey], out FillingMethod fillingMethod))
				throw new Exception("Cannot parse filling method");

			return fillingMethod;
		}

		internal static void SetFillingMethod(this GameSave gameSave, FillingMethod fillingMethod)
		{
			gameSave.DataKeyStrings[FillMethodSaveFileKey] = fillingMethod.ToString();
		}

		internal static bool HasMeleeOrb(this GameSave gameSave, EInventoryOrbType orbType)
		{
			return gameSave.DataKeyBools.ContainsKey(MeleeOrbPrefixKey + (int) orbType);
		}

		internal static bool HasFamiliar(this GameSave gameSave, EInventoryFamiliarType familiar)
		{
			return gameSave.DataKeyBools.ContainsKey(FamilierPrefixKey + (int)familiar);
		}

		internal static bool HasRelic(this GameSave gameSave, EInventoryRelicType relic)
		{
			return gameSave.Inventory.RelicInventory.Inventory.ContainsKey((int)relic);
		}

		internal static bool HasOrb(this GameSave gameSave, EInventoryOrbType orbType)
		{
			return gameSave.Inventory.OrbInventory.Inventory.ContainsKey((int) orbType);
		}

		internal static bool HasRing(this GameSave gameSave, EInventoryOrbType orbType)
		{
			var orbCollection = gameSave.Inventory.OrbInventory.Inventory;

			return orbCollection.TryGetValue((int)orbType, out var orb) && orb.IsPassiveUnlocked;
		}

		internal static bool HasSpell(this GameSave gameSave, EInventoryOrbType orbType)
		{
			var orbCollection = gameSave.Inventory.OrbInventory.Inventory;

			return orbCollection.TryGetValue((int)orbType, out var orb) && orb.IsSpellUnlocked;
		}

		internal static bool HasItem(this GameSave gameSave, ItemIdentifier item)
		{
			switch (item.LootType)
			{
				case LootType.ConstEquipment:
					return gameSave.Inventory.EquipmentInventory.Inventory.ContainsKey((int)item.Enquipment);
				case LootType.ConstFamiliar:
					return gameSave.Inventory.FamiliarInventory.Inventory.ContainsKey((int)item.Familiar);
				case LootType.ConstRelic:
					return gameSave.Inventory.RelicInventory.Inventory.ContainsKey((int)item.Relic);
				case LootType.ConstUseItem:
					return gameSave.Inventory.UseItemInventory.Inventory.ContainsKey((int)item.UseItem);
				case LootType.ConstOrb:
					switch (item.OrbSlot)
					{
						case EOrbSlot.Melee:
							return HasMeleeOrb(gameSave, item.OrbType);
						case EOrbSlot.Spell:
							return HasSpell(gameSave, item.OrbType);
						case EOrbSlot.Passive:
							return HasRing(gameSave, item.OrbType);
						default:
							return HasMeleeOrb(gameSave, item.OrbType)
								&& HasSpell(gameSave, item.OrbType)
								&& HasRing(gameSave, item.OrbType);
					}
				default:
					throw new NotImplementedException($"PlayerIventory.Contains is not supported for loottype {item.LootType}");
			}
		}

		internal static bool HasCutsceneBeenTriggered(this GameSave gameSave, string cutsceneEnunMember)
		{
			var cutsceneEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Cutscene.CutsceneBase+ECutsceneType");

			return gameSave.GetSaveBool($"Cutscene_{cutsceneEnumType.GetEnumValue(cutsceneEnunMember)}");
		}

		static void AddOrb(this GameSave gameSave, EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			var orbCollection = gameSave.Inventory.OrbInventory.Inventory;
			var orbTypeKey = (int) orbType;
			var newOrb = new InventoryOrb(orbType);
			if (!orbCollection.ContainsKey(orbTypeKey))
				orbCollection.Add(orbTypeKey, newOrb);

			if (gameSave.GetSeed().Value.Options.DamageRando) OrbDamageManager.SetOrbBaseDamage(newOrb);

			switch (orbSlot)
			{
				case EOrbSlot.Melee:
					gameSave.DataKeyBools[MeleeOrbPrefixKey + orbTypeKey] = true;
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

		static void AddEnquipment(this GameSave gameSave, EInventoryEquipmentType enquipment)
		{
			gameSave.Inventory.EquipmentInventory.AddItem((int) enquipment);
		}

		static void AddUseItem(this GameSave gameSave, EInventoryUseItemType useItem)
		{
			gameSave.Inventory.UseItemInventory.AddItem((int)useItem);
		}

		static void AddRelic(this GameSave gameSave, EInventoryRelicType relic)
		{
			gameSave.Inventory.RelicInventory.AddItem((int)relic);
		}

		static void AddFamiliar(this GameSave gameSave, EInventoryFamiliarType familiar)
		{
			gameSave.Inventory.FamiliarInventory.AddItem((int)familiar);

			gameSave.DataKeyBools[FamilierPrefixKey + (int)familiar] = true;
		}

		static void AddStat(this GameSave gameSave, Level level, EItemType stat)
		{
			if (level.MainHero == null) return;

			var lunais = level.MainHero.AsDynamic();

			switch (stat)
			{
				case EItemType.MaxHP:
					gameSave.CharacterStats.MaxHPFound++;
					lunais.RefreshCharacterStats(true);
					lunais.HP = lunais.MaxHP;
					break;
				case EItemType.MaxAura:
					gameSave.CharacterStats.MaxAuraFound++;
					lunais.RefreshCharacterStats(true);
					((object)lunais._spellManager).AsDynamic().Aura = (float)lunais.MaxAura;
					break;
				case EItemType.MaxSand:
					gameSave.CharacterStats.MaxSandFound++;
					lunais.RefreshCharacterStats(true);
					lunais.MP = lunais.MaxMP;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
			}
		}

		internal static void AddItem(this GameSave gameSave, Level level, ItemIdentifier itemInfo)
		{
			switch (itemInfo.LootType)
			{
				case LootType.ConstOrb:
					gameSave.AddOrb(itemInfo.OrbType, itemInfo.OrbSlot);
					break;
				case LootType.ConstEquipment:
					gameSave.AddEnquipment(itemInfo.Enquipment);
					break;
				case LootType.ConstUseItem:
					gameSave.AddUseItem(itemInfo.UseItem);
					break;
				case LootType.ConstRelic:
					gameSave.AddRelic(itemInfo.Relic);
					break;
				case LootType.ConstFamiliar:
					gameSave.AddFamiliar(itemInfo.Familiar);
					break;
				case LootType.ConstStat:
					gameSave.AddStat(level, itemInfo.Stat);
					break;
				default:
					throw new ArgumentOutOfRangeException($"LootType {itemInfo.LootType} isnt suppored yet");
			}
		}

		internal static void AddConcussion(this GameSave gameSave)
        {
			if (gameSave.DataKeyInts.ContainsKey(ConcussionCountFileKey))
				gameSave.DataKeyInts[ConcussionCountFileKey]++;
			else
				gameSave.DataKeyInts[ConcussionCountFileKey] = 1;
        }

		internal static int GetConcussionCount(this GameSave gameSave)
        {
			return gameSave.DataKeyInts.ContainsKey(ConcussionCountFileKey)
				? gameSave.DataKeyInts[ConcussionCountFileKey]
				: 0;
        }
	}
}
