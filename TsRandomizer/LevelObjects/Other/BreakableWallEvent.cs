using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.BreakableWallEvent")]
	class BreakableWallEvent : LevelObject
	{
		SeedOptions options;

		public BreakableWallEvent(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions seedOptions)
		{
			options = seedOptions;
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if(!options.EyeSpy)
				return;

			if (!Level.GameSave.HasRing(EInventoryOrbType.Eye))
			{
				Dynamic._invulnerableTimer = 1f;
				Dynamic._isSolid = true;
			}
		}
	}
}
