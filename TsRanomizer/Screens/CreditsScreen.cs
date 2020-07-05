using System;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
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
			if(hasAddedCredits) return;

			var creditList = (IList)Reflected._creditsList;

			if(creditList.Count != 26) return;

			UpdateMyBackerName(creditList);
			AddSection(creditList, "Randomizer", "Project Manager, Lead Developer", "Jarno Westhof");

			hasAddedCredits = true;
		}

		static void UpdateMyBackerName(IList creditList)
		{
			var backersList = creditList[25];

			var nameList = ((string[])backersList.AsDynamic()._extendedContributors).ToList();

			nameList.Remove("Quandora");
			nameList.Insert(2307, "Jarno Westhof");

			backersList.AsDynamic()._extendedContributors = nameList.ToArray();
		}

		void AddSection(IList creditList, string header, string subtitle, string contributer)
		{
			var section = creditsSectionType.CreateInstance(true, header, subtitle);
			var dynamicSection = section.AsDynamic();

			dynamicSection.AddContributor(contributer);

			creditList.Insert(creditList.Count, section);

			int previousCreditBottom = Reflected._farthestBottomY;
			var headerLineSpacing = ((SpriteFont)Reflected._primaryFont).LineSpacing;
			var subTitleLineSpacing = ((SpriteFont)Reflected._latinFont).LineSpacing;
			var sectionMargin = headerLineSpacing * 4;

			dynamicSection.CalculateSize(previousCreditBottom, headerLineSpacing, subTitleLineSpacing, Reflected._zoom);

			Reflected._farthestBottomY += dynamicSection.Height + sectionMargin;
		}
	}
}
 