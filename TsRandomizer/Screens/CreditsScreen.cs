using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.CreditsScreen")]
	// ReSharper disable once UnusedMember.Global
	class CreditsScreen : Screen
	{
		readonly Type creditsSectionType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.MainMenu.CreditsSection");

		bool hasAddedCredits;

		public CreditsScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (hasAddedCredits) return;

			var creditList = (IList)Dynamic._creditsList;

			if (creditList.Count != 26) return;

			AddSection(20, creditList, "Randomizer");
			AddSection(21, creditList, "Project Manager, Lead Developer", "Jarno Westhof");
			AddSection(22, creditList, "Linux & Max support, Quality of Life", "marcmagus");
			AddSection(23, creditList, "Gyre Archives, Cantoran, Quality of Life", "TriumphantBass");
			AddSection(24, creditList, "Bonk counter, Randomized pickups, Quality of Life", "weffjebster");

			RecalcuteCreditSizes(creditList);

			hasAddedCredits = true;
		}

		void RecalcuteCreditSizes(IList creditList)
		{
			int previousCreditBottom = 0;
			int primaryLineSpacing = Dynamic._primaryFont.LineSpacing;
			int secondairyLineSpacing = Dynamic._latinFont.LineSpacing;
			int sectionSpacing = primaryLineSpacing * 4;

			foreach (var section in creditList)
			{
				var sectionDynamic = section.AsDynamic();

				sectionDynamic.CalculateSize(previousCreditBottom, primaryLineSpacing, secondairyLineSpacing, Dynamic._zoom);
				previousCreditBottom += sectionDynamic.Height + sectionSpacing;
			}
			Dynamic._farthestBottomY = previousCreditBottom;
		}

		void AddSection(int index, IList creditList, string header, string contributer)
		{
			var section = creditsSectionType.CreateInstance(true, header);
			var dynamicSection = section.AsDynamic();

			dynamicSection.AddContributor(contributer);

			creditList.Insert(index, section);
		}

		void AddSection(int index, IList creditList, string header)
		{
			var section = creditsSectionType.CreateInstance(true, header);
			creditList.Insert(index, section);
		}
	}
}
 