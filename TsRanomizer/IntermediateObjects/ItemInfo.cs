using System;
using System.Collections.Generic;
using System.Reflection;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRanodmizer.Extensions;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.IntermediateObjects
{
	class ItemInfo : IEquatable<ItemInfo>
	{
		static readonly MethodInfo GetIconFromUseItemMethod;
		static readonly MethodInfo GetIconFromOrbMethod;
		static readonly MethodInfo GetIconFromRelicMethod;
		static readonly MethodInfo GetIconFromEnquipmentMethod;
		static readonly MethodInfo GetIconFromFamilierMethod;

		static readonly Dictionary<EInventoryUseItemType, ItemInfo> UseItems;
		static readonly Dictionary<EInventoryRelicType, ItemInfo> RelicItems;
		static readonly Dictionary<EInventoryEquipmentType, ItemInfo> EnquipmentItems;
		static readonly Dictionary<EInventoryFamiliarType, ItemInfo> FamilierItems;
		static readonly Dictionary<int, ItemInfo> OrbItems;

		public static readonly Dictionary<ItemInfo, Requirement> UnlockingMap;
		public static ItemInfo Dummy;
		
		static ItemInfo()
		{
			var inventoryItemType = TimeSpinnerType.Get("Timespinner.GameAbstractions.Inventory.InventoryItem");
			GetIconFromUseItemMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryUseItemType));
			GetIconFromOrbMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryOrbType), typeof(EOrbSlot));
			GetIconFromRelicMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryRelicType));
			GetIconFromEnquipmentMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryEquipmentType));
			GetIconFromFamilierMethod = inventoryItemType.GetPrivateStaticMethod("GetIconFromItem", typeof(EInventoryFamiliarType));

			UseItems = new Dictionary<EInventoryUseItemType, ItemInfo>();
			RelicItems = new Dictionary<EInventoryRelicType, ItemInfo>();
			EnquipmentItems = new Dictionary<EInventoryEquipmentType, ItemInfo>();
			FamilierItems = new Dictionary<EInventoryFamiliarType, ItemInfo>();
			OrbItems = new Dictionary<int, ItemInfo>();
			
			UnlockingMap = new Dictionary<ItemInfo, Requirement>
			{
				{Get(EInventoryRelicType.TimespinnerWheel), Requirement.TimespinnerWheel | Requirement.TimeStop},
				{Get(EInventoryRelicType.DoubleJump), Requirement.DoubleJump | Requirement.TimeStop},
				{Get(EInventoryRelicType.Dash), Requirement.ForwardDash},
				{Get(EInventoryOrbType.Flame, EOrbSlot.Passive), Requirement.AntiWeed},
				{Get(EInventoryOrbType.Flame, EOrbSlot.Melee), Requirement.AntiWeed},
				{Get(EInventoryOrbType.Flame, EOrbSlot.Spell), Requirement.AntiWeed},
				{Get(EInventoryRelicType.ScienceKeycardA), Requirement.CardA | Requirement.CardB | Requirement.CardC | Requirement.CardD},
				{Get(EInventoryRelicType.ScienceKeycardB), Requirement.CardB | Requirement.CardC | Requirement.CardD},
				{Get(EInventoryRelicType.ScienceKeycardC), Requirement.CardC | Requirement.CardD},
				{Get(EInventoryRelicType.ScienceKeycardD), Requirement.CardD},
				{Get(EInventoryRelicType.ElevatorKeycard), Requirement.CardE},
				{Get(EInventoryRelicType.ScienceKeycardV), Requirement.CardV},
				{Get(EInventoryRelicType.WaterMask), Requirement.Swimming},
				{Get(EInventoryRelicType.PyramidsKey), Requirement.None}, //Set in ItemLocationRandomizer.CalculateTeleporterPickAction(),
				{Get(EInventoryRelicType.TimespinnerSpindle), Requirement.TimespinnerSpindle},
				{Get(EInventoryRelicType.TimespinnerGear1), Requirement.TimespinnerPiece1},
				{Get(EInventoryRelicType.TimespinnerGear2), Requirement.TimespinnerPiece2},
				{Get(EInventoryRelicType.TimespinnerGear3), Requirement.TimespinnerPiece3},
				{Get(EInventoryRelicType.EssenceOfSpace), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop},
				{Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), Requirement.UpwardDash | Requirement.DoubleJump | Requirement.TimeStop}
			};

			Dummy = new ItemInfo(EInventoryEquipmentType.DemonHorn);
		}

		public static ItemInfo Get(EInventoryUseItemType useItem)
		{
			return GetOrAdd(UseItems, useItem, () => new ItemInfo(useItem));
		}

		public static ItemInfo Get(EInventoryRelicType relicItem)
		{
			return GetOrAdd(RelicItems, relicItem, () => new ItemInfo(relicItem));
		}

		public static ItemInfo Get(EInventoryEquipmentType equipmentItem)
		{
			return GetOrAdd(EnquipmentItems, equipmentItem, () => new ItemInfo(equipmentItem));
		}

		public static ItemInfo Get(EInventoryFamiliarType familiarItem)
		{
			return GetOrAdd(FamilierItems, familiarItem, () => new ItemInfo(familiarItem));
		}

		public static ItemInfo Get(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return GetOrAdd(OrbItems, GetOrbKey(orbType, orbSlot), () => new ItemInfo(orbType, orbSlot));
		}

		static int GetOrbKey(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			return ((int)orbType * 10) + (int)orbSlot;
		}

		static ItemInfo GetOrAdd<T>(Dictionary<T, ItemInfo> dictionary, T item, Func<ItemInfo> createNew)
		{
			if (dictionary.ContainsKey(item))
				return dictionary[item];

			var newItem = createNew();
			dictionary[item] = newItem;
			return newItem;
		}
		
		public LootType LootType { get; }
		public int ItemId { get; }
		public int ItemSubId { get; }
		public Requirement Unlocks => GetUnlockedRequirements();
		public Enum TreasureLootType => LootType.ToETreasureLootType();
		public EInventoryUseItemType UseItem => (EInventoryUseItemType)ItemId;
		public EInventoryRelicType Relic => (EInventoryRelicType)ItemId;
		public EInventoryEquipmentType Enquipment => (EInventoryEquipmentType)ItemId;
		public EInventoryFamiliarType Familiar => (EInventoryFamiliarType)ItemId;
		public EInventoryOrbType OrbType => (EInventoryOrbType)ItemId;
		public EOrbSlot OrbSlot => (EOrbSlot)ItemSubId;

		public int AnimationIndex => GetAnimationIndex();
		public BestiaryItemDropSpecification BestiaryItemDropSpecification => GetBestiaryItemDropSpecification();

		Action<Level> PickupAction { get; set; }

		ItemInfo(EInventoryUseItemType useItem)
		{
			LootType = LootType.UseItem;
			ItemId = (int)useItem;
		}

		ItemInfo(EInventoryRelicType relicType)
		{
			LootType = LootType.Relic;
			ItemId = (int)relicType;
		}

		ItemInfo(EInventoryEquipmentType enquipment)
		{
			LootType = LootType.Equipment;
			ItemId = (int)enquipment;
		}

		ItemInfo(EInventoryOrbType orbType, EOrbSlot orbSlot)
		{
			LootType = LootType.Orb;
			ItemId = (int)orbType;
			ItemSubId = (int)orbSlot;
		}

		ItemInfo(EInventoryFamiliarType familiar)
		{
			LootType = LootType.Familiar;
			ItemId = (int)familiar;
		}

		public void SetPickupAction(Action<Level> onPickUp)
		{
			PickupAction = onPickUp;
		}

		public void OnPickup(Level level)
		{
			PickupAction?.Invoke(level);
		}

		int GetAnimationIndex()
		{
			switch (LootType)
			{
				case LootType.ConstOrb:
					return (int)GetIconFromOrbMethod.InvokeStatic(OrbType, OrbSlot) -1;
				case LootType.ConstEquipment:
					return (int)GetIconFromEnquipmentMethod.InvokeStatic(Enquipment) - 1;
				case LootType.ConstFamiliar:
					return (int)GetIconFromFamilierMethod.InvokeStatic(Familiar) - 1; 
				case LootType.ConstRelic:
					return (int)GetIconFromRelicMethod.InvokeStatic(Relic) - 1; 
				case LootType.ConstStat:
					return -1;
				case LootType.ConstUseItem:
					return (int)GetIconFromUseItemMethod.InvokeStatic(UseItem) - 1; 
				default:
					throw new ArgumentOutOfRangeException($"LootType {LootType} isnt a valid loot type");
			}
		}

		BestiaryItemDropSpecification GetBestiaryItemDropSpecification()
		{
			return new BestiaryItemDropSpecification
			{
				Category = (int)LootType.ToEInventoryCategoryType(),
				Item = ItemId
			};
		}
		
		Requirement GetUnlockedRequirements()
		{
			if (UnlockingMap.TryGetValue(this, out Requirement requirement))
				return requirement;
			return Requirement.None;
		}

		public bool Equals(ItemInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return LootType.Equals(other.LootType) 
			       && ItemId.Equals(other.ItemId) 
			       && ItemSubId.Equals(other.ItemSubId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ItemInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = LootType.GetHashCode();
				hashCode = (hashCode * 397) ^ ((ItemId << 4) + ItemSubId);
				return hashCode;
			}
		}

		public override string ToString()
		{
			switch (LootType)
			{
				case LootType.ConstEquipment:
					return Enquipment.ToString();
				case LootType.ConstFamiliar:
					return Familiar.ToString();
				case LootType.ConstOrb:
					return $"{OrbSlot}{OrbType}";
				case LootType.ConstRelic:
					return Relic.ToString();
				case LootType.ConstUseItem:
					return UseItems.ToString();
				default:
					throw new NotImplementedException($"Loottype {LootType}.ToString() isnt implemented");
			}
		}
	}
}