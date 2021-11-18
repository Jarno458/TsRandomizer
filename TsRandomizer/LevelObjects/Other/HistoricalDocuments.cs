using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.EnvironmentPrefabs.L11_Lab.EnvPrefabLabHistoricalDocuments")]
	// ReSharper disable once UnusedMember.Global
	class HistoricalDocuments: LevelObject
	{
		bool gyreArchivesEnabled = false;
		public HistoricalDocuments(Mobile typedObject) : base(typedObject)
		{
			if (gyreArchivesEnabled)
				TimeSpinnerGame.Localizer.OverrideKey("q_ram_4_lun_29alt",
					"It says, 'Redacted Temporal Research: Lord of Ravens'. Maybe I should ask the crow about this...");
		}

		protected override void Initialize(SeedOptions options)
		{
			gyreArchivesEnabled = options.GyreArchives;
		}
	}
}
