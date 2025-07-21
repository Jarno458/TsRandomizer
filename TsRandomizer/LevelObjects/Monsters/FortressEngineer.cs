using System;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;
using TsRandomizer.Settings;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using TsRandomizer.IntermediateObjects;
using System;
using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;



namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.FortressEngineer")]
	class FortressEngineer : LevelObject<Monster>
	{
		public FortressEngineer(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
		}

		protected override void OnUpdate()
		{
			// Modified behavior exclusive to new trash jump engineers in lab
			if (Level.ID != 11)
				return;
			Dynamic._aggroBbox = new Rectangle(0, 0, 0, 0);
			var bombType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.FortressEngineerBomb");
			var sprite = Level.GCM.SpRobotJunk;
			Point position = Dynamic.Position;
			position.X += (Dynamic.IsFacingLeft ? -24 : 24);
			var bomb = (Projectile)bombType.CreateInstance(false, position, Level, sprite, -1, 0);

			Random rand = new Random();
			// Only ERobotJunkType.Junk_Fresh
			int[] trashIds = { 0, 3, 6 };
			for (int i = 0; i < 5; i++)
			{
				int trashId = rand.Next(trashIds.Length);
				bomb.AsDynamic()._animationStart = rand.Next(trashId);
				Dynamic._bombBag[i] = bomb;
			}
		}
	}
}
