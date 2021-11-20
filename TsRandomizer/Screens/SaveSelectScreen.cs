using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Drawables;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.SaveSelectScreen")]
	// ReSharper disable once UnusedMember.Global
	class SaveSelectScreen : Screen
	{
		const int NumberOfSaveFileSlots = 8;

		readonly Dictionary<object, SeedRepresentation> seedRepresentations = new Dictionary<object, SeedRepresentation>(10);

		int fileToDeleteIndex = -1;
		int zoom;

		public SaveSelectScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			var saveFileEntries = (IList)((object)Dynamic._saveFileCollection).AsDynamic().Entries;

			foreach (var entry in saveFileEntries)
			{
				var entryReflected = entry.AsDynamic();

				if (entryReflected.IsEmptySaveSlot)
					continue;

				var saveFile = (GameSave)entryReflected._saveFile;
				var seed = saveFile.GetSeed();

				seedRepresentations.Add(entry, new SeedRepresentation(seed, screenManager.Reflected.GCM, false));
			}
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (fileToDeleteIndex >= 0)
				HandleFileDeletion();
			else
				HandleUpdate(input);
		}

		void HandleUpdate(InputState input)
		{
			var saveFileEntries = (IList)((object)Dynamic._saveFileCollection).AsDynamic().Entries;

			if (IsZoomChanged())
			{
				UpdateSeedRepresentationIconSize(saveFileEntries);
				UpdateAreaNameSize(saveFileEntries);
			}

			UpdateDrawPositions(saveFileEntries);

			var missingEntries = new List<object>();

			foreach (var seedRepresentation in seedRepresentations)
			{
				seedRepresentation.Value.ShowSeedId = false;

				if (!saveFileEntries.Contains(seedRepresentation.Key))
					missingEntries.Add(seedRepresentation.Key);
			}

			foreach (var missingEntry in missingEntries)
				seedRepresentations.Remove(missingEntry);

			UpdateInput(input);
		}

		void HandleFileDeletion()
		{
			var saveFileCollection = ((object) Dynamic._saveFileCollection).AsDynamic();
			var saveFileManager = ((object) Dynamic._saveFileManager).AsDynamic();

			if (!saveFileManager.IsFinishedSaving)
				return;

			var saveFile = GetNextSaveFile(saveFileCollection);
			if (saveFile == null)
			{
				StopDeletion(saveFileCollection);
				return;
			}

			saveFileCollection.SelectedIndex = fileToDeleteIndex;

			saveFileManager.RequestGameSaveDelete(saveFile);
			saveFileCollection.DeleteSelectedFile();
		}

		GameSave GetNextSaveFile(dynamic saveFileCollection)
		{
			while (fileToDeleteIndex < NumberOfSaveFileSlots)
			{
				var saveFile = ((IList) saveFileCollection.SaveFiles)[fileToDeleteIndex].AsDynamic().SaveFile;

				if (saveFile != null)
					return saveFile;

				fileToDeleteIndex++;
			}

			return null;
		}

		void StopDeletion(dynamic saveFileCollection)
		{
			fileToDeleteIndex = -1;
			saveFileCollection.SelectedIndex = 0;
		}

		void UpdateDrawPositions(IList saveFileEntries)
		{
			foreach (var saveFileEntry in saveFileEntries)
			{
				if (!seedRepresentations.ContainsKey(saveFileEntry)) continue;

				var entry = saveFileEntry.AsDynamic();

				if (entry.IsEmptySaveSlot || entry.IsCorrupt)
					continue;

				var drawPosition = (Vector2) entry.DrawPosition;
				var textXOffset = (int) entry._textOffsetX;
				var font = (SpriteFont) entry._font;
				var origin = new Vector2(0.0f, font.LineSpacing / 2f);

				var drawPoint = new Point(
					(int) (drawPosition.X + textXOffset + entry._saveColumnOffsetX - seedRepresentations[saveFileEntry].Width),
					(int) drawPosition.Y);

				seedRepresentations[saveFileEntry].SetDrawPoint(drawPoint, origin);
			}
		}

		object CurrentSelectedMenuEntry =>
			seedRepresentations
				.FirstOrDefault(sr => ((GameSave)sr.Key.AsDynamic().SaveFile).SaveFileIndex == Dynamic.SelectedIndex)
				.Key;

		GameSave CurrentSelectedSave => CurrentSelectedMenuEntry?.AsDynamic().SaveFile;

		void UpdateInput(InputState input)
		{
			if (input.IsButtonHold(Buttons.LeftTrigger))
			{
				UpdateDescription(true);

				DisplaySeedId();

				if (input.IsNewButtonPress(Buttons.X))
				{
					Dynamic._isDeleting = false;

					ShowDeleteAllDialog();
				}
			}
			else
			{
				UpdateDescription(false);
			}

			if (input.IsNewButtonPress(Buttons.RightTrigger))
			{
				var selectedSaveFile = CurrentSelectedSave;
				if(selectedSaveFile == null)
					return;

				ShowSpoilerGenerationDialog(selectedSaveFile);
			}

			if (input.IsControllHold() && input.IsKeyHold(Keys.C))
			{
				var seed = CurrentSelectedSave?.GetSeed();

				if (!seed.HasValue)
					return;

				SDL.SDL_SetClipboardText(seed.Value.ToDisplayString());
			}
		}

		void UpdateDescription(bool displayDeleteAll)
		{
			var x = CurrentSelectedMenuEntry?.AsDynamic();
			if (x == null) return;

			string desc = (x._isCleared)
				? TimeSpinnerGame.Localizer.Get("SaveSelectContClearedDescription")
				: TimeSpinnerGame.Localizer.Get("SaveSelectContinueDescription");

			if (displayDeleteAll)
			{
				var xStringStart = desc.IndexOf("$X", StringComparison.InvariantCulture);
				var xStringEnd = desc.IndexOf("$Y", xStringStart, StringComparison.InvariantCulture);

				var aString = desc.Substring(0, xStringStart + 3);
				var yString = (xStringEnd != -1)
					? desc.Substring(xStringEnd -1)
					: "";

				desc = aString + "to delete all files." + yString;
			}

			x.Description = desc;

			Dynamic.OnSelectedEntryChanged(Dynamic.SelectedIndex);
		}

		void DisplaySeedId()
		{
			var currentEntry = CurrentSelectedMenuEntry;
			if (currentEntry != null 
			   && seedRepresentations.TryGetValue(currentEntry, out var seedRepresentation) 
			   && seedRepresentation != null)
					seedRepresentation.ShowSeedId = true;
		}

		void ShowDeleteAllDialog()
		{
			var messageBox = MessageBox.Create(ScreenManager, "Delete all saves?", _ => OnDeleteAllSavesAccepted());

			ScreenManager.AddScreen(messageBox.Screen, GameScreen.ControllingPlayer);
		}

		void OnDeleteAllSavesAccepted()
		{
			fileToDeleteIndex = 0;
		}

		void ShowSpoilerGenerationDialog(GameSave save)
		{
			var seed = save.GetSeed();

			if (!seed.HasValue)
				return;

			var messageBox = MessageBox.Create(ScreenManager, "Generate Spoiler log?", _ => OnSpoilerLogCreationAccepted(save));

			ScreenManager.AddScreen(messageBox.Screen, GameScreen.ControllingPlayer);
		}

		void OnSpoilerLogCreationAccepted(GameSave save)
		{
			var fillingMethod = save.GetFillingMethod();

			if (fillingMethod == FillingMethod.Archipelago)
			{
				var messageBox = MessageBox.Create(ScreenManager, "Not supported for Archipelago based seed");

				ScreenManager.AddScreen(messageBox.Screen, GameScreen.ControllingPlayer);

				return;
			}

			var seed = save.GetSeed();

			if (!seed.HasValue)
				return;

			using (var file = new StreamWriter(GetFileName(seed.Value)))
			{
				file.WriteLine($"Seed: {seed.Value.ToDisplayString()}");
				file.WriteLine($"Timespinner version: v{TimeSpinnerGame.Constants.GameVersion}");
				file.WriteLine($"TsRandomizer version: v{Assembly.GetExecutingAssembly().GetName().Version}");

				var itemLocations = Randomizer.Randomize(seed.Value, fillingMethod, save);

				var progressionItems = itemLocations.Where(l => l.ItemInfo.Unlocks != Requirement.None);
				var otherItems = itemLocations.Where(l => l.ItemInfo.Unlocks == Requirement.None);

				WriteProgressionChain(file, itemLocations);
				WriteItemListSection(file, progressionItems, "Progression Items:");
				WriteItemListSection(file, otherItems, "Other Locations:");
			}
		}

		static void WriteProgressionChain(StreamWriter file, ItemLocationMap itemLocations)
		{
			file.WriteLine();
			file.WriteLine("Progression Chain:");
			
			int depth = 0;
			var progressionChain = itemLocations.GetProgressionChain();

			do
			{
				WriteItemList(file, progressionChain.Locations, depth);

				depth++;
				progressionChain = progressionChain.Sub;
			} while (progressionChain != null);
		}

		static void WriteItemListSection(StreamWriter file, IEnumerable<ItemLocation> itemLocations, string sectionName)
		{
			file.WriteLine();
			file.WriteLine(sectionName);

			WriteItemList(file, itemLocations, 0);
		}

		static void WriteItemList(StreamWriter file, IEnumerable<ItemLocation> itemLocations, int depth)
		{
			var prefix = new string('\t', depth);

			foreach (var itemLocation in itemLocations)
				file.WriteLine(prefix + itemLocation);
		}

		static string GetFileName(Seed seed)
		{
			var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fileDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm");

			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(directory, $"SpoilerLog {seed} {fileDateTime}.txt");
		}

		bool IsZoomChanged()
		{
			var newZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			if (zoom == newZoom) return false;

			zoom = newZoom;

			return true;
		}

		void UpdateSeedRepresentationIconSize(IList saveFileEntries)
		{
			foreach (var saveFileEntry in saveFileEntries)
			{
				if(!seedRepresentations.ContainsKey(saveFileEntry)) continue;

				var entry = saveFileEntry.AsDynamic();
				var font = (SpriteFont)entry._font;

				seedRepresentations[saveFileEntry].IconSize = (int)(font.LineSpacing * zoom * 0.75);
			}
		}

		void UpdateAreaNameSize(IList saveFileEntries)
		{
			foreach (var saveFileEntry in saveFileEntries)
			{
				if (!seedRepresentations.ContainsKey(saveFileEntry)) continue;

				var entry = saveFileEntry.AsDynamic();
				var scrollableTextBlock = ((object)entry._areaNameTextBlock).AsDynamic();

				scrollableTextBlock._baseWidth = 
					entry._leftColumnWidth - ((int)seedRepresentations[saveFileEntry].Width / zoom);

				scrollableTextBlock.MeasureString();
			}
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (!GameScreen.IsActive)
				return;

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
				foreach (var seedRepresentation in seedRepresentations)
					if (!seedRepresentation.Key.AsDynamic().IsScrolledOff)
						seedRepresentation.Value.Draw(spriteBatch);
		}
	}
}