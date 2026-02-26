using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.Screens;
using TsRandomizer.Screens.Menu;
using GameScreen = Timespinner.GameStateManagement.ScreenManager.GameScreen;
using ScreenManager = Timespinner.GameStateManagement.ScreenManager.ScreenManager;

namespace TsRandomizer
{
	public static class QoLSettingsMenu
	{
		static readonly Type JournalMenuType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");
		static readonly Type MenuEntryType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.MenuEntry");

		public static bool IsQoLMenuPending { get; set; }

		public static GameScreen Create(ScreenManager screenManager)
		{
			var gcm = (GCM)screenManager.AsDynamic().GCM;
			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);

			void OnExit() { }

			return (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.DemoSave, gcm, (Action)OnExit);
		}
	}

	[HarmonyPatch]
	class QoLSettingsMenuInitPatch
	{
		static readonly Type JournalMenuType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");
		static readonly Type MenuEntryType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.MenuEntry");

		static MethodBase TargetMethod() =>
			AccessTools.Method(typeof(GameSettingsScreen), "Initialize");

		static void Postfix(object __instance)
		{
			if (!QoLSettingsMenu.IsQoLMenuPending)
				return;

			QoLSettingsMenu.IsQoLMenuPending = false;

			try
			{
				BuildMenu(((Screen)__instance).GameScreen, 0);
			}
			catch
			{
				// ignored
			}
		}

		internal static void BuildMenu(GameScreen gameScreen, int selectedIndex)
		{
			var emptyList = new object[0].ToList(MenuEntryType);

			AccessTools.Field(JournalMenuType, "_menuTitle").SetValue(gameScreen, "QoL Menu");

			void ClearCollection(string fieldName)
			{
				var col = AccessTools.Field(JournalMenuType, fieldName).GetValue(gameScreen);
				AccessTools.Field(col.GetType(), "_entries").SetValue(col, emptyList);
			}

			ClearCollection("_primaryMenuCollection");
			ClearCollection("_memoriesInventoryCollection");
			ClearCollection("_lettersInventoryCollection");
			ClearCollection("_filesInventoryCollection");
			ClearCollection("_questInventory");
			ClearCollection("_bestiaryInventory");
			ClearCollection("_featsInventory");

			var menuList = new object[0].ToList(MenuEntryType);

			void AddEntry(string label, Action onSelect, string description)
			{
				int idx = ((IList)menuList).Count;
				var entry = MenuEntry.Create(label, _ => { onSelect(); BuildMenu(gameScreen, idx); });
				entry.IsCenterAligned = false;
				entry.DoesDrawLargeShadow = false;
				entry.Description = description;
				((IList)menuList).Add(entry.AsTimeSpinnerMenuEntry());
			}

			AddEntry(GetCutsceneLabel(), () => { QoLSettings.Current.AutoSkipCutscenes = !QoLSettings.Current.AutoSkipCutscenes; QoLSettings.Save(); }, "Automatically skips cutscenes without any button press.");
			AddEntry(GetDialogueLabel(), () => { QoLSettings.Current.AutoSkipDialogue = !QoLSettings.Current.AutoSkipDialogue; QoLSettings.Save(); }, "Dialogue boxes advance instantly without input.");
			AddEntry(GetStackCapLabel(), () => { var steps = new[] { 9, 25, 50, 99 }; var idx2 = Array.IndexOf(steps, QoLSettings.Current.StackCap); QoLSettings.Current.StackCap = steps[(idx2 + 1) % steps.Length]; QoLSettings.Save(); }, "Maximum stack size for consumable items. Click to cycle: 9, 25, 50, 99. Shop cap mirrors this value (restart required for shop to apply).");
			AddEntry(GetFastToastLabel(), () => { QoLSettings.Current.FastToastPopups = !QoLSettings.Current.FastToastPopups; QoLSettings.Save(); }, "Fast: toasts appear and disappear instantly. Vanilla: original timing.");
			AddEntry(GetToastBlockLabel(), () => { QoLSettings.Current.ToastsBlockMovement = !QoLSettings.Current.ToastsBlockMovement; QoLSettings.Save(); }, "On: toasts freeze movement (vanilla). Off: toasts never block movement.");

			// Put entries in _questInventory (right panel)
			var questCol = AccessTools.Field(JournalMenuType, "_questInventory").GetValue(gameScreen);
			AccessTools.Field(questCol.GetType(), "_entries").SetValue(questCol, menuList);
			AccessTools.Method(JournalMenuType, "ChangeMenuCollection").Invoke(gameScreen, new object[] { questCol, false });
			AccessTools.Method(questCol.GetType(), "SetSelectedIndex").Invoke(questCol, new object[] { selectedIndex });
		}

		static string GetCutsceneLabel() =>
			$"Auto Skip Cutscenes: {(QoLSettings.Current.AutoSkipCutscenes ? "On" : "Off")}";

		static string GetDialogueLabel() =>
			$"Auto Skip Dialogue: {(QoLSettings.Current.AutoSkipDialogue ? "On" : "Off")}";

		static string GetStackCapLabel() =>
			$"Stack Cap: {QoLSettings.Current.StackCap}";

		static string GetFastToastLabel() =>
			$"Toast Popup Speed: {(QoLSettings.Current.FastToastPopups ? "Fast" : "Vanilla")}";

		static string GetToastBlockLabel() =>
			$"Toasts Block Movement: {(QoLSettings.Current.ToastsBlockMovement ? "On" : "Off")}";
	}
}