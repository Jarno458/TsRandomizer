using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.LevelObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen")]
	// ReSharper disable once UnusedMember.Global
	class GameplayScreen : Screen
	{
		RoomSpecification currentRoom;
		readonly ItemLocationMap itemLocations;

		Level Level => (Level)ScreenReflected._level;
		dynamic LevelReflected => Level.Reflect();

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			var seed = GetSeed(ScreenReflected.SaveFile);
			var levelReflected = Level.Reflect();
			levelReflected._random = new DeRandomizer(levelReflected._random, seed);
			itemLocations = ItemLocationMap.FromSeed(seed);
		}

		Seed GetSeed(GameSave saveFile)
		{
			var seed = saveFile.FindSeed() ?? Seed.Current;

			saveFile.SetSeed(seed);
			Seed.Current = seed;

			return seed;
		}

		public override void Update(InputState input)
		{
			LevelObject.UpdateAll();

			if (IsRoomChanged())
				LevelObject.OnChangeRoom(itemLocations, Level);

			var newObjects = (List<Mobile>)LevelReflected._newObjects;
			if (newObjects.Any())
				LevelObject.GenerateShadowObjects(itemLocations, newObjects);
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			var levelId = LevelReflected._id;
			var text = $"Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);

			LevelObject.DrawAll(spriteBatch, menuFont, Level.LevelRenderCenter, itemLocations);

			spriteBatch.End();
		}

		bool IsRoomChanged()
		{
			if (currentRoom == null || LevelReflected.CurrentRoom != currentRoom)
			{
				currentRoom = LevelReflected.CurrentRoom;
				return true;
			}

			return false;
		}
	}
}
