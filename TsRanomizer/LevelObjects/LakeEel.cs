using System;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Enemies.LakeEel")]
	class LakeEel : LevelObject
	{
		public LakeEel(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
		{
		}

		protected override void Initialize()
		{
			var random = new Random(Seed.Current);
			var randomMultiplier = (float)random.NextDouble();

			Reflected._randomMultiplier = randomMultiplier;
			Reflected._swimVelocityRandomizer = (float)(-randomMultiplier * 25.0);
			Reflected._actionTimer = randomMultiplier;
		}
	}
}
