using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.LevelObjects
{
	class RandomizerEvent : GameEvent
	{
		public const int Id = -69;

		readonly Action onCollisionDetection;

		public RandomizerEvent(Level level, Dictionary<int, GameEvent> events, Action onCollisionDetection) 
			: base(level, new Point(-100, -100), -1, new ObjectTileSpecification())
		{
			this.onCollisionDetection = onCollisionDetection;

			DoesCollideWithTiles = true;

			events.Add(Id, this);
		}

		public override bool DetectTileCollisions()
		{
			onCollisionDetection();

			return false;
		}
	}
}