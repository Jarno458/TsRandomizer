using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Microsoft.Xna.Framework;
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
	// ReSharper disable once UnusedMember.Global
	class OrbPedestal : LevelObject<Mobile>
	{
		static MethodInfo GetOrbGlowColorByTypeMethod = TimeSpinnerType
			.Get("Timespinner.GameObjects.Heroes.Orbs.LunaisOrb")
			.GetMethod("GetOrbGlowColorByType", BindingFlags.Static | BindingFlags.NonPublic);

		bool hasDroppedLoot;

		public OrbPedestal(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, new ItemInfo(EInventoryOrbType.Flame, EOrbSlot.Melee))
		{
		}

		List<Appendage> Appendages => Reflected._appendages;

		protected override void Initialize()
		{
			if (ItemInfo == null)
				return;

			Reflected._orbType = ItemInfo.OrbType;

			UpdateGlowColor();
			UpdateSprite();
		}

		void UpdateSprite()
		{
			const int wierdOffset = 19;
			if (Appendages.Count == 0)
				((Animate)Object).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
			else
				((Appendage)Reflected._orbAppendage).ChangeAnimation((int)ItemInfo.OrbType + wierdOffset);
		}

		void UpdateGlowColor()
		{
			var orbGlowColorVector = GetOrbGlowColorByType(ItemInfo.OrbType);
			var orbGlowColor = new Color(orbGlowColorVector);
			Reflected._baseOrbGlowColorAsVector = orbGlowColorVector;
			Reflected._baseOrbGlowColorAsColor = orbGlowColor;
			((object)Reflected._glowCircle).Reflect().BaseColor = orbGlowColor;
			((OrbPedestalLeakParticleSystem)Reflected._pixelLeakParticleSystem).BaseColor = orbGlowColorVector;
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasDroppedLoot)
				return;

			if (ItemInfo.LootType == LootType.Orb && Reflected.HasBeenPickedUp)
			{
				var gameSave = ((Level)Reflected._level).GameSave;
				gameSave.AddOrb(ItemInfo.OrbType, ItemInfo.OrbSlot);
				hasDroppedLoot = true;
			}
			//TODO: incase of not an melee orb??
		}

		Vector4 GetOrbGlowColorByType(EInventoryOrbType orbType)
		{
			return (Vector4)GetOrbGlowColorByTypeMethod.Invoke(null, new object[]{ orbType });
		}
	}
}
