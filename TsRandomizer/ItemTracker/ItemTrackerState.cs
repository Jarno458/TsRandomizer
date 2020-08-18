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

		static readonly Dictionary<ItemInfo, Expression<Func<ItemTrackerState, bool>>> ItemToMemberMap = new Dictionary<ItemInfo, Expression<Func<ItemTrackerState, bool>>>
		{
			{ItemInfo.Get(EInventoryRelicType.TimespinnerWheel), s => s.Timestop},
			{ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle), s => s.TimeSpindle},
			{ItemInfo.Get(EInventoryRelicType.TimespinnerGear1), s => s.TimeGear1},
			{ItemInfo.Get(EInventoryRelicType.TimespinnerGear2), s => s.TimeGear2},
			{ItemInfo.Get(EInventoryRelicType.TimespinnerGear3), s => s.TimeGear3},
			{ItemInfo.Get(EInventoryRelicType.DoubleJump), s => s.DoubleJump},
			{ItemInfo.Get(EInventoryRelicType.Dash), s => s.Dash},
			{ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell), s => s.Lightwall},
			{ItemInfo.Get(EInventoryRelicType.EssenceOfSpace), s => s.CelestialSash},
			{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee), s => s.FireOrb},
			{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell), s => s.FireSpell},
			{ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive), s => s.FireRing},
			{ItemInfo.Get(EInventoryOrbType.Book, EOrbSlot.Spell), s => s.DinsFire},
			{ItemInfo.Get(EInventoryRelicType.ScienceKeycardA), s => s.CardA},
			{ItemInfo.Get(EInventoryRelicType.ScienceKeycardB), s => s.CardB},
			{ItemInfo.Get(EInventoryRelicType.ScienceKeycardC), s => s.CardC},
			{ItemInfo.Get(EInventoryRelicType.ScienceKeycardD), s => s.CardD},
			{ItemInfo.Get(EInventoryRelicType.ElevatorKeycard), s => s.CardE},
			{ItemInfo.Get(EInventoryRelicType.ScienceKeycardV), s => s.CardV},
			{ItemInfo.Get(EInventoryRelicType.WaterMask), s => s.WaterMask},
			{ItemInfo.Get(EInventoryRelicType.AirMask), s => s.GassMask},
			{ItemInfo.Get(EInventoryRelicType.PyramidsKey), s => s.PyramidKeys},
			{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee), s => s.PinkOrb},
			{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Spell), s => s.PinkSpell},
			{ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Passive), s => s.PinkRing},
		};

		static void SetMemberForItem(ItemTrackerState trackerState, ItemInfo itemInfo)
		{
			if (!ItemToMemberMap.TryGetValue(itemInfo, out Expression<Func<ItemTrackerState, bool>> expression))
				return;

			var memberExpression = (MemberExpression)expression.Body;
			var fieldInfo = (FieldInfo) memberExpression.Member;

			fieldInfo.SetValue(trackerState, true);
		}
	}
}