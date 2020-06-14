using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
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

		Level Level => (Level)Reflected._level;
		dynamic LevelReflected => Level.AsDynamic();

		public ItemLocationMap ItemLocations { get; private set; }

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap)
		{
			var seed = GetSeed(Reflected.SaveFile);
			var levelReflected = Level.AsDynamic();

			levelReflected._random = new DeRandomizer(levelReflected._random, seed);
			ItemLocations = new ItemLocationMap(Level.GameSave);

			new ItemLocationRandomizer(seed).AddRandomItemsToLocationMap(ItemLocations);
		}

		Seed GetSeed(GameSave saveFile)
		{
			var seed = saveFile.FindSeed() ?? Seed.Current;

			Console.Out.WriteLine($"Seed: {seed}");

			saveFile.SetSeed(seed);
			Seed.Current = seed;

			return seed;
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			LevelObject.Update(Level, ItemLocations, IsRoomChanged());
		}

		public override void Draw(GCM gcm, SpriteBatch spriteBatch, SpriteFont menuFont)
		{
#if DEBUG
			var levelId = LevelReflected._id;
			var text = $"Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
			spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);

			LevelObject.Draw(spriteBatch, menuFont, Level.LevelRenderCenter, ItemLocations);

			spriteBatch.End();
#endif
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
