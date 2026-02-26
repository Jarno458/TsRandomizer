using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer
{
	[HarmonyPatch]
	class QoLMenuInjector
	{
		static readonly Type OptionsMenuScreenType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen");
		static readonly Type MainMenuEntryType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.MenuEntry");

		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.PauseMenu.OptionsMenuScreen");
			return AccessTools.Method(type, "LoadContent");
		}

		static void Postfix(object __instance)
		{
			try
			{
				var screenManager = (ScreenManager)((GameScreen)__instance).ScreenManager;
				var button = MenuEntry.Create("Randomizer QoL Settings", _ => OpenQoLMenu(screenManager));
				button.IsCenterAligned = false;
				button.DoesDrawLargeShadow = false;
				var primaryMenuCollection = ((object)__instance.AsDynamic()._primaryMenuCollection).AsDynamic();
				var buttons = (IList)primaryMenuCollection._entries;
				buttons.Add(button.AsTimeSpinnerMenuEntry());
				primaryMenuCollection._entries = buttons.ToList(MainMenuEntryType);
			}
			catch
			{
				// ignored
			}
		}

		static void OpenQoLMenu(ScreenManager screenManager)
		{
			QoLSettingsMenu.IsQoLMenuPending = true;
			var menu = QoLSettingsMenu.Create(screenManager);
			screenManager.AddScreen(menu, PlayerIndex.One);
		}
	}
}