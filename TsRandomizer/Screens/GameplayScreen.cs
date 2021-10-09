using System;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.LevelObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen")]
	// ReSharper disable once UnusedMember.Global
	class GameplayScreen : Screen
	{
		static readonly MethodInfo LoadingScreenLoadMethod = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.BaseClasses.LoadingScreen")
			.GetPublicStaticMethod("Load");

		static readonly Type TitleBackgroundScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.MainMenu.TitleBackgroundScreen");

		RoomSpecification currentRoom;
		SeedOptions seedOptions;

		Level Level => (Level)Dynamic._level;
		dynamic LevelReflected => Level.AsDynamic();

		public ItemLocationMap ItemLocations { get; private set; }

		public GCM GameContentManager { get; private set; }

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap _, GCM gameContentManager)
		{
			GameContentManager = gameContentManager;

			var saveFile = (GameSave)Dynamic.SaveFile;
			var seed = saveFile.GetSeed();
			var fillingMethod = saveFile.GetFillingMethod();

			if(!seed.HasValue)
				seed = Seed.Zero;

			Console.Out.WriteLine($"Seed: {seed}");

			seedOptions = seed.Value.Options;

			try
			{
				ItemLocations = Randomizer.Randomize(seed.Value, fillingMethod, Level.GameSave);
			}
			catch (ConnectionFailedException e)
			{
				SendBackToMainMenu(e.Message);
				return;
			}

			ItemLocations.Initialize(Level.GameSave);

			ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(ItemLocations));

			LevelReflected._random = new DeRandomizer(LevelReflected._random, seed.Value);

			ItemManipulator.Initialize(ItemLocations);

			if(fillingMethod == FillingMethod.Archipelago)
				Client.SetStatus(ArchipelagoClientState.ClientPlaying);
		}

		void SendBackToMainMenu(string message)
		{
			ScreenManager.Jukebox.FadeOutSong(0.5f);

			var titleBackgroundScreen = (GameScreen)TitleBackgroundScreenType.CreateInstance(false, true);

			LoadingScreenLoadMethod.InvokeStatic(ScreenManager, false, new PlayerIndex?(), new [] { titleBackgroundScreen });

			var messageBox = MessageBox.Create(ScreenManager, message);

			ScreenManager.AddScreen(messageBox.Screen, null);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (ItemLocations == null)
				return;

			LevelObject.Update(Level, this, ItemLocations, IsRoomChanged(), seedOptions, ScreenManager);

			FamiliarManager.Update(Level);

#if DEBUG
			TimespinnerAfterDark(input);
#endif
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (ItemLocations == null || currentRoom == null)
				return;
#if DEBUG
			var levelId = LevelReflected._id;
			var text = $"Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
				spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);
#endif
		}

		public void HideItemPickupBar()
		{
			((object)Dynamic._itemGetBanner).AsDynamic()._displayTimer = 3;
		}

		bool IsRoomChanged()
		{
			if (currentRoom != null && LevelReflected.CurrentRoom == currentRoom) return false;

			currentRoom = LevelReflected.CurrentRoom;

			ExceptionLogger.SetLevelContext(Level.ID, currentRoom.ID);

			return true;
		}

#if DEBUG
		void TimespinnerAfterDark(InputState input)
		{
			if (input.IsNewButtonPress(Buttons.DPadLeft))
				Level.RequestChangeLevel(new LevelChangeRequest { LevelID = Math.Max(Level.ID - 1, 0), RoomID = 0 });
			if (input.IsNewButtonPress(Buttons.DPadRight))
				Level.RequestChangeLevel(new LevelChangeRequest { LevelID = Level.ID + 1, RoomID = 0 });
			if (input.IsNewButtonPress(Buttons.DPadDown))
				Level.RequestChangeRoom(new LevelChangeRequest { LevelID = Level.ID, RoomID = Math.Max(Level.RoomID - 1, 0) });
			if (input.IsNewButtonPress(Buttons.DPadUp))
				Level.RequestChangeRoom(new LevelChangeRequest { LevelID = Level.ID, RoomID = Math.Min(Level.RoomID + 1, Level.TotalRooms - 1) });
		}
#endif
	}
}
