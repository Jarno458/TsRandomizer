using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Drawables;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.MainMenu.SaveSelectScreen")]
	// ReSharper disable once UnusedMember.Global
	class SaveSelectScreen : Screen
	{
		readonly dynamic reflected;
		readonly Dictionary<object, SeedRepresentation> seedRepresentations = new Dictionary<object, SeedRepresentation>(10);

		public SaveSelectScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			reflected = screen.Reflect();

			var saveFileEntries = (IList)((object)reflected._saveFileCollection).Reflect().Entries;

			foreach (var entry in saveFileEntries)
			{
				var entryReflected = entry.Reflect();

				if (entryReflected.IsEmptySaveSlot)
					continue;

				var saveFile = (GameSave)entryReflected._saveFile;
				var seed = saveFile.FindSeed();

				seedRepresentations.Add(entry, new SeedRepresentation(seed, screenManager.Reflected.GCM, false));
			}
		}

		public override void Update(InputState input)
		{
			var saveFileEntries = (IList)((object)reflected._saveFileCollection).Reflect().Entries;

			foreach (var saveFileEntry in saveFileEntries)
			{
				var saveFileReflected = saveFileEntry.Reflect();

				if(saveFileReflected.IsEmptySaveSlot)
					continue;

				var saveFile = (GameSave)saveFileReflected._saveFile;
				var areaName = saveFile.GetAreaName();

				saveFileReflected._areaName = $"{new string(' ', 12)}{areaName}";
			}
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			base.Draw(spriteBatch, menuFont);

			if (!GameScreen.IsActive)
				return;

			var zoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var saveMenuCollection = ((object) ScreenReflected._saveFileCollection).Reflect();
			
			IList saveFileEntries = saveMenuCollection.Entries;

			foreach (object entry in saveFileEntries)
			{
				var reflectedEntry = entry.Reflect();

				if (reflectedEntry.IsEmptySaveSlot)
					continue;

				var drawPosition = (Vector2)reflectedEntry.DrawPosition;
				var textXOffset = (int)reflectedEntry._textOffsetX;
				var font = (SpriteFont)reflectedEntry._font;
				var origin = new Vector2(0.0f, font.LineSpacing / 2f);

				seedRepresentations[entry].IconSize = (int)(font.LineSpacing * zoom * 0.75);
				seedRepresentations[entry].SetDrawPoint(new Point((int)(drawPosition.X + textXOffset), (int)drawPosition.Y), origin);
			}

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

			foreach (var seedRepresentation in seedRepresentations.Values)
				seedRepresentation.Draw(spriteBatch);

			spriteBatch.End();
		}
	}
}