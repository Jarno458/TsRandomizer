using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.Screens.Menu;
using TsRandomizer.IntermediateObjects;
using ScreenManager = Timespinner.GameStateManagement.ScreenManager.ScreenManager;
using GameScreen = Timespinner.GameStateManagement.ScreenManager.GameScreen;

namespace TsRandomizer.Screens
{
	public static class QoLSettingsMenu
	{
		static readonly Type JournalMenuType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");
		static readonly Type MenuEntryType =
			TimeSpinnerType.Get("Timespinner.GameStateManagement.MenuEntry");

		public static bool IsQoLMenuPending { get; set; }

		internal static GameScreen Create(ScreenManager screenManager)
		{
			GCM gcm = screenManager.AsDynamic().GCM;
			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);

			void OnExit() { }

			return (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.DemoSave, gcm, (Action)OnExit);
		}

		public static void BuildMenu(GameScreen gameScreen, int selectedIndex)
		{
			var d = gameScreen.AsDynamic();
			var emptyList = new object[0].ToList(MenuEntryType);

			d._menuTitle = "QoL Menu";

			((object)d._primaryMenuCollection).AsDynamic()._entries = emptyList;
			((object)d._memoriesInventoryCollection).AsDynamic()._entries = emptyList;
			((object)d._lettersInventoryCollection).AsDynamic()._entries = emptyList;
			((object)d._filesInventoryCollection).AsDynamic()._entries = emptyList;
			((object)d._questInventory).AsDynamic()._entries = emptyList;
			((object)d._bestiaryInventory).AsDynamic()._entries = emptyList;
			((object)d._featsInventory).AsDynamic()._entries = emptyList;

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

			var questCol = ((object)d._questInventory);
			questCol.AsDynamic()._entries = menuList;
			d.ChangeMenuCollection(questCol, false);
			((object)d._selectedMenuCollection).AsDynamic().SetSelectedIndex(selectedIndex);
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