using System.Collections.Generic;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.HUD;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneLab0")]
	// ReSharper disable once UnusedMember.Global
	class CutsceneLab0 : LevelObject
	{
		public CutsceneLab0(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize()
		{
			Object.SilentKill();

			//abort already triggered scripts
			((List<ScriptAction>)LevelReflected._activeScripts).Clear();
			((Queue<DialogueBox>)LevelReflected._dialogueQueue).Clear();
			((Queue<ScriptAction>)LevelReflected._waitingScripts).Clear();
		}
	}
}
