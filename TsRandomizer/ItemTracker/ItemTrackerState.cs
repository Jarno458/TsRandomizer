﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;

namespace TsRandomizer.ItemTracker
{
	[Serializable]
	public class ItemTrackerState
	{
		public const int NumberOfItems = 41;

		public bool Timestop;
		public bool TimeSpindle;
		public bool TimeGear1;
		public bool TimeGear2;
		public bool TimeGear3;
		public bool DoubleJump;
		public bool Dash;
		public bool Lightwall;
		public bool CelestialSash;
		public bool FireOrb;
		public bool FireSpell;
		public bool FireRing;
		public bool DinsFire;
		public bool CardA;
		public bool CardB;
		public bool CardC;
		public bool CardD;
		public bool CardE;
		public bool CardV;
		public bool WaterMask;
		public bool GasMask;
		public bool PyramidKeys;
		public bool PinkOrb;
		public bool PinkSpell;
		public bool PinkRing;
		public bool Tablet;
		public bool EyeRing;
		public bool Kobo;
		public bool MerchantCrow;
		public bool PastWarp;
		public bool PresentWarp;
		public bool PyramidWarp;
		public bool LaserA;
		public bool LaserI;
		public bool LaserM;
		public bool LabGenza;
		public bool LabDynamo;
		public bool LabExperiment;
		public bool LabResearch;
		public bool DrawbridgeKey;
		public bool LanternCube;

		internal static ItemTrackerState FromItemLocationMap(IEnumerable<ItemLocation> itemLocations)
		{
			var unlockedProgressionItems = itemLocations
				.Where(l => l.IsPickedUp && l.ItemInfo.IsProgression)
				.Select(l => l.ItemInfo)
				.ToArray();

			var obtainedProgressiveItems = unlockedProgressionItems
				.OfType<ProgressiveItemInfo>();

			var obtainedSingleItems = unlockedProgressionItems
				.Where(i => !(i is ProgressiveItemInfo));

			var obtainedItemIdentifiers = obtainedSingleItems
				.Select(i => i.Identifier)
				.Concat(obtainedProgressiveItems.SelectMany(pi => pi.GetAllUnlockedItems().Select(i => i.Identifier)));

			var trackerState = new ItemTrackerState();

			foreach (var item in obtainedItemIdentifiers)
				SetMemberForItem(trackerState, item);

			return trackerState;
		}

		static readonly Dictionary<ItemIdentifier, Expression<Func<ItemTrackerState, bool>>> ItemToMemberMap = 
			new Dictionary<ItemIdentifier, Expression<Func<ItemTrackerState, bool>>>(NumberOfItems)
		{
			{new ItemIdentifier(EInventoryRelicType.TimespinnerWheel), s => s.Timestop},
			{new ItemIdentifier(EInventoryRelicType.TimespinnerSpindle), s => s.TimeSpindle},
			{new ItemIdentifier(EInventoryRelicType.TimespinnerGear1), s => s.TimeGear1},
			{new ItemIdentifier(EInventoryRelicType.TimespinnerGear2), s => s.TimeGear2},
			{new ItemIdentifier(EInventoryRelicType.TimespinnerGear3), s => s.TimeGear3},
			{new ItemIdentifier(EInventoryRelicType.DoubleJump), s => s.DoubleJump},
			{new ItemIdentifier(EInventoryRelicType.Dash), s => s.Dash},
			{new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell), s => s.Lightwall},
			{new ItemIdentifier(EInventoryRelicType.EssenceOfSpace), s => s.CelestialSash},
			{new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Melee), s => s.FireOrb},
			{new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Spell), s => s.FireSpell},
			{new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Passive), s => s.FireRing},
			{new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell), s => s.DinsFire},
			{new ItemIdentifier(EInventoryRelicType.ScienceKeycardA), s => s.CardA},
			{new ItemIdentifier(EInventoryRelicType.ScienceKeycardB), s => s.CardB},
			{new ItemIdentifier(EInventoryRelicType.ScienceKeycardC), s => s.CardC},
			{new ItemIdentifier(EInventoryRelicType.ScienceKeycardD), s => s.CardD},
			{new ItemIdentifier(EInventoryRelicType.ElevatorKeycard), s => s.CardE},
			{new ItemIdentifier(EInventoryRelicType.ScienceKeycardV), s => s.CardV},
			{new ItemIdentifier(EInventoryRelicType.WaterMask), s => s.WaterMask},
			{new ItemIdentifier(EInventoryRelicType.AirMask), s => s.GasMask},
			{new ItemIdentifier(EInventoryRelicType.PyramidsKey), s => s.PyramidKeys},
			{new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Melee), s => s.PinkOrb},
			{new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Spell), s => s.PinkSpell},
			{new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Passive), s => s.PinkRing},
			{new ItemIdentifier(EInventoryRelicType.Tablet), s => s.Tablet},
			{new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Passive), s => s.EyeRing},
			{new ItemIdentifier(EInventoryFamiliarType.Kobo), s => s.Kobo},
			{new ItemIdentifier(EInventoryFamiliarType.MerchantCrow), s => s.MerchantCrow},
			{CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon), s => s.PastWarp},
			{CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon), s => s.PresentWarp},
			{CustomItem.GetIdentifier(CustomItemType.MysteriousWarpBeacon), s => s.PyramidWarp},
			{CustomItem.GetIdentifier(CustomItemType.LaserAccessA), s => s.LaserA},
			{CustomItem.GetIdentifier(CustomItemType.LaserAccessI), s => s.LaserI},
			{CustomItem.GetIdentifier(CustomItemType.LaserAccessM), s => s.LaserM},
			{CustomItem.GetIdentifier(CustomItemType.LabAccessGenza), s => s.LabGenza},
			{CustomItem.GetIdentifier(CustomItemType.LabAccessDynamo), s => s.LabDynamo},
			{CustomItem.GetIdentifier(CustomItemType.LabAccessExperiment), s => s.LabExperiment},
			{CustomItem.GetIdentifier(CustomItemType.LabAccessResearch), s => s.LabResearch},
			{CustomItem.GetIdentifier(CustomItemType.DrawbridgeKey), s => s.DrawbridgeKey},
			{CustomItem.GetIdentifier(CustomItemType.CubeOfBodie), s => s.LanternCube},
		};

		static void SetMemberForItem(ItemTrackerState trackerState, ItemIdentifier itemInfo)
		{
			if (!ItemToMemberMap.TryGetValue(itemInfo, out var expression))
				return;

			var memberExpression = (MemberExpression)expression.Body;
			var fieldInfo = (FieldInfo)memberExpression.Member;

			fieldInfo.SetValue(trackerState, true);
		}
	}
}
