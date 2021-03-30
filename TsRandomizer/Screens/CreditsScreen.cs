using System;
using System.Collections;
using System.Linq;
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

			UpdateMyBackerName(creditList);
			AddSection(20, creditList, "Randomizer", "Project Manager, Lead Developer", "Jarno Westhof");
			AddSection(21, creditList, "Randomizer", "Linux & Max support, Quality of Life", "marcmagus");

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

		static void UpdateMyBackerName(IList creditList)
		{
			var backersList = creditList[25];

			var nameList = ((string[])backersList.AsDynamic()._extendedContributors).ToList();

			nameList.Remove("Quandora");
			nameList.Insert(2307, "Jarno Westhof");

			backersList.AsDynamic()._extendedContributors = nameList.ToArray();
		}

		void AddSection(int index, IList creditList, string header, string subtitle, string contributer)
		{
			var section = creditsSectionType.CreateInstance(true, header, subtitle);
			var dynamicSection = section.AsDynamic();

			dynamicSection.AddContributor(contributer);

			creditList.Insert(index, section);
		}
	}
}
 