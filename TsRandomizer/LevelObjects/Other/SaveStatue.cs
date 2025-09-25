using System;
using Timespinner.GameAbstractions.Assets.Audio;
using Timespinner.GameAbstractions.GameObjects;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.SaveStatue")]
	// ReSharper disable once UnusedMember.Global
	class SaveStatue : LevelObject
	{
		static readonly Type SaveOrbStateType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.SaveStatue+ESaveOrbState");

		public SaveStatue(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (Dynamic._isBroken)
				return;

			bool breakAllSaves = Level.GameSave.GetSettings().NoSaveStatues.Value;

			if (!breakAllSaves
				&& (Level.ID != 14 || Level.RoomID != 8) // Ravenlord
				&& (Level.ID != 14 || Level.RoomID != 6))  // Ifrit
				return;

			Dynamic._isBroken = true;
			Dynamic._orbSaveState = SaveOrbStateType.GetEnumValue("Dead");

			var orbAppendage = (Appendage)Dynamic._orbAppendage;
			orbAppendage.ChangeAnimation(5); //5 = broken
			orbAppendage.ClearBattleAnimations();
			orbAppendage.IsGlowing = false;

			((SFXCueInstance)Dynamic._glowCueInstance)?.Stop();
		}
	}
}
