﻿using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using System.Reflection;
using Timespinner.GameObjects.Events.Cutscene;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class PalmPunchTrap : Trap
	{
		static readonly MethodInfo PalmPunchMethod = typeof(CutsceneBase).GetPrivateStaticMethod("AddLunaisPalmPunch", typeof(Level));

		public PalmPunchTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.PalmPunchTrap) { }

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			level.MainHero.AddUnskippableWaitScript(1f);
			PalmPunchMethod.InvokeStatic(level);
		}
	}
}
