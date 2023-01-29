using System;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
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
				ObjectID = (int)EEnemyTileType.LakeFly
			};

			var offsets = new []
			{
				new Tuple<int, int, bool>(-7, -1, false),
				new Tuple<int, int, bool>(-4, -3, false),
				new Tuple<int, int, bool>(0, -7, false),
				new Tuple<int, int, bool>(4, -3, true),
				new Tuple<int, int, bool>(7, -1, true),
			};

			foreach (var offset in offsets)
			{
				bee.X = offset.Item1 + (level.MainHero.LastPosition.X / 16);
				bee.Y = offset.Item2 + (level.MainHero.LastPosition.Y / 16);
				bee.IsFlippedHorizontally = offset.Item3;

				level.PlaceEvent(bee, false);
			}
		}
	}
}