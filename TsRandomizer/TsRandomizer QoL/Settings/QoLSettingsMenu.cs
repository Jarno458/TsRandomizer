using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer
{
	public static class QoLSettingsMenu
	{
		static readonly Type JournalMenuType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.PauseMenu.JournalMenuScreen");
		static readonly Type MenuEntryType =
			AccessTools.TypeByName("Timespinner.GameStateManagement.MenuEntry");

		public static object PendingScreenInstance { get; set; }

		public static GameScreen Create(ScreenManager screenManager)
		{
			var gcm = (GCM)screenManager.AsDynamic().GCM;
			gcm.LoadAllResources(screenManager.AsDynamic().GeneralContentManager, screenManager.GraphicsDevice);

			void OnExit() { }

			var screen = (GameScreen)Activator.CreateInstance(JournalMenuType, GameSave.DemoSave, gcm, (Action)OnExit);
			PendingScreenInstance = screen;
			return screen;
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
			AccessTools.Method(JournalMenuType, "LoadContent");

		static void Postfix(object __instance)
		{
			if (QoLSettingsMenu.PendingScreenInstance != __instance)
				return;

			QoLSettingsMenu.PendingScreenInstance = null;

			try
			{
				var d = __instance.AsDynamic();

				d._menuTitle = "TsRandomizer QoL";

				// Clear all collections
				var emptyList = new object[0].ToList(MenuEntryType);
				((object)d._memoriesInventoryCollection).AsDynamic()._entries = emptyList;
				((object)d._lettersInventoryCollection).AsDynamic()._entries = emptyList;
				((object)d._filesInventoryCollection).AsDynamic()._entries = emptyList;
				((object)d._questInventory).AsDynamic()._entries = emptyList;
				((object)d._bestiaryInventory).AsDynamic()._entries = emptyList;
				((object)d._featsInventory).AsDynamic()._entries = emptyList;

				// Build main menu entries
				var menuList = new object[0].ToList(MenuEntryType);

				// Auto Skip Cutscenes toggle
				var cutsceneEntry = MenuEntry.Create(
					GetCutsceneLabel(),
					_ => ToggleCutscenes(__instance));
				cutsceneEntry.IsCenterAligned = false;
				cutsceneEntry.DoesDrawLargeShadow = false;
				cutsceneEntry.Description = "Automatically skips cutscenes without any button press.";
				menuList.Add(cutsceneEntry.AsTimeSpinnerMenuEntry());

				// Auto Skip Dialogue toggle
				var dialogueEntry = MenuEntry.Create(
					GetDialogueLabel(),
					_ => ToggleDialogue(__instance));
				dialogueEntry.IsCenterAligned = false;
				dialogueEntry.DoesDrawLargeShadow = false;
				dialogueEntry.Description = "Dialogue boxes advance instantly without input.";
				menuList.Add(dialogueEntry.AsTimeSpinnerMenuEntry());

				// Stack Cap
				var stackEntry = MenuEntry.Create(
					GetStackCapLabel(),
					_ => CycleStackCap(__instance));
				stackEntry.IsCenterAligned = false;
				stackEntry.DoesDrawLargeShadow = false;
				stackEntry.Description = "Maximum stack size for consumable items. Click to cycle: 9, 25, 50, 99. Shop cap mirrors this value (restart required for shop to apply).";
				menuList.Add(stackEntry.AsTimeSpinnerMenuEntry());

				// Toast Popup Speed toggle
				var fastToastEntry = MenuEntry.Create(
					GetFastToastLabel(),
					_ => ToggleFastToasts(__instance));
				fastToastEntry.IsCenterAligned = false;
				fastToastEntry.DoesDrawLargeShadow = false;
				fastToastEntry.Description = "Fast: toasts appear and disappear instantly. Vanilla: original timing.";
				menuList.Add(fastToastEntry.AsTimeSpinnerMenuEntry());

				// Toasts Block Movement toggle
				var toastBlockEntry = MenuEntry.Create(
					GetToastBlockLabel(),
					_ => ToggleToastBlock(__instance));
				toastBlockEntry.IsCenterAligned = false;
				toastBlockEntry.DoesDrawLargeShadow = false;
				toastBlockEntry.Description = "On: toasts freeze movement (vanilla). Off: toasts never block movement.";
				menuList.Add(toastBlockEntry.AsTimeSpinnerMenuEntry());

				((object)d._primaryMenuCollection).AsDynamic()._entries = menuList;
				((object)d._selectedMenuCollection).AsDynamic().SetSelectedIndex(0);
			}
			catch
			{
				// ignored
			}
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

		static void ToggleCutscenes(object __instance)
		{
			QoLSettings.Current.AutoSkipCutscenes = !QoLSettings.Current.AutoSkipCutscenes;
			QoLSettings.Save();
			RefreshEntry(__instance, 0, GetCutsceneLabel());
		}

		static void ToggleDialogue(object __instance)
		{
			QoLSettings.Current.AutoSkipDialogue = !QoLSettings.Current.AutoSkipDialogue;
			QoLSettings.Save();
			RefreshEntry(__instance, 1, GetDialogueLabel());
		}

		static void CycleStackCap(object __instance)
		{
			var steps = new[] { 9, 25, 50, 99 };
			var current = QoLSettings.Current.StackCap;
			var idx = Array.IndexOf(steps, current);
			QoLSettings.Current.StackCap = steps[(idx + 1) % steps.Length];
			QoLSettings.Save();
			RefreshEntry(__instance, 2, GetStackCapLabel());
		}

		static void ToggleFastToasts(object __instance)
		{
			QoLSettings.Current.FastToastPopups = !QoLSettings.Current.FastToastPopups;
			QoLSettings.Save();
			RefreshEntry(__instance, 3, GetFastToastLabel());
		}

		static void ToggleToastBlock(object __instance)
		{
			QoLSettings.Current.ToastsBlockMovement = !QoLSettings.Current.ToastsBlockMovement;
			QoLSettings.Save();
			RefreshEntry(__instance, 4, GetToastBlockLabel());
		}

		static void RefreshEntry(object __instance, int index, string newLabel)
		{
			try
			{
				var d = __instance.AsDynamic();
				var entries = (IList)((object)d._primaryMenuCollection).AsDynamic()._entries;
				if (index < entries.Count)
					new MenuEntry(entries[index]).Text = newLabel;
			}
			catch
			{
				// ignored
			}
		}
	}
}
