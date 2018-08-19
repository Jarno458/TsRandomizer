using System;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;

namespace TsRanodmizer.IntermediateObjects
{
	abstract class TriggerBase : GameEvent
	{
		readonly Action action;
		bool hasTriggered;

		protected TriggerBase(Level inLevel, Action action) 
			: base(inLevel, new Point(-50,-50), -1, new ObjectTileSpecification())
		{
			this.action = action;
		}

		protected abstract void OnUpdate();

		public override void Update(float delta)
		{
			base.Update(delta);

			if (hasTriggered)
				return;

			OnUpdate();
		}

		protected void Trigger()
		{
			hasTriggered = true;
			action();
			Kill();
		}
	}
}