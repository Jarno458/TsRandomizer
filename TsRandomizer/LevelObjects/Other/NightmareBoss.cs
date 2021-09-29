using Archipelago.MultiClient.Net.Enums;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss")]
	// ReSharper disable once UnusedMember.Global
	class NightmareBoss : LevelObject
	{
		bool hasRun;

		public NightmareBoss(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (hasRun || Dynamic._deathScriptTimer <= 0) return;

			var fillingMethod = Level.GameSave.GetFillingMethod();

			if (fillingMethod == FillingMethod.Archipelago)
				Client.SetStatus(ArchipelagoClientState.ClientGoal);

			hasRun = true;
		}
	}
}
