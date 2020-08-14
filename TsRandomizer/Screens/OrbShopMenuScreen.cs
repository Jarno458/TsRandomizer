using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.Shop.OrbShopMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class OrbShopMenuScreen : Screen
	{
		public OrbShopMenuScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			var emptyOrbShopMenuEntryList = new object[0]
				.ToList(TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.Shop.OrbShopMenuEntry"));
			var emptyMenuEntryList = new object[0]
				.ToList(TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry"));

			((object)Reflected._spellOrbMenuCollection).AsDynamic()._items = emptyOrbShopMenuEntryList;
			((object)Reflected._spellOrbMenuCollection).AsDynamic()._entries = emptyMenuEntryList;

			((object)Reflected._passiveOrbMenuCollection).AsDynamic()._items = emptyOrbShopMenuEntryList;
			((object)Reflected._passiveOrbMenuCollection).AsDynamic()._entries = emptyMenuEntryList;
			
			((object)Reflected._infuseOrbMenuCollection).AsDynamic()._items = emptyOrbShopMenuEntryList;
			((object)Reflected._infuseOrbMenuCollection).AsDynamic()._entries = emptyMenuEntryList;
		}
	}
}