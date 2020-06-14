using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutscenePrologue4")]
	// ReSharper disable once UnusedMember.Global
	class CutscenePrologue4 : LevelObject
	{
		bool hasAwardedMeleeOrb;

		public CutscenePrologue4(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			Scripts.UpdateRelicOrbGetToastToItem(ItemInfo);
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			var orbCollection = Level.GameSave.Inventory.OrbInventory.Inventory;

			if (hasAwardedMeleeOrb || !orbCollection.ContainsKey((int)EInventoryOrbType.Blue))
				return;

			AwardContainedItem();
			hasAwardedMeleeOrb = true;
		}
	}
}
