using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Extensions;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabHistoricalDocuments")]
	// ReSharper disable once UnusedMember.Global
	class HistoricalDocuments: LevelObject
	{
		public HistoricalDocuments(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			Level.ReplaceDialogue(options);
		}
	}
}
