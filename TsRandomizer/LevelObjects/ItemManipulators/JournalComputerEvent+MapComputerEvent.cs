using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.JournalComputerEvent")]
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.MapComputerEvent")]
	// ReSharper disable once UnusedMember.Global
	class DownloadEvent : ItemManipulator
	{
		bool hasAwardedItem;

		public DownloadEvent(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
			if (ItemInfo == null || hasAwardedItem || ((List<Appendage>)Dynamic.Appendages).Any())
				return;

			RebuildScreen();
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (ItemInfo == null || hasAwardedItem || !Dynamic._isTriggered || !Dynamic._wasActivating)
				return;

			ShowItemAwardPopup();
			AwardContainedItem();

			hasAwardedItem = true;
		}

		void RebuildScreen()
		{
			/*
			this.ChangeAnimation(23);
			this._sparkleParticles = new PassiveBuffSparkleParticleSystem(this._level.GCM.TxParticleEnergy, 10);
			this._particleSystems.Add((ParticleSystem)this._sparkleParticles);
			Appendage appendage = new Appendage((Animate)this, new Point(10, 17), Point.Zero, this._level, this._sprite);
			appendage.FollowType = EAppendageFollowType.AnchorLocked;
			appendage.AnchorOffset = new Point(0, -25);
			appendage.DrawPriority = 1;
			appendage.IsGlowing = true;
			appendage.GlowBase = 3.5f;
			appendage.GlowColor = new Color(0.8f, 0.85f, 0.9f, 0.6f);
			this._screenAppendage = appendage;
			this._screenAppendage.ChangeAnimation(24);
			this.Appendages.Add(this._screenAppendage);
			this._glowTexture = new GlowTexture(this._level)
			*/
		}
	}
}
