using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.ItemTracker
{
	[Serializable]
	public class ItemTrackerState
	{
		public const int NumberOfItems = 25;

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
		public bool GassMask;
		public bool PyramidKeys;
		public bool PinkOrb;
		public bool PinkSpell;
		public bool PinkRing;

		internal static ItemTrackerState FromItemLocationMap(IEnumerable<ItemLocation> itemLocations)
		{
			var obtainedProgressionItems = itemLocations
				.Where(l => l.IsPickedUp && l.Unlocks != Requirement.None)
				.Select(l => l.ItemInfo);

			var trackerState = new ItemTrackerState();

			foreach (var item in obtainedProgressionItems)
				SetMemberForItem(trackerState, item);

			return trackerState;
		}

		static readonly Dictionary<ItemInfo, Expression<Func<ItemTrackerState, bool>>> ItemToMemberMap = new Dictionary<ItemInfo, Expression<Func<ItemTrackerState, bool>>>(NumberOfItems)
		{
			{new SingleItemInfo(EInventoryRelicType.TimespinnerWheel), s => s.Timestop},
			{new SingleItemInfo(EInventoryRelicType.TimespinnerSpindle), s => s.TimeSpindle},
			{new SingleItemInfo(EInventoryRelicType.TimespinnerGear1), s => s.TimeGear1},
			{new SingleItemInfo(EInventoryRelicType.TimespinnerGear2), s => s.TimeGear2},
			{new SingleItemInfo(EInventoryRelicType.TimespinnerGear3), s => s.TimeGear3},
			{new SingleItemInfo(EInventoryRelicType.DoubleJump), s => s.DoubleJump},
			{new SingleItemInfo(EInventoryRelicType.Dash), s => s.Dash},
			{new SingleItemInfo(EInventoryOrbType.Barrier, EOrbSlot.Spell), s => s.Lightwall},
			{new SingleItemInfo(EInventoryRelicType.EssenceOfSpace), s => s.CelestialSash},
			{new SingleItemInfo(EInventoryOrbType.Flame, EOrbSlot.Melee), s => s.FireOrb},
			{new SingleItemInfo(EInventoryOrbType.Flame, EOrbSlot.Spell), s => s.FireSpell},
			{new SingleItemInfo(EInventoryOrbType.Flame, EOrbSlot.Passive), s => s.FireRing},
			{new SingleItemInfo(EInventoryOrbType.Book, EOrbSlot.Spell), s => s.DinsFire},
			{new SingleItemInfo(EInventoryRelicType.ScienceKeycardA), s => s.CardA},
			{new SingleItemInfo(EInventoryRelicType.ScienceKeycardB), s => s.CardB},
			{new SingleItemInfo(EInventoryRelicType.ScienceKeycardC), s => s.CardC},
			{new SingleItemInfo(EInventoryRelicType.ScienceKeycardD), s => s.CardD},
			{new SingleItemInfo(EInventoryRelicType.ElevatorKeycard), s => s.CardE},
			{new SingleItemInfo(EInventoryRelicType.ScienceKeycardV), s => s.CardV},
			{new SingleItemInfo(EInventoryRelicType.WaterMask), s => s.WaterMask},
			{new SingleItemInfo(EInventoryRelicType.AirMask), s => s.GassMask},
			{new SingleItemInfo(EInventoryRelicType.PyramidsKey), s => s.PyramidKeys},
			{new SingleItemInfo(EInventoryOrbType.Pink, EOrbSlot.Melee), s => s.PinkOrb},
			{new SingleItemInfo(EInventoryOrbType.Pink, EOrbSlot.Spell), s => s.PinkSpell},
			{new SingleItemInfo(EInventoryOrbType.Pink, EOrbSlot.Passive), s => s.PinkRing},
		};

		static void SetMemberForItem(ItemTrackerState trackerState, ItemInfo itemInfo)
		{
			//TODD check if its stil matches
			if (!ItemToMemberMap.TryGetValue(itemInfo, out Expression<Func<ItemTrackerState, bool>> expression))
				return;

			var memberExpression = (MemberExpression)expression.Body;
			var fieldInfo = (FieldInfo)memberExpression.Member;

			fieldInfo.SetValue(trackerState, true);
		}
	}
}