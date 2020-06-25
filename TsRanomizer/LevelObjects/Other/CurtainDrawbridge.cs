using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.CurtainDrawbridge")]
	// ReSharper disable once UnusedMember.Global
	class CurtainDrawbridge : LevelObject
	{
		public CurtainDrawbridge(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize()
		{
			if (!(!LevelReflected.GetLevelSaveBool("HasWinchBeenUsed") ? false : LevelReflected.GetLevelSaveBool("IsDrawbridgeRaised")))
			{
				Object._isEngineerDead = true;
				Object._isRaising = true;
				Object._raiseLowerCounter = 0.0f;
			}
			else
			{
				Object._isRaising = false;
				Object._raiseLowerCounter = 4f;
			}
		}
	}
}
