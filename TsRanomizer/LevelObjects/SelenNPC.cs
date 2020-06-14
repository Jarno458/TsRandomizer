using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.SelenNPC")]
	// ReSharper disable once UnusedMember.Global
	class SelenNpc : LevelObject
	{
		bool hasReplacedSpellPopup;
		bool hasAwardedSpellOrb;

		public SelenNpc(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			if (!hasReplacedSpellPopup && Object._tutorialSection == 2)
			{
				Scripts.UpdateRelicOrbGetToastToItem(ItemInfo);
				hasReplacedSpellPopup = true;
			}
			
			if (hasAwardedSpellOrb)
				return;

			var orbCollection = Level.GameSave.Inventory.OrbInventory.Inventory;
			if (!orbCollection.TryGetValue((int)EInventoryOrbType.Blue, out InventoryOrb orb) || !orb.IsSpellUnlocked)
				return;

			orbCollection[(int)EInventoryOrbType.Blue].IsSpellUnlocked = false;
			AwardContainedItem();
			hasAwardedSpellOrb = true;
		}
	}
}
