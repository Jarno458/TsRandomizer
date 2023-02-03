using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;

namespace TsRandomizer.Extensions
{
	public static class ScriptActionQueueExtensions
	{
		public static readonly Vector4 ReplacedArguments = new Vector4(1, 3, 3, 7);

		internal static void RemoveGiveItem(this Queue<ScriptAction> scripts)
		{
			var giveItemScript = scripts.Last(s => s.AsDynamic().ScriptType == EScriptType.GiveItem);
			var reflectedScript = giveItemScript.AsDynamic();

			reflectedScript.ScriptType = EScriptType.Delegate;
			reflectedScript.Delegate = (Action)(() => {});
			reflectedScript.Arguments = ReplacedArguments;
		}

		internal static void MakeEventsSkippable(this Queue<ScriptAction> scripts)
		{
			foreach (var script in scripts)
				script.AsDynamic().IsUnskippable = false;
		}
	}
}
