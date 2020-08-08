using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Heroes;

namespace TsRandomizer.IntermediateObjects
{
	class RequestButtonPressTrigger : GameEvent
	{
		readonly Action action;

		public RequestButtonPressTrigger(Level inLevel, Point position, ObjectTileSpecification objectSpec, Action action) 
			: base(inLevel, position, -1, objectSpec)
		{
			this.action = action;

			EventType = EEventTileType.TransitionWarpEvent;
			Bbox = new Rectangle(0, 0, 48, 32);
		}

		public override bool TriggerEvent(Alive who, Vector2 depth)
		{
			if (!IsFrozen)
			{
				_isTriggered = true;
				_level.RequestButtonPrompt(4, new Point(Bbox.Center.X - 2, Bbox.Top));

				if (who is Protagonist protagonist && protagonist.CheckButton(4) && (!protagonist.CheckButton(5) && !protagonist.CheckButton(7)))
					action();
			}

			return base.TriggerEvent(who, depth);
		}
	}
}