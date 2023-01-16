using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.LevelObjects
{
	class CollisionDetectionEvent : GameEvent
	{
		readonly Action onCollisionDetection;

		public CollisionDetectionEvent(Level level, Action onCollisionDetection) 
			: base(level, new Point(-100, -100), -1, new ObjectTileSpecification())
		{
			this.onCollisionDetection = onCollisionDetection;

			DoesCollideWithTiles = true;
		}

		public override bool DetectTileCollisions()
		{
			onCollisionDetection();

			return false;
		}
	}
}