using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events.Cutscene;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LabAdult")]
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LabChild")]
	// ReSharper disable once UnusedMember.Global
	class LabExperiment: LevelObject<Monster>
	{

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			/*Level.ToggleExits(false);
			Level.OpenAllBossDoors(-1f);
			Level.LockAllBossDoors(0.5f);

			Level.JukeBox.StopSong();
			Level.JukeBox.PlaySong(EBGM.Library);*/
		}

		public LabExperiment(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			typedObject.InitializeMob();
			// typedObject.SilentKill();
		}



		protected override void OnUpdate()
		{
		}
	}
}
