using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.CurtainDrawbridge")]
	// ReSharper disable once UnusedMember.Global
	class CurtainDrawbridge : LevelObject
	{
		public CurtainDrawbridge(Mobile typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			if (!(!LevelReflected.GetLevelSaveBool("HasWinchBeenUsed") ? seed.Options.GateKeep && !Level.GameSave.HasItem(CustomItem.GetIdentifier(CustomItemType.DrawbridgeKey)) : LevelReflected.GetLevelSaveBool("IsDrawbridgeRaised")))
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
