using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.LevelObjects.Other;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class BeeTrap : CustomItem
	{
		public BeeTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.BeeTrap) {}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			var bee = new ObjectTileSpecification
			{
				Category = EObjectTileCategory.Enemy,
				Layer = ETileLayerType.Objects,
				ObjectID = (int)EEnemyTileType.LakeFly,
				Argument = Bee.Argument
			};

			var offsets = new []
			{
				new Tuple<int, int>(-7, -1),
				new Tuple<int, int>(-4, -3),
				new Tuple<int, int>(0, -7),
				new Tuple<int, int>(4, -3),
				new Tuple<int, int>(7, -1),
			};

			foreach (var offset in offsets)
			{
				bee.X = offset.Item1 + (level.MainHero.LastPosition.X / 16);
				bee.Y = offset.Item2 + (level.MainHero.LastPosition.Y / 16);
				bee.IsFlippedHorizontally = offset.Item1 > 0;

				level.PlaceEvent(bee, false);
			}
		}
	}
}