using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
			var saveFile = (GameSave)Reflected.SaveFile;
			var seed = saveFile.GetSeed();
			var fillingMethod = saveFile.GetFillingMethod();

			Console.Out.WriteLine($"Seed: {seed}");

			ItemLocations = Randomizer.Randomize(seed, fillingMethod);
			ItemLocations.BaseOnSave(Level.GameSave);

			LevelReflected._random = new DeRandomizer(LevelReflected._random, seed);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			LevelObject.Update(Level, ItemLocations, IsRoomChanged());

#if DEBUG
			if (input.IsNewButtonPress(Buttons.DPadLeft, PlayerIndex.One, out _))
			{
				Level.RequestChangeRoom(new LevelChangeRequest
				{
					LevelID = 1,
					RoomID = 1,
				});
			}
			if (input.IsNewButtonPress(Buttons.DPadRight, PlayerIndex.One, out _))
			{
				Level.RequestChangeRoom(new LevelChangeRequest
				{
					LevelID = Level.ID + 1,
					RoomID = 1,
				});
			}
#endif
		}

		public override void Draw(GCM gcm, SpriteBatch spriteBatch, SpriteFont menuFont)
		{
#if DEBUG
			var levelId = LevelReflected._id;
			var text = $"Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);

				LevelObject.Draw(spriteBatch, menuFont, Level.LevelRenderCenter, ItemLocations);
			}
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
