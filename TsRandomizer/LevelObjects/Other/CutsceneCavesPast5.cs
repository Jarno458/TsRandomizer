using System.Collections.Generic;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneCavesPast5")]
	class CutsceneCavesPast5 : LevelObject
	{
		public CutsceneCavesPast5(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			bool isRandomized = Level.GameSave.GetSettings().BossRando.Value;
			if (!isRandomized)
				return;

			BossAttributes vanillaBoss = BestiaryManager.GetVanillaBoss(Level, (int)EBossID.Maw);
			if (vanillaBoss.Index != (int)EBossID.Prince && vanillaBoss.Index != (int)EBossID.Vol)
				return;

			Dynamic.SilentKill();

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
		}
	}
}

