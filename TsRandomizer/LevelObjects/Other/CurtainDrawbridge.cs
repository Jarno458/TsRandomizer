using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.CurtainDrawbridge")]
	// ReSharper disable once UnusedMember.Global
	class CurtainDrawbridge : LevelObject
	{
		public CurtainDrawbridge(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed)
		{
			if (!(!LevelReflected.GetLevelSaveBool("HasWinchBeenUsed") ? false : LevelReflected.GetLevelSaveBool("IsDrawbridgeRaised")))
			{
				Dynamic._isEngineerDead = true;
				Dynamic._isRaising = true;
				Dynamic._raiseLowerCounter = 0.0f;
			}
			else
			{
				Dynamic._isRaising = false;
				Dynamic._raiseLowerCounter = 4f;
			}
		}
	}
}
