using System;
using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Archipelago.Gifting
{
	class TraitMapping
	{
		public static readonly TraitMapping ValuesPerItem = new TraitMapping();

		static readonly Dictionary<EInventoryUseItemType, Dictionary<Trait, float>> ValuesPerUseItem = new Dictionary<EInventoryUseItemType, Dictionary<Trait, float>> {
			// HP
			{ EInventoryUseItemType.EmpressCake,    new Dictionary<Trait, float> {{ Trait.Heal, 2f    }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Fruit, 0.2f } } }, //400
			{ EInventoryUseItemType.Casserole,      new Dictionary<Trait, float> {{ Trait.Heal, 1.5f  }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 0.5f }, { Trait.Vegetable, 0.5f } } }, //300
			{ EInventoryUseItemType.Spaghetti,      new Dictionary<Trait, float> {{ Trait.Heal, 1.5f  }, { Trait.Consumable, 1f }, { Trait.Food, 1.5f }, { Trait.Vegetable, 0.3f } } }, //300
			{ EInventoryUseItemType.HiPotion,       new Dictionary<Trait, float> {{ Trait.Heal, 1.25f }, { Trait.Consumable, 1f }, { Trait.Drink, 1f } } }, //250
			{ EInventoryUseItemType.CheveuxAuVin,   new Dictionary<Trait, float> {{ Trait.Heal, 1.25f }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 0.7f }, { Trait.Vegetable, 0.3f }} }, //250
			{ EInventoryUseItemType.LachiemiSun,    new Dictionary<Trait, float> {{ Trait.Heal, 1f    }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Fruit, 1f } } }, //200
			{ EInventoryUseItemType.UnagiRoll,      new Dictionary<Trait, float> {{ Trait.Heal, 1f    }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Fish, 0.5f }, { Trait.Vegetable, 0.5f } } }, //200
			{ EInventoryUseItemType.FutureHiPotion, new Dictionary<Trait, float> {{ Trait.Heal, 0.75f }, { Trait.Consumable, 1f }, { Trait.Food, 1f } } }, //150
			{ EInventoryUseItemType.SauteedTail,    new Dictionary<Trait, float> {{ Trait.Heal, 0.75f }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 1.1f } } }, //150
			{ EInventoryUseItemType.FriedCheveux,   new Dictionary<Trait, float> {{ Trait.Heal, 0.5f  }, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 1f } } }, //100
			{ EInventoryUseItemType.Biscuit,        new Dictionary<Trait, float> {{ Trait.Heal, 0.4f  }, { Trait.Consumable, 1f }, { Trait.Food, 1f } } }, //80
			{ EInventoryUseItemType.Potion,         new Dictionary<Trait, float> {{ Trait.Heal, 0.375f}, { Trait.Consumable, 1f }, { Trait.Drink, 1f } } }, //75
			{ EInventoryUseItemType.FuturePotion,   new Dictionary<Trait, float> {{ Trait.Heal, 0.25f }, { Trait.Consumable, 1f }, { Trait.Food, 1f } } }, //50
			{ EInventoryUseItemType.Jerky,          new Dictionary<Trait, float> {{ Trait.Heal, 0.195f}, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 1f } } }, //39
			{ EInventoryUseItemType.PlumpMaggot,    new Dictionary<Trait, float> {{ Trait.Heal, 0.125f}, { Trait.Consumable, 1f }, { Trait.Food, 1f }, { Trait.Meat, 0.5f } } }, //25
			//Mana
			{ EInventoryUseItemType.HiEther,        new Dictionary<Trait, float> {{ Trait.Mana, 1.5f  }, { Trait.Consumable, 1f }, { Trait.Drink, 1.5f } } }, //150
			{ EInventoryUseItemType.FutureHiEther,  new Dictionary<Trait, float> {{ Trait.Mana, 1f    }, { Trait.Consumable, 1f } } }, //100
			{ EInventoryUseItemType.FiligreeTea,    new Dictionary<Trait, float> {{ Trait.Heal, 1f    }, { Trait.Consumable, 1f }, { Trait.Drink, 1f } } }, //100
			{ EInventoryUseItemType.Ether,          new Dictionary<Trait, float> {{ Trait.Mana, 0.75f }, { Trait.Consumable, 1f }, { Trait.Drink, 1f } } }, //75
			{ EInventoryUseItemType.OrangeJuice,    new Dictionary<Trait, float> {{ Trait.Heal, 0.35f }, { Trait.Consumable, 1f }, { Trait.Drink, 1f }, { Trait.Fruit, 1f } } }, //35
			{ EInventoryUseItemType.FutureEther,    new Dictionary<Trait, float> {{ Trait.Mana, 0.3f  }, { Trait.Consumable, 1f } } }, //30
			//Sand
			{ EInventoryUseItemType.HiSandBottle,   new Dictionary<Trait, float> {{ Trait.Resource, 1f  }, { Trait.Consumable, 1.5f } } }, //100
			{ EInventoryUseItemType.SandBottle,     new Dictionary<Trait, float> {{ Trait.Resource, 0.5f}, { Trait.Consumable, 1f } } }, //50
			//Cure
			{ EInventoryUseItemType.Antidote,       new Dictionary<Trait, float> {{ Trait.Cure, 1f   }, { Trait.Consumable, 1f }, { Trait.Drink, 1f } } },
			{ EInventoryUseItemType.ChaosHeal,      new Dictionary<Trait, float> {{ Trait.Cure, 0.9f }, { Trait.Consumable, 1f }, { Trait.Flower, 1f } } },
			//Others
			{ EInventoryUseItemType.WarpCard,       new Dictionary<Trait, float> {{ Trait.Consumable, 1f }, { Trait.Speed, 1f } } },
			{ EInventoryUseItemType.FamiliarTreat,  new Dictionary<Trait, float> {{ Trait.Consumable, 1f }, { Trait.Food, 0.1f } } },
			{ EInventoryUseItemType.Herb,           new Dictionary<Trait, float> {{ Trait.Fiber, 0.1f } } },

			{ EInventoryUseItemType.Mushroom,       new Dictionary<Trait, float> {{ Trait.Vegetable, 0.2f } } },
			{ EInventoryUseItemType.EelMeat,        new Dictionary<Trait, float> {{ Trait.Fish, 0.2f } } },
			{ EInventoryUseItemType.CheveuxBreast,  new Dictionary<Trait, float> {{ Trait.Meat, 0.25f } } },
			{ EInventoryUseItemType.Drumstick,      new Dictionary<Trait, float> {{ Trait.Meat, 0.2f } } },
			{ EInventoryUseItemType.WyvernTail,     new Dictionary<Trait, float> {{ Trait.Meat, 0.15f } } },

			//{ EInventoryUseItemType.RottenTail,     new Dictionary<Trait, float> {{ Trait.Damage/Trap, 0.7f } } },

			//To odd to give
			//AlchemistTools
			//GalaxyStone
			//MagicMarbles
			//RadiationCrystal
			//PlasmaIV
			//FoodSynth
			//CheveuxFeather
			//SirenInk
			//PlasmaCore
			//SilverOre
		};

		static readonly Dictionary<EInventoryEquipmentType, Dictionary<Trait, float>> ValuesPerEquipmentItem = new Dictionary<EInventoryEquipmentType, Dictionary<Trait, float>> {
			//Specail Trinket
			{ EInventoryEquipmentType.FamiliarEgg, new Dictionary<Trait, float> {{ Trait.Egg, 1f } } },
			{ EInventoryEquipmentType.BirdStatue, new Dictionary<Trait, float> {{ Trait.Cure, 1.9f } } },
			{ EInventoryEquipmentType.Pendulum, new Dictionary<Trait, float> {{ Trait.Cure, 2f } } },
			{ EInventoryEquipmentType.GlassPumpkin, new Dictionary<Trait, float> {{ Trait.Resource, 2f } } },
			
			//To odd to give
			//{ EInventoryEquipmentType.ShinyRock, new Dictionary<Trait, float> {{ , 1f } } }, 
			//{ EInventoryEquipmentType.SelenBangle, new Dictionary<Trait, float> {{ , 1f } } },

			//Armor (armorvalue / 8)
			{ EInventoryEquipmentType.EternalCoat, new Dictionary<Trait, float> {{ Trait.Armor, 4.5f } } }, //37
			{ EInventoryEquipmentType.LachiemCrown, new Dictionary<Trait, float> {{ Trait.Armor, 3f } } }, //24
			{ EInventoryEquipmentType.VileteDress, new Dictionary<Trait, float> {{ Trait.Armor, 2.8f } } }, //23
			{ EInventoryEquipmentType.EternalTiara, new Dictionary<Trait, float> {{ Trait.Armor, 2.55f } } }, //21
			{ EInventoryEquipmentType.EmpressCoat, new Dictionary<Trait, float> {{ Trait.Armor, 2.5f } } }, //21
			{ EInventoryEquipmentType.CaptainsCap, new Dictionary<Trait, float> {{ Trait.Armor, 2.2f } } }, //17
			{ EInventoryEquipmentType.CaptainsJacket, new Dictionary<Trait, float> {{ Trait.Armor, 2.1f } } }, //17
			{ EInventoryEquipmentType.LabCoat, new Dictionary<Trait, float> {{ Trait.Armor, 2f } } }, //17
			{ EInventoryEquipmentType.MilitaryArmor, new Dictionary<Trait, float> {{ Trait.Armor, 1.9f } } }, //16
			{ EInventoryEquipmentType.LabGlasses, new Dictionary<Trait, float> {{ Trait.Armor, 1.85f } } }, //15
			{ EInventoryEquipmentType.VileteCrown, new Dictionary<Trait, float> {{ Trait.Armor, 1.80f } } }, //15
			{ EInventoryEquipmentType.LibrarianRobe, new Dictionary<Trait, float> {{ Trait.Armor, 1.75f } } }, //14
			{ EInventoryEquipmentType.LibrarianHat, new Dictionary<Trait, float> {{ Trait.Armor, 1.5f } } }, //12
			{ EInventoryEquipmentType.AdvisorRobe, new Dictionary<Trait, float> {{ Trait.Armor, 1.4f } } }, //11
			{ EInventoryEquipmentType.CalvaryArmor, new Dictionary<Trait, float> {{ Trait.Armor, 1.35f } } }, //11
			{ EInventoryEquipmentType.MidnightCloak, new Dictionary<Trait, float> {{ Trait.Armor, 1.3f } } }, //11
			{ EInventoryEquipmentType.AzureStole, new Dictionary<Trait, float> {{ Trait.Armor, 1.25f } } }, //10
			{ EInventoryEquipmentType.FiligreeClasp, new Dictionary<Trait, float> {{ Trait.Armor, 1.2f } } }, //10
			{ EInventoryEquipmentType.CombatHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 1.125f } } }, //9
			{ EInventoryEquipmentType.AdvisorHat, new Dictionary<Trait, float> {{ Trait.Armor, 1.1f } } }, //8
			{ EInventoryEquipmentType.TravelersCloak, new Dictionary<Trait, float> {{ Trait.Armor, 1f } } }, //8
			{ EInventoryEquipmentType.DemonStole, new Dictionary<Trait, float> {{ Trait.Armor, 0.9f } } }, //7
			{ EInventoryEquipmentType.BuckleHat, new Dictionary<Trait, float> {{ Trait.Armor, 0.85f } } }, //7
			{ EInventoryEquipmentType.MotherOfPearl, new Dictionary<Trait, float> {{ Trait.Armor, 0.8f } } }, //6
			{ EInventoryEquipmentType.DemonHorn, new Dictionary<Trait, float> {{ Trait.Armor, 0.75f } } }, //6
			{ EInventoryEquipmentType.PointyHat, new Dictionary<Trait, float> {{ Trait.Armor, 0.70f } } }, //6
			{ EInventoryEquipmentType.CopperArmor, new Dictionary<Trait, float> {{ Trait.Armor, 0.65f } } }, //6
			{ EInventoryEquipmentType.CalvaryHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.625f } } }, //5
			{ EInventoryEquipmentType.LuckyCoin, new Dictionary<Trait, float> {{ Trait.Armor, 0.6f } } }, //5
			{ EInventoryEquipmentType.SirenHairband, new Dictionary<Trait, float> {{ Trait.Armor, 0.51f } } }, //4
			{ EInventoryEquipmentType.CopperHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.5f } } }, //4
            { EInventoryEquipmentType.EngineerGoggles, new Dictionary<Trait, float> {{ Trait.Armor, 0.49f } } }, //4
			{ EInventoryEquipmentType.LeatherArmor, new Dictionary<Trait, float> {{ Trait.Armor, 0.48f } } }, //4
			{ EInventoryEquipmentType.LeatherHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.4f } } }, //3
			{ EInventoryEquipmentType.SecurityVest, new Dictionary<Trait, float> {{ Trait.Armor, 0.35f } } }, //3
			{ EInventoryEquipmentType.CheveuxPlume, new Dictionary<Trait, float> {{ Trait.Armor, 0.3f } } }, //2
			{ EInventoryEquipmentType.SecurityVisor, new Dictionary<Trait, float> {{ Trait.Armor, 0.25f } } }, //2
			{ EInventoryEquipmentType.TrendyJacket, new Dictionary<Trait, float> {{ Trait.Armor, 0.2f } } }, //2
			{ EInventoryEquipmentType.MetalWristband, new Dictionary<Trait, float> {{ Trait.Armor, 0.13f } } }, //1
			{ EInventoryEquipmentType.Sunglasses, new Dictionary<Trait, float> {{ Trait.Armor, 0.125f } } }, //1
			{ EInventoryEquipmentType.SyntheticPlume, new Dictionary<Trait, float> {{ Trait.Armor, 0.12f } } }, //1
			{ EInventoryEquipmentType.OldCoat, new Dictionary<Trait, float> {{ Trait.Armor, 0.1f } } }, //1
		};

		public Dictionary<Trait, float> this[InventoryItem item] {
			get {
				switch (item)
				{
					case InventoryUseItem useItem:
						return this[useItem.UseItemType];
					case InventoryEquipment equipment:
						return this[equipment.EquipmentType];
					default:
						throw new ArgumentOutOfRangeException(nameof(item), "parameter should be either UseItem or Equipment");
				}
			}
		}
		public Dictionary<Trait, float> this[EInventoryUseItemType useItem] => ValuesPerUseItem[useItem];
		public Dictionary<Trait, float> this[EInventoryEquipmentType useItem] => ValuesPerEquipmentItem[useItem];

		public bool TryGetValue(EInventoryUseItemType item, out Dictionary<Trait, float> traits) => ValuesPerUseItem.TryGetValue(item, out traits);
		public bool TryGetValue(EInventoryEquipmentType item, out Dictionary<Trait, float> traits) => ValuesPerEquipmentItem.TryGetValue(item, out traits);

		static readonly Dictionary<string, InventoryItem> ItemNameToInventoryItemCache = new Dictionary<string, InventoryItem>();

		public static InventoryItem ParseItem(string name, Dictionary<Trait, float> traits, int amount)
		{
			if (!ItemNameToInventoryItemCache.Any())
				InitializeItemNameToInventoryItemCache();

			if (ItemNameToInventoryItemCache.TryGetValue(name, out var item))
				return AddAmountToItem(item, amount);

			if (traits.ContainsKey(Trait.Speed))
				return new InventoryUseItem(EInventoryUseItemType.WarpCard) { Count = amount };
			if (traits.ContainsKey(Trait.Flower))
				return new InventoryUseItem(EInventoryUseItemType.ChaosHeal) { Count = amount };
			if (traits.ContainsKey(Trait.Cure))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Cure, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Fish))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Fish, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Egg))
				return new InventoryEquipment(EInventoryEquipmentType.FamiliarEgg) { Count = amount };
			if (traits.ContainsKey(Trait.Fiber))
				return new InventoryUseItem(EInventoryUseItemType.Herb) { Count = amount };
			if (traits.ContainsKey(Trait.Armor))
				return new InventoryEquipment(FindClosestMatchForArmorTrait(Trait.Armor, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Mana))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Mana, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Mana))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Mana, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Heal))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Heal, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Resource))
				return new InventoryUseItem(FindClosestMatchForTrait(Trait.Resource, traits)) { Count = amount };
			if (traits.ContainsKey(Trait.Fruit) && traits.ContainsKey(Trait.Drink))
				return new InventoryUseItem(EInventoryUseItemType.OrangeJuice) { Count = amount };

			return new InventoryUseItem(FindClosestMatch(traits)) { Count = amount };
		}
		
		static void InitializeItemNameToInventoryItemCache()
		{
			foreach (var itemType in ValuesPerUseItem.Keys)
			{
				var identifier = new ItemIdentifier(itemType);
				var name = Client.ItemsHelper.GetItemName(ItemMap.GetItemId(identifier));

				ItemNameToInventoryItemCache.Add(name, new InventoryUseItem(identifier.UseItem));
			}

			foreach (var itemType in ValuesPerEquipmentItem.Keys)
			{
				var identifier = new ItemIdentifier(itemType);
				var name = Client.ItemsHelper.GetItemName(ItemMap.GetItemId(identifier));

				ItemNameToInventoryItemCache.Add(name, new InventoryEquipment(identifier.Equipment));
			}
		}

		static InventoryItem AddAmountToItem(InventoryItem item, int amount)
		{
			switch (item)
			{
				case InventoryUseItem useItem:
					useItem.Count = amount;
					return useItem;
				case InventoryEquipment equipment:
					equipment.Count = amount;
					return equipment;
				default:
					throw new ArgumentOutOfRangeException(nameof(item), "parameter should be either UseItem or Equipment");
			}
		}
		
		static EInventoryUseItemType FindClosestMatch(Dictionary<Trait, float> traits)
		{
			var itemTypesWithMatchCount = new Dictionary<EInventoryUseItemType, int>();
			var mostMatches = 0;

			foreach (var useItemTraitMapping in ValuesPerUseItem)
			{
				var matches = 0;

				foreach (var trait in traits.Keys)
					if (useItemTraitMapping.Value.ContainsKey(trait))
						matches++;

				if (matches >= mostMatches)
					itemTypesWithMatchCount.Add(useItemTraitMapping.Key, matches);
			}

			var mostMatchedItemTypes = itemTypesWithMatchCount
				.Where(kvp => kvp.Value == mostMatches)
				.Select(kvp => kvp.Key);

			var closestMatch = 0f;
			var closestItemType = EInventoryUseItemType.None;
			
			foreach (var itemType in mostMatchedItemTypes)
			{
				var diff = 0f;
				foreach (var trait in traits)
					if (ValuesPerUseItem[itemType].ContainsKey(trait.Key))
						diff += Math.Abs(trait.Value - ValuesPerUseItem[itemType][trait.Key]);

				if (closestItemType == EInventoryUseItemType.None || diff < closestMatch)
				{
					closestMatch = diff;
					closestItemType = itemType;
				}
			}

			return closestItemType;
		}

		static EInventoryUseItemType FindClosestMatchForTrait(Trait trait, Dictionary<Trait, float> traits)
		{
			var itemValue = traits[trait];
			var closestMatch = 0f; 
			var closestItemType = EInventoryUseItemType.None;

			foreach (var useItemTraitMapping in ValuesPerUseItem.Where(i => i.Value.ContainsKey(trait)))
			{
				var diff = Math.Abs(itemValue - useItemTraitMapping.Value[trait]);

				if (closestItemType == EInventoryUseItemType.None || diff < closestMatch)
				{
					closestMatch = diff;
					closestItemType = useItemTraitMapping.Key;
				}
			}

			return closestItemType;
		}

		static EInventoryEquipmentType FindClosestMatchForArmorTrait(Trait trait, Dictionary<Trait, float> traits)
		{
			var itemValue = traits[trait];
			var closestMatch = 0f;
			var closestItemType = EInventoryEquipmentType.None;

			foreach (var useItemTraitMapping in ValuesPerEquipmentItem.Where(i => i.Value.ContainsKey(trait)))
			{
				var diff = Math.Abs(itemValue - useItemTraitMapping.Value[trait]);

				if (closestItemType == EInventoryEquipmentType.None || diff < closestMatch)
				{
					closestMatch = diff;
					closestItemType = useItemTraitMapping.Key;
				}
			}

			return closestItemType;
		}
	}
}