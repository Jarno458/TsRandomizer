using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneLab2")]
	// ReSharper disable once UnusedMember.Global
	class CutsceneLab2 : LevelObject
	{
		public CutsceneLab2(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(SeedOptions options)
		{
			bool hasTimespinnerPieces = AreTriggerConditionsMet();
			Level.GameSave.SetValue("TSRando_IsLabTSReady", hasTimespinnerPieces);

			if ((Level.GameSave.GetSettings().BossRando.Value && Level.GameSave.GetSaveBool("IsFightingBoss")) || !hasTimespinnerPieces)
				Dynamic.SilentKill();
		}

		bool AreTriggerConditionsMet()
		{
			var relics = Level.GameSave.Inventory.RelicInventory.Inventory;

			return
				relics.ContainsKey((int) EInventoryRelicType.TimespinnerWheel)
				&& relics.ContainsKey((int)EInventoryRelicType.TimespinnerSpindle)
				&& relics.ContainsKey((int)EInventoryRelicType.TimespinnerGear1)
				&& relics.ContainsKey((int)EInventoryRelicType.TimespinnerGear2)
				&& relics.ContainsKey((int)EInventoryRelicType.TimespinnerGear3);
		}
	}
}
