using System.Linq;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class LoadDefaultItems
	{
		[Test]
		public void Should_load_all_items()
		{
			var itemLocations = new ItemLocationMap();

			var levelSpecificaiton = LevelSpecification.FromCompressedFile(@"E:\TimeSpinnerModding\AssemblyHack\Content\Levels\Level_01.dat");

			foreach (var itemLocation in itemLocations
				.Where(l => l.Key.LevelId == 1))
			{
				var x = (itemLocation.Key.X - 8) / 16;
				var y = (itemLocation.Key.Y - 16) / 16;

				var room = levelSpecificaiton.Rooms.First(r => r.ID == itemLocation.Key.RoomId);

				var objectTile = room.ObjectTiles.First(t => t.Key.X == x && t.Key.Y == y).Value.First();

				switch (objectTile.GetEventType())
				{
					case EEventTileType.TreasureChest:
						var newEvent = (GameEvent)new TreasureChestEvent(null, new Point(), 1, objectTile);
						break;
				}

			}
		}
	}
}
