using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.Journal.FeatsMenuEntryCollection")]
	// ReSharper disable once UnusedMember.Global
	class FeatsMenuEntryCollection : Screen
	{
		static readonly Type FeatsMenuEntryType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.Inventory.FeatsMenuEntry");

		public FeatsMenuEntryCollection(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			var buttons = (IList)((object)Dynamic._primaryMenuCollection).AsDynamic()._entries;

			var testButton = MenuEntry.Create("Test Achievement", NullAction);
			buttons.Add(testButton.AsTimeSpinnerMenuEntry());
			((object)Dynamic._featsEntries).AsDynamic()._entries = buttons.ToList(FeatsMenuEntryType);
		}

		void NullAction(PlayerIndex playerIndex)
		{
		}
	}
}
