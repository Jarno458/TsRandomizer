using System;
using Microsoft.Xna.Framework;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	/*[TimeSpinnerType("Timespinner.GameObjects.Events.JunkSpawnerEvent")]
	class LabTrashSpawner : LevelObject<Animate>
	{
		int damage;
		int hp;
		int exp;

		public LabTrashSpawner(Animate typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
			//at level 11
			// damage = 48
			// hp = 32;

			damage = gameplayScreen.Level.ID * 4;
			hp = gameplayScreen.Level.ID * 3;
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			var shouldScale = settings.EnemyRando.Value == "Scaled" || settings.EnemyRando.Value == "Ryshia";
			if (!shouldScale)
				return;
			
			for (int i = 0; i < 7; i++)
				Dynamic.CreateJunk(new Point(-100, 0), Dynamic._junkType == ERobotJunkType.Spawn_Crushed ? ERobotJunkType.Junk_Crushed : ERobotJunkType.Junk_Fresh);

			for (int i = 0; i < 7; i++)
			{
				var junk = (Monster)Dynamic._junkStorage[i];
				junk.ScaleTo(damage, hp, hp, (int)Math.Ceiling(exp / 8f));
			}
		}
	}*/
}
