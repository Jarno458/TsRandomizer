using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
		readonly Dictionary<object, SeedRepresentation> seedRepresentations = new Dictionary<object, SeedRepresentation>(10);

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
			var saveFileEntries = (IList)((object)Dynamic._saveFileCollection).AsDynamic().Entries;

			if(IsZoomChanged())
			{
				UpdateSeedRepresentationIconSize(saveFileEntries);
				UpdateAreaNameSize(saveFileEntries);
			}

			UpdateDrawPositions(saveFileEntries);

			var missingEntries = new List<object>();

			foreach (var seedRepresentation in seedRepresentations)
			{
				seedRepresentation.Value.ShowSeedId = false;

				if(!saveFileEntries.Contains(seedRepresentation.Key))
					missingEntries.Add(seedRepresentation.Key);
			}
			
			foreach (var missingEntry in missingEntries)
				seedRepresentations.Remove(missingEntry);

			UpdateInput(input);
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

		void UpdateInput(InputState input)
		{
			var selectedIndex = Dynamic.SelectedIndex;

			if (input.IsButtonHold(Buttons.LeftTrigger, null, out _))
			{
				var seedRepresentation = seedRepresentations
					.FirstOrDefault(sr => ((GameSave)sr.Key.AsDynamic().SaveFile).SaveFileIndex == selectedIndex).Value;

				if (seedRepresentation != null)
					seedRepresentation.ShowSeedId = true;
			}
			else if (input.IsNewButtonPress(Buttons.RightTrigger, null, out _))
			{
				var selectedSaveFile = seedRepresentations
					.Select(sr => (GameSave)sr.Key.AsDynamic().SaveFile)
					.FirstOrDefault(save => save.SaveFileIndex == selectedIndex);

				if(selectedSaveFile == null)
					return;

				ShowSpoilerGenerationDialog(selectedSaveFile);
			}
		}

		void ShowSpoilerGenerationDialog(GameSave save)
		{
			var seed = save.GetSeed();

			if (!seed.HasValue)
				return;

			var messageBox = MessageBox.Create(ScreenManager, "Generate Spoiler log?", (pi) => OnSpoilerLogCreationAccepted(save));

			ScreenManager.AddScreen(messageBox.Screen, GameScreen.ControllingPlayer);
		}

		void OnSpoilerLogCreationAccepted(GameSave save)
		{
			var seed = save.GetSeed();

			if (!seed.HasValue)
				return;

			using (var file = new StreamWriter(GetFileName(seed.Value)))
			{
				file.WriteLine($"Seed: {seed}");
				file.WriteLine($"Timespinner version: v{TimeSpinnerGame.Constants.GameVersion}");
				file.WriteLine($"TsRandomizer version: v{Assembly.GetExecutingAssembly().GetName().Version}");

				var itemLocations = Randomizer.Randomize(seed.Value, save.GetFillingMethod());

				var progressionItems = itemLocations.Where(l => l.ItemInfo.Unlocks != Requirement.None);
				var otherItems = itemLocations.Where(l => l.ItemInfo.Unlocks == Requirement.None);

				WriteProgressionChain(file, itemLocations);
				WriteItemListSection(file, progressionItems, "Progression Items:");
				WriteItemListSection(file, otherItems, "Other Locations:");
			}
		}

		void WriteProgressionChain(StreamWriter file, ItemLocationMap itemLocations)
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

		void WriteItemListSection(StreamWriter file, IEnumerable<ItemLocation> itemLocations, string sectionName)
		{
			file.WriteLine();
			file.WriteLine(sectionName);

			WriteItemList(file, itemLocations, 0);
		}

		void WriteItemList(StreamWriter file, IEnumerable<ItemLocation> itemLocations, int depth)
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