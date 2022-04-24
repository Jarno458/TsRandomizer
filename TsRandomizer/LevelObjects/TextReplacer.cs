using System;
using Timespinner.GameAbstractions.Gameplay;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;

namespace TsRandomizer.LevelObjects
{
    class TextReplacer
    {
	    static readonly LookupDictionary<RoomItemKey, TextReplacer> TextReplacers = new LookupDictionary<RoomItemKey, TextReplacer>(rt => rt.key);

		static TextReplacer()
	    {
			TextReplacers.Add(new TextReplacer(16,26, (level, itemLocations, options) =>
			{
				var concussions = level.GameSave.GetConcussionCount();

				var replacement = "What—? I don't *think* I hit my head...";
				switch (concussions)
				{
					case 1:
						replacement = "What—? I feel like I've suffered a concussion...";
						break;
					case int c when (c > 1):
						replacement = $"What—? I feel like I've suffered {concussions} concussions...";
						break;
				}
				TimeSpinnerGame.Localizer.OverrideKey("cs_tem_1_lun_01", replacement);
			}));
			TextReplacers.Add(new TextReplacer(11, 4, (level, itemLocations, options) => {
				if (options.GyreArchives)
					TimeSpinnerGame.Localizer.OverrideKey("q_ram_4_lun_29alt",
						"It says, 'Redacted Temporal Research: Lord of Ravens'. Maybe I should ask the crow about this...");
			}));
			TextReplacers.Add(new TextReplacer(2, 51, (level, itemLocations, options) => {
				if (!options.GyreArchives)
					return;
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_02",
					"Yorne? Oh... not quite. Is this... his memory?");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_03",
					"I still can't believe they picked *her*, I deserved this.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_04",
					"*sigh* Even as just a reflection... Yorne is still as Yorne as ever.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_05",
					"I have to find where that Kobo has run off to.");
				TimeSpinnerGame.Localizer.OverrideKey("cs_pro_lun_06",
					"'Kobo'... There's no one in our village by that name... I don't think this 'memory' is real.");
			}));
		}

		public static void OnChangeRoom(
			Level level, SeedOptions seedOptions, ItemLocationMap itemLocations, int levelId, int roomId)
		{
			var roomKey = new RoomItemKey(levelId, roomId);

			if (TextReplacers.TryGetValue(roomKey, out var replacer))
				replacer.replacer(level, itemLocations, seedOptions);
		}

		readonly RoomItemKey key;
		readonly Action<Level, ItemLocationMap, SeedOptions> replacer;

		TextReplacer(int level, int room, Action<Level, ItemLocationMap, SeedOptions> replacer)
		{
			key = new RoomItemKey(level, room);
			this.replacer = replacer;
		}
	}
}
