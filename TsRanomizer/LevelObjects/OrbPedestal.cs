using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent")]
	[AlwaysSpawn(EEventTileType.OrbPedestal)] //TODO Fix spawnning without sprite
	// ReSharper disable once UnusedMember.Global
	class OrbPedestal : LevelObject<Mobile>
	{
		static readonly MethodInfo GetOrbGlowColorByTypeMethod = TimeSpinnerType
			.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrb")
			.GetPrivateStaticMethod("GetOrbGlowColorByType", typeof(EInventoryOrbType));

		readonly SpriteSheet menuIcons;
		
		List<Appendage> Appendages => Object._appendages;
		int appendagesCount;
		bool hasDroppedLoot;

		public OrbPedestal(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			menuIcons = ((Level)Object._level).GCM.SpMenuIcons;
		}
		
		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			if (ItemInfo.LootType == LootType.Orb)
			{
				Object._orbType = ItemInfo.OrbType;
				UpdateOrbGlowColor();

				if (ItemInfo.OrbSlot == EOrbSlot.Melee)
					UpdateMeleeOrbSprite();
				else
					UpdateSprite();
			}
			else
			{
				UpdateSprite();
				UpdateGlowColor();
			}
			
			appendagesCount = Appendages.Count;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot)
				return;

			if (Appendages.Count != appendagesCount)
			{
				appendagesCount = Appendages.Count;
				UpdateSprite();
			}

			if (!Object.HasBeenPickedUp) return;

			Scripts.UpdateRelicOrbGetToastToItem(ItemInfo);

			AwardContainedItem();
			hasDroppedLoot = true;
		}

		void UpdateMeleeOrbSprite()
		{
			const int wierdOffset = 19;
			if (Appendages.Count == 0)
				((Animate)~Object).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
			else
				((Appendage)Object._orbAppendage).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
		}

		void UpdateSprite()
		{
			if (Appendages.Count == 0)
			{
				Object._sprite = menuIcons;
				((Animate)~Object).ChangeAnimation(ItemInfo.AnimationIndex);
			}
			else
			{
				Appendage orbAppendage = (Appendage)Object._orbAppendage;
				orbAppendage.AnchorOffset = new Point(-4, -36); //TODO fix glow position
				((Animate)Object._orbAppendage).AsDynamic()._sprite = menuIcons;
				((Animate)Object._orbAppendage).ChangeAnimation(ItemInfo.AnimationIndex);
			}
		}

		void UpdateOrbGlowColor()
		{
			var orbGlowColorVector = (Vector4)GetOrbGlowColorByTypeMethod.InvokeStatic(ItemInfo.OrbType);
			var orbGlowColor = new Color(orbGlowColorVector);
			Object._baseOrbGlowColorAsVector = orbGlowColorVector;
			Object._baseOrbGlowColorAsColor = orbGlowColor;
			((object)Object._glowCircle).AsDynamic().BaseColor = orbGlowColor;
			((OrbPedestalLeakParticleSystem)Object._pixelLeakParticleSystem).BaseColor = orbGlowColorVector;
		}

		void UpdateGlowColor()
		{
			var orbGlowColorVector = new Vector4(0.75f);
			var orbGlowColor = new Color(orbGlowColorVector);
			Object._baseOrbGlowColorAsVector = orbGlowColorVector;
			Object._baseOrbGlowColorAsColor = orbGlowColor;
			((object)Object._glowCircle).AsDynamic().BaseColor = orbGlowColor;
			((OrbPedestalLeakParticleSystem)Object._pixelLeakParticleSystem).BaseColor = orbGlowColorVector;
		}
	}
}
