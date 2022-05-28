using System.Collections.Generic;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;


namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneCity2")]
	class CutsceneCity2 : LevelObject
	{
		public CutsceneCity2(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			// Spindle cutscene, to be removed during boss rando
			Level level = (Level)Dynamic._level;
			bool isRandomized = level.GameSave.GetSettings().BossRando.Value;
			if (!isRandomized || !level.GameSave.GetSaveBool("IsFightingBoss"))
				return;

			Dynamic.SilentKill();

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
		}
	}
}
