using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
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
	partial class GameplayScreen : Screen
	{
		RoomSpecification currentRoom;
		readonly ItemLocationMap itemLocations;

		Level Level => (Level)ScreenReflected._level;
		dynamic LevelReflected => Level.Reflect();

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			itemLocations = ItemLocationMap.FromSaveFile(ScreenReflected.SaveFile);
		}

		public override void Update(InputState input)
		{
			//if (input.IsButtonHold(Buttons.RightStick, PlayerIndex.One, out PlayerIndex playerIndex))
			//    LoadNextLevel();

			LevelObject.UpdateAll();

			if (IsRoomChanged())
				LevelObject.OnChangeRoom(itemLocations, Level);

			var newObjects = (List<Mobile>)LevelReflected._newObjects;
			if (newObjects.Any())
				LevelObject.RandomiseObjects(itemLocations, newObjects);
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
