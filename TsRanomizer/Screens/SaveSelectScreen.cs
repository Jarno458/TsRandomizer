using System.Collections;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.SaveSelectScreen")]
	// ReSharper disable once UnusedMember.Global
	class SaveSelectScreen : Screen
	{
		readonly dynamic reflected;

		public SaveSelectScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			reflected = screen.Reflect();
		}

		public override void Update(InputState input)
		{
			var saveFileEntries = (IList)((object)reflected._saveFileCollection).Reflect().Entries;

			foreach (var saveFileEntry in saveFileEntries)
			{
				var saveFileReflected = saveFileEntry.Reflect();

				if(saveFileReflected.IsEmptySaveSlot)
					continue;

				var saveFile = (GameSave)saveFileReflected._saveFile;
				var seed = saveFile.FindSeed();
				var areaName = saveFile.GetAreaName();

				saveFileReflected._areaName = $"{seed} {areaName}";
			}
		}
	}
}