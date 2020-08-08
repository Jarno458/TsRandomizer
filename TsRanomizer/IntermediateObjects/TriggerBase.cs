using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRandomizer.IntermediateObjects
{
	abstract class TriggerBase : GameEvent
	{
		static readonly Point OutOfBounds = new Point(-50, -50);

		readonly Action action;
		bool hasTriggered;

		protected TriggerBase(Level inLevel, Action action) 
			: base(inLevel, OutOfBounds, -1, new ObjectTileSpecification())
		{
			this.action = action;
		}

		protected abstract bool ShouldTrigger();

		public override void Update(float delta)
		{
			base.Update(delta);

			if (hasTriggered)
				return;

			if(ShouldTrigger())
				Trigger();
		}

		protected void Trigger()
		{
			hasTriggered = true;
			action();
			Kill();
		}
	}
}