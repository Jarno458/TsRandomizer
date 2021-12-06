using System;
using Timespinner.GameAbstractions.Gameplay;

namespace TsRandomizer.Extensions
{
    static class TextReplacerLevelExtensions
    {
	    internal static void ReplaceText(this Level level, SeedOptions options)
	    {
		    TextReplacer(level, options)();
	    }

	    internal static string RoomKeyString(this Level level) => $"{level.ID}.{level.RoomID}";

		static Action TextReplacer(Level level, SeedOptions options)
        {
			switch(level.RoomKeyString())
            {
				case "16.26": // initialize during the nightmare fight. cutscenes do not override before they're queued
					return () =>
					{
						int concussions = level.GameSave.GetConcussionCount();

						string replacement = "What—? I don't *think* I hit my head...";
						switch (concussions)
						{
							case 1:
								replacement = "What—? I feel like I've suffered a concussion...";
								break;
							case int c when (c > 1):
								replacement =
									string.Format("What—? I feel like I've suffered {0} concussions...",
									concussions);
								break;
						}
						TimeSpinnerGame.Localizer.OverrideKey("cs_tem_1_lun_01", replacement);
					};
				case "2.51":
					return () =>
					{
						if (options.GyreArchives)
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
					};
				case "11.4":
					return () =>
					{
						if (options.GyreArchives)
							TimeSpinnerGame.Localizer.OverrideKey("q_ram_4_lun_29alt",
								"It says, 'Redacted Temporal Research: Lord of Ravens'. Maybe I should ask the crow about this...");
					};
				//case "3.16":
				//    return () =>
				//    {
				//        TimeSpinnerGame.Localizer.OverrideKey("sign_forest_directions",
				//            GetRequiredProgressionHint(itemLocations, level));
				//    };
				//case "5.14":
				//    return () =>
				//    {
				//        TimeSpinnerGame.Localizer.OverrideKey("inv_jou_Letter6_14",
				//            string.Format("Aelana\n\nP.S. {0}", GetRequiredItemHint(itemLocations, level)));
				//    };
				default: return () => { };
			}
		}
    }
}
