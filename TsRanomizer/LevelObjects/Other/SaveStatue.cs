using System;
using Timespinner.GameAbstractions.Assets.Audio;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.SaveStatue")]
	// ReSharper disable once UnusedMember.Global
	class SaveStatue : LevelObject
	{
		Type SaveOrbStateType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.SaveStatue+ESaveOrbState");

		public SaveStatue(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize()
		{
			if (Level.ID != 2 || Level.RoomID != 20 || Object._isBroken) //Right side libarary elevator room
				return;

			Object._isBroken = true;
			Object._orbSaveState = SaveOrbStateType.GetEnumValue("Dead");

			var orbAppendage = (Appendage)Object._orbAppendage;
			orbAppendage.ChangeAnimation(5); //5 = broken
			orbAppendage.ClearBattleAnimations();
			orbAppendage.IsGlowing = false;

			((SFXCueInstance)Object._glowCueInstance)?.Stop();
		}
	}
}
