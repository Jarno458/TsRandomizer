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

		protected override void Initialize(Seed seed)
		{
			options = seed.Options;

			if (seed.FloodFlags.Basement && Level.ID == 5 && Level.RoomID == 10) //castle basement secret
				Dynamic.IsUnderwater = true;
			if (seed.FloodFlags.Maw && Level.ID == 8 && Level.RoomID == 9) //maw basement secret
				Dynamic.IsUnderwater = true;
			if (seed.FloodFlags.PyramidShaft && Level.ID == 16 && (Level.RoomID == 8 || Level.RoomID == 22)) //Pyramid shaft
				Dynamic.IsUnderwater = true;
			if (seed.FloodFlags.DryLakeSerene && Level.ID == 7 && Level.RoomID == 6) //Lake serene secret
				Dynamic.IsUnderwater = false;
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if (!options.EyeSpy)
				return;

			if (!Level.GameSave.HasRing(EInventoryOrbType.Eye))
			{
				Dynamic._invulnerableTimer = 1f;
				Dynamic._isSolid = true;
			}
		}
	}
}
