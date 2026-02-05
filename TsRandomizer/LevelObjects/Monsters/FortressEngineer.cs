using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.RoomTriggers;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.LevelObjects.Monsters
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.FortressEngineer")]
	class FortressEngineer : LevelObject<Monster>
	{
		static readonly Type FortressEngineerBombType = TimeSpinnerType.Get("Timespinner.GameAbstractions.GameObjects.FortressEngineerBomb");

		// Only ERobotJunkType.Junk_Fresh as in uncrushed junk
		static readonly int[] trashIds = { 0, 3, 6 };

		Seed seed;

		public FortressEngineer(Monster typedObject, GameplayScreen gameplayScreen) : base(typedObject, gameplayScreen)
		{
		}

		protected override void Initialize(Seed seed, SettingCollection settings)
		{
			this.seed = seed;
		}

		protected override void OnUpdate()
		{
			// Modified behavior exclusive to new trash jump engineers in lab before the dynamo
			if (Level.ID != 11 || Level.RoomID != 18)
				return;

			// These engineers only place by the room trigger LabPowerChanges.cs [RoomTriggerTrigger(11, 18)], this condition is mirrored there
			if (seed.Options.LockKeyAmadeus || !Level.GameSave.GetSaveBool("11_LabPower"))
				return;

			Dynamic._aggroBbox = new Rectangle(0, 0, 0, 0);

			var sprite = Level.GCM.SpRobotJunk;
			Point position = Dynamic.Position;
			position.X += (Dynamic.IsFacingLeft ? -24 : 24);

			Random rand = new Random(((int)seed.Id ^ Level.RoomID) + position.X);

			var bombBag = Enumerable.Range(1, 5)
				.Select(i => {
					try
					{
						if (!(FortressEngineerBombType.CreateInstance(false, position, Level, sprite, -1, 0) is Projectile bomb))
							return null;

						bomb.AsDynamic()._animationStart = trashIds.SelectRandom(rand);

						return bomb;
					}
					catch (Exception e)
					{
						return null;
					}

				})
				.Where(b => b != null)
				.ToArray(FortressEngineerBombType);
			
			Dynamic._bombBag = bombBag;
		}
	}
}
