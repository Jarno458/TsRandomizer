using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

using Microsoft.Xna.Framework;

// TODO revert
using TsRandomizer.Extensions;

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
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.EmpTowerCandelabraLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.ForestLampLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.GyreLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.HangarHangingLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.HangarTowerLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.HangingLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LabCoffeeLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LabTowerLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LakeCandleLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.LakeCoralLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternLibraryHangEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternTowerStandEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternTunnelEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.PinkFruitLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.PrologueLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.SconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.StairwellSconceLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.TempleCandelabraLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.TempleCandleLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.TowerCandelabraLanternEvent")]

	// ReSharper disable once UnusedMember.Global
	class LanternEvent : ItemManipulator
	{
		bool hasAwardedItem;
		Vector4 originalColor;
		int originalRadius;
		public LanternEvent(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation)
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			Dynamic.DoesRegenerate = true;
			Dynamic._isAffectedByTime = false;
			Dynamic._isFrozen = false;
			Dynamic.IsInvulnerable = false;
			originalColor = Dynamic.OrbGlowColor;
			originalRadius = Dynamic._glowRadius;
			if (!hasAwardedItem && ItemInfo != null)
			{
				Dynamic.OrbGlowColor = Color.DarkSeaGreen.ToVector4();
				Dynamic._glowRadius = (int)(originalRadius * 1.5);
			}
		}
		protected override void OnUpdate()
		{
			if (ItemInfo == null ||  hasAwardedItem || !Dynamic.IsDormant)
				return;
			AwardContainedItem();
			ShowItemAwardPopup();
			// OnItemPickup();
			hasAwardedItem = true;
			Dynamic.OrbGlowColor = originalColor;
			Dynamic._glowRadius = originalRadius;
		}
	}
}
