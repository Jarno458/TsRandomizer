using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Base;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
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
		static readonly Type GlowTextureType = TimeSpinnerType.Get("Timespinner.GameObjects.Animations.GlowTexture");
	
		bool hasAwardedItem;

		public DownloadEvent(Mobile typedObject, ItemLocation itemLocation) : base(typedObject, itemLocation)
		{
			if (ItemInfo == null || IsPickedUp || hasAwardedItem || ((List<Appendage>)Dynamic.Appendages).Any())
				return;

			RebuildScreen();

			Dynamic._doesPlayerHaveTablet = Level.GameSave.HasRelic(EInventoryRelicType.Tablet);
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
			Dynamic.ChangeAnimation(23);

			var particles = new PassiveBuffSparkleParticleSystem(Level.GCM.TxParticleEnergy, 10);

			Dynamic._sparkleParticles = particles;
			((List<ParticleSystem>)Dynamic._particleSystems).Add(particles);

			var appendage = new Appendage((Animate)TypedObject, new Point(10, 17), Point.Zero, Level, Dynamic._sprite)
			{
				FollowType = EAppendageFollowType.AnchorLocked,
				AnchorOffset = new Point(0, -25),
				DrawPriority = 1,
				IsGlowing = true,
				GlowBase = 3.5f,
			};
			appendage.AsDynamic().GlowColor = new Color(0.8f, 0.85f, 0.9f, 0.6f);
			appendage.ChangeAnimation(24);

			Dynamic._screenAppendage = appendage;
			((List<Appendage>)Dynamic._appendages).Add(appendage);

			var glowTexture = GlowTextureType.CreateInstance(true, Level);
			var dynamicGlowTexture = glowTexture.AsDynamic();
			dynamicGlowTexture.GlowSpriteSheet = Dynamic._sprite;
			dynamicGlowTexture.FrameIndex = 24;
			dynamicGlowTexture.GlowCircleCount = 6;
			dynamicGlowTexture.Center = new Point(TypedObject.Position.X, TypedObject.Position.Y - 36);
			dynamicGlowTexture.GlowCircleWidth = 32;
			dynamicGlowTexture.GlowCircleHeight = 32;

			Dynamic._glowTexture = glowTexture;

			Dynamic._isUsable = true;
		}
	}
}
