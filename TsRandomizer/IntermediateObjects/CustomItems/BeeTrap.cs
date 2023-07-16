using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.LevelObjects.Monsters;
using TsRandomizer.LevelObjects.Other;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class BeeTrap : Trap
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
				new Point(-7, -1),
				new Point(-4, -3),
				new Point(0, -7),
				new Point(4, -3),
				new Point(7, -1)
			};

			foreach (var offset in offsets)
			{
				bee.X = offset.X + (level.MainHero.LastPosition.X / 16);
				bee.Y = offset.Y + (level.MainHero.LastPosition.Y / 16);
				bee.IsFlippedHorizontally = offset.X > 0;

				level.PlaceEvent(bee, false);
			}
		}
	}
}