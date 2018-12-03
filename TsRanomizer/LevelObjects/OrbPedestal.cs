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
		
		List<Appendage> Appendages => Reflected._appendages;
		int appendagesCount;
		bool hasDroppedLoot;

		public OrbPedestal(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
			menuIcons = ((Level)Reflected._level).GCM.SpMenuIcons;
		}
		
		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			if (ItemInfo.LootType == LootType.Orb)
			{
				Reflected._orbType = ItemInfo.OrbType;
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

			if (!Reflected.HasBeenPickedUp) return;

			var level = (Level)Reflected._level;
			var scripts = (Queue<ScriptAction>)level.Reflect()._waitingScripts;

			scripts.UpdateRelicOrbGetToastToItem(ItemInfo);

			AwardContainedItem(level);
			hasDroppedLoot = true;
		}

		void UpdateMeleeOrbSprite()
		{
			const int wierdOffset = 19;
			if (Appendages.Count == 0)
				((Animate)Object).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
			else
				((Appendage)Reflected._orbAppendage).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
		}

		void UpdateSprite()
		{
			if (Appendages.Count == 0)
			{
				Reflected._sprite = menuIcons;
				((Animate)Object).ChangeAnimation(ItemInfo.AnimationIndex);
			}
			else
			{
				Appendage orbAppendage = (Appendage)Reflected._orbAppendage;
				orbAppendage.AnchorOffset = new Point(-4, -36); //TODO fix glow position
				((Animate)Reflected._orbAppendage).Reflect()._sprite = menuIcons;
				((Animate)Reflected._orbAppendage).ChangeAnimation(ItemInfo.AnimationIndex);
			}
		}

		void UpdateOrbGlowColor()
		{
			var orbGlowColorVector = (Vector4)GetOrbGlowColorByTypeMethod.InvokeStatic(ItemInfo.OrbType);
			var orbGlowColor = new Color(orbGlowColorVector);
			Reflected._baseOrbGlowColorAsVector = orbGlowColorVector;
			Reflected._baseOrbGlowColorAsColor = orbGlowColor;
			((object)Reflected._glowCircle).Reflect().BaseColor = orbGlowColor;
			((OrbPedestalLeakParticleSystem)Reflected._pixelLeakParticleSystem).BaseColor = orbGlowColorVector;
		}

		void UpdateGlowColor()
		{
			var orbGlowColorVector = new Vector4(0.75f);
			var orbGlowColor = new Color(orbGlowColorVector);
			Reflected._baseOrbGlowColorAsVector = orbGlowColorVector;
			Reflected._baseOrbGlowColorAsColor = orbGlowColor;
			((object)Reflected._glowCircle).Reflect().BaseColor = orbGlowColor;
			((OrbPedestalLeakParticleSystem)Reflected._pixelLeakParticleSystem).BaseColor = orbGlowColorVector;
		}
	}
}
