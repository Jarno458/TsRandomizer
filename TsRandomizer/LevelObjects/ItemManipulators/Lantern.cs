using Microsoft.Xna.Framework;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;
using TsRandomizer.RoomTriggers;
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
	[TimeSpinnerType("Timespinner.GameObjects.Events.MetropolisLanternEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternLibraryHangEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Lanterns.MetropolisLanternLibraryStandEvent")]
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
			hasAwardedItem = itemLocation.IsPickedUp;
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			Dynamic.DoesRegenerate = true;
			originalColor = Dynamic.OrbGlowColor;
			originalRadius = Dynamic._glowRadius;

			// Cube-blocked
			if (seed.Options.FindTheFlame && !Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.CubeOfBodie)))
			{
				Dynamic._isAffectedByTime = true;
				Dynamic._isFrozen = true;
				Dynamic.IsInvulnerable = true;
				Dynamic.OrbGlowColor = Color.DimGray.ToVector4();
				Dynamic._glowRadius = (int)(originalRadius * 0.5);
			}
			// Collectable
			else if (!hasAwardedItem && ItemInfo != null)
			{
				Dynamic._isAffectedByTime = false;
				Dynamic._isFrozen = false;
				Dynamic.IsInvulnerable = false;
				Dynamic.OrbGlowColor = Color.DarkSeaGreen.ToVector4();
				Dynamic._glowRadius = (int)(originalRadius * 1.5);
			}
		}
		protected override void OnUpdate()
		{
			if (ItemInfo == null ||  hasAwardedItem || !Dynamic.IsDormant)
				return;
			RoomTriggerHelper.SpawnItemDropPickup(Dynamic.Level, ItemInfo, Dynamic.AnchorPosition.X, Dynamic.AnchorPosition.Y);
			hasAwardedItem = true;
			Dynamic.OrbGlowColor = originalColor;
			Dynamic._glowRadius = originalRadius;
			Dynamic._isAffectedByTime = true;
			Dynamic._isFrozen = false;
			Dynamic.IsInvulnerable = false;
		}
	}
	
}
