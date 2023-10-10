using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;

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
			{ EInventoryUseItemType.WarpCard,       new Dictionary<Trait, float> {{ Trait.Consumable, 1f }, { Trait.Speed, 1f }} },
			{ EInventoryUseItemType.FamiliarTreat,  new Dictionary<Trait, float> {{ Trait.Consumable, 1f }, { Trait.Food, 0.1f } } },
		};

		static readonly Dictionary<EInventoryEquipmentType, Dictionary<Trait, float>> ValuesPerEquipmentItem = new Dictionary<EInventoryEquipmentType, Dictionary<Trait, float>> {
			//Trinket
			{ EInventoryEquipmentType.FamiliarEgg, new Dictionary<Trait, float> {{ Trait.Egg, 1f } } },

			//Armor
			{ EInventoryEquipmentType.EternalCoat, new Dictionary<Trait, float> {{ Trait.Armor, 4.625f } } }, //37
			{ EInventoryEquipmentType.LachiemCrown, new Dictionary<Trait, float> {{ Trait.Armor, 3f } } }, //24
			{ EInventoryEquipmentType.VileteDress, new Dictionary<Trait, float> {{ Trait.Armor, 2.875f } } }, //23
			{ EInventoryEquipmentType.EternalTiara, new Dictionary<Trait, float> {{ Trait.Armor, 2.625f } } }, //21
			{ EInventoryEquipmentType.EmpressCoat, new Dictionary<Trait, float> {{ Trait.Armor, 2.625f } } }, //21
			{ EInventoryEquipmentType.CaptainsCap, new Dictionary<Trait, float> {{ Trait.Armor, 2.125f } } }, //17
			{ EInventoryEquipmentType.CaptainsJacket, new Dictionary<Trait, float> {{ Trait.Armor, 2.125f } } }, //17
			{ EInventoryEquipmentType.LabCoat, new Dictionary<Trait, float> {{ Trait.Armor, 2.125f } } }, //17
			{ EInventoryEquipmentType.MilitaryArmor, new Dictionary<Trait, float> {{ Trait.Armor, 2f } } }, //16
			{ EInventoryEquipmentType.LabGlasses, new Dictionary<Trait, float> {{ Trait.Armor, 1.875f } } }, //15
			{ EInventoryEquipmentType.VileteCrown, new Dictionary<Trait, float> {{ Trait.Armor, 1.875f } } }, //15
			{ EInventoryEquipmentType.LibrarianRobe, new Dictionary<Trait, float> {{ Trait.Armor, 1.75f } } }, //14
			{ EInventoryEquipmentType.LibrarianHat, new Dictionary<Trait, float> {{ Trait.Armor, 1.5f } } }, //12
			{ EInventoryEquipmentType.AdvisorRobe, new Dictionary<Trait, float> {{ Trait.Armor, 1.375f } } }, //11
			{ EInventoryEquipmentType.CalvaryArmor, new Dictionary<Trait, float> {{ Trait.Armor, 1.375f } } }, //11
			{ EInventoryEquipmentType.MidnightCloak, new Dictionary<Trait, float> {{ Trait.Armor, 1.375f } } }, //11
			{ EInventoryEquipmentType.CombatHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 1.125f } } }, //9
			{ EInventoryEquipmentType.AdvisorHat, new Dictionary<Trait, float> {{ Trait.Armor, 1f } } }, //8
			{ EInventoryEquipmentType.TravelersCloak, new Dictionary<Trait, float> {{ Trait.Armor, 1f } } }, //8
			{ EInventoryEquipmentType.BuckleHat, new Dictionary<Trait, float> {{ Trait.Armor, 0.875f } } }, //7
			{ EInventoryEquipmentType.PointyHat, new Dictionary<Trait, float> {{ Trait.Armor, 0.75f } } }, //6
			{ EInventoryEquipmentType.CopperArmor, new Dictionary<Trait, float> {{ Trait.Armor, 0.75f } } }, //6
			{ EInventoryEquipmentType.CalvaryHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.625f } } }, //5
			{ EInventoryEquipmentType.CopperHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.5f } } }, //4
            { EInventoryEquipmentType.EngineerGoggles, new Dictionary<Trait, float> {{ Trait.Armor, 0.5f } } }, //4
			{ EInventoryEquipmentType.LeatherArmor, new Dictionary<Trait, float> {{ Trait.Armor, 0.5f } } }, //4
			{ EInventoryEquipmentType.LeatherHelmet, new Dictionary<Trait, float> {{ Trait.Armor, 0.375f } } }, //3
			{ EInventoryEquipmentType.SecurityVest, new Dictionary<Trait, float> {{ Trait.Armor, 0.375f } } }, //3
			{ EInventoryEquipmentType.SecurityVisor, new Dictionary<Trait, float> {{ Trait.Armor, 0.25f } } }, //2
			{ EInventoryEquipmentType.TrendyJacket, new Dictionary<Trait, float> {{ Trait.Armor, 0.28f } } }, //2
			{ EInventoryEquipmentType.Sunglasses, new Dictionary<Trait, float> {{ Trait.Armor, 0.125f } } }, //1
			{ EInventoryEquipmentType.OldCoat, new Dictionary<Trait, float> {{ Trait.Armor, 0.125f } } }, //1
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
						throw new ArgumentOutOfRangeException(nameof(item), "paramter should be either UseItem or Equipment");
				}
			}
		}
		public Dictionary<Trait, float> this[EInventoryUseItemType useItem] => ValuesPerUseItem[useItem];
		public Dictionary<Trait, float> this[EInventoryEquipmentType useItem] => ValuesPerEquipmentItem[useItem];

		public bool TryGetValue(EInventoryUseItemType item, out Dictionary<Trait, float> traits) => ValuesPerUseItem.TryGetValue(item, out traits);
		public bool TryGetValue(EInventoryEquipmentType item, out Dictionary<Trait, float> traits) => ValuesPerEquipmentItem.TryGetValue(item, out traits);
	}
}