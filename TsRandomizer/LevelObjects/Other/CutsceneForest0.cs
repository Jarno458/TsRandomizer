using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;


namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneForest0")]
	// ReSharper disable once UnusedMember.Global
	class CutsceneForest0 : LevelObject
	{
		public CutsceneForest0(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			Scripts.MakeEventsSkippable();
		}
	}
}
