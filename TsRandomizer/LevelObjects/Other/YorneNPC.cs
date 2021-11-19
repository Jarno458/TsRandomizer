using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.NPCs.YorneNPC")]
	// ReSharper disable once UnusedMember.Global
	class YorneNpc : LevelObject
	{
		public YorneNpc(Mobile typedObject) : base(typedObject)
		{
			if (typedObject.Level.ID == 2) // Copy of Yorne beyond backer room
            {
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_02",
					"Yorne? Oh... not quite. Is this... his memory?");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_yor_03",
					"I still can't believe they picked *her*, I deserved this.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_04",
					"*sigh* Even as just a reflection... Yorne is still as Yorne as ever.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_yor_05",
					"I have to find where that Kobo has run off to.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_06",
					"'Kobo'... There's no one in our village by that name... I don't think this 'memory' is real.");
			}
		}
	}
}
