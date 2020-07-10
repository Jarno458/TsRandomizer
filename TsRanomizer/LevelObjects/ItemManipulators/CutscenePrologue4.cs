using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutscenePrologue4")]
	// ReSharper disable once UnusedMember.Global
	class CutscenePrologue4 : ItemManipulator
	{
		bool hasAwardedMeleeOrb;

		public CutscenePrologue4(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
		}

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			Scripts.UpdateRelicOrbGetToastToItem(Level, ItemInfo);
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
