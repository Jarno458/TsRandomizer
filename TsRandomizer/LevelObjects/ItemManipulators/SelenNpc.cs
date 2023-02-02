using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.SelenNPC")]
	// ReSharper disable once UnusedMember.Global
	class SelenNpc : ItemManipulator
	{
		bool hasReplacedSpellPopup;
		bool hasAwardedSpellOrb;

		public SelenNpc(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null)
				return;

			if (!hasReplacedSpellPopup && Dynamic._tutorialSection == 2)
			{
				UpdateRelicOrbGetToastToItem();
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
