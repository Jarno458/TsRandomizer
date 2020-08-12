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
			if (Object._isBroken) 
				return;

			if ((Level.ID != 2 || Level.RoomID != 20) //Right side libarary elevator room
				&& (Level.ID != 16 || Level.RoomID != 21))  //Pyramid Pit
				return;

			Object._isBroken = true;
			Object._orbSaveState = SaveOrbStateType.GetEnumValue("Dead");

			var orbAppendage = (Appendage) Object._orbAppendage;
			orbAppendage.ChangeAnimation(5); //5 = broken
			orbAppendage.ClearBattleAnimations();
			orbAppendage.IsGlowing = false;

			((SFXCueInstance) Object._glowCueInstance)?.Stop();
		}
	}
}
