using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.BlueFruitLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.BrazierLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CandelabraLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CaveBrickCandelabraLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CaveBrickSconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CaveMineLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CursedCaveCandelabraLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CursedCaveMineLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.CursedCaveSconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.ForestLampLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.HangarHangingLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.HangarTowerLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LakeCandleLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LakeCoralLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternLibraryHangEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternTowerStandEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternTunnelEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.PinkFruitLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.SconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.StairwellSconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.TowerCandelabraLanternEvent")]

	// ReSharper disable once UnusedMember.Global
	class LanternEvent : ItemManipulator
	{
		bool hasAwardedItem;

		public LanternEvent(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation)
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}


		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			Dynamic.DoesRegenerate = true;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null ||  hasAwardedItem || !Dynamic.IsDormant)
				return;
			AwardContainedItem();
			ShowItemAwardPopup();
			hasAwardedItem = true;
		}
	}
}
