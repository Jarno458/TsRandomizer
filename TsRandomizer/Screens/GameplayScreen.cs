using System;
using System.Reflection;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Commands;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;
using TsRandomizer.LevelObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Settings;

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

		public Seed Seed { get; private set; }
		public SettingCollection Settings { get; private set; }

		public GameSave Save => (GameSave)Dynamic.SaveFile;
		Level Level => (Level)Dynamic._level;
		dynamic LevelReflected => Level.AsDynamic();

		public ItemLocationMap ItemLocations { get; private set; }

		public GCM GameContentManager { get; private set; }

		public DeathLinker deathLinkService;

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap _, GCM gameContentManager)
		{
			GameContentManager = gameContentManager;

			var saveFile = Save;
			var saveFileSeed = saveFile.GetSeed();
			var fillingMethod = saveFile.GetFillingMethod();
			var settings = saveFile.GetSettings();

			ScreenManager.Log.SetSettings(settings);
			gameContentManager.UpdateMinimapColors(settings);

			Seed = saveFileSeed ?? Seed.Zero;

			Console.Out.WriteLine($"Seed: {Seed}");

			Settings = settings;

			try
			{
				ItemLocations = Randomizer.Randomize(Seed, fillingMethod, Level.GameSave);
			}
			catch (ConnectionFailedException e)
			{
				SendBackToMainMenu(e.Message);
				return;
			}

			ItemLocations.Initialize(Level.GameSave);

			ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(ItemLocations));

			ItemManipulator.Initialize(ItemLocations);

			if (settings.DamageRando.Value != "Off")
				OrbDamageManager.PopulateOrbLookups(Level.GameSave, settings.DamageRando.Value, settings.DamageRandoOverrides.Value);

			BestiaryManager.UpdateBestiary(Level, settings);
			if (!saveFile.GetSaveBool("IsFightingBoss"))
				BestiaryManager.RefreshBossSaveFlags(Level);

			if (Seed.Options.Archipelago)
			{
				Client.SetStatus(ArchipelagoClientState.ClientPlaying);

				var service = Client.GetDeathLinkService();
				deathLinkService = new DeathLinker(settings, service);
				ScreenManager.Console.AddCommand(new ToggleDeathLinkCommand(service, () => Level));
			}

#if DEBUG
			ScreenManager.Console.AddCommand(new TeleportCommand(() => Level));
			ScreenManager.Console.AddCommand(new GiveRelicCommand(() => Level));
			ScreenManager.Console.AddCommand(new GiveOrbCommand(() => Level));
			ScreenManager.Console.AddCommand(new GiveFamiliarCommand(() => Level));
#endif
		}

		void SendBackToMainMenu(string message)
		{
			ScreenManager.Jukebox.FadeOutSong(0.5f);

			var titleBackgroundScreen = (GameScreen)TitleBackgroundScreenType.CreateInstance(false, true);

			LoadingScreenLoadMethod.InvokeStatic(ScreenManager, false, new PlayerIndex?(), new[] { titleBackgroundScreen });

			var messageBox = MessageBox.Create(ScreenManager, message);

			ScreenManager.AddScreen(messageBox.Screen, null);
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			if (ItemLocations == null)
				return;

			LevelObject.Update(Level, this, ItemLocations, IsRoomChanged(), Seed, Settings, ScreenManager);

			FamiliarManager.Update(Level);

			deathLinkService?.Update(Level, ScreenManager);

#if DEBUG
			TimespinnerAfterDark(input);
#endif
		}

#if DEBUG
		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{

			if (ItemLocations == null || currentRoom == null)
				return;

			var levelId = LevelReflected._id;
			var text = $"Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			using (spriteBatch.BeginUsing())
				spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);
		}
#endif

		public void HideItemPickupBar() => ((object)Dynamic._itemGetBanner).AsDynamic()._displayTimer = 3f;

		//public void ChangeItemPickupBar(string name) => ((object)Dynamic._itemGetBanner).AsDynamic()._itemName = name;

		public void ShowItemPickupBar(string name)
		{
			var itemBanner = ((object)Dynamic._itemGetBanner).AsDynamic();

			itemBanner._viewButton = null; // Wont allow you to open menu
			itemBanner.IsMapReveal = false;
			itemBanner.ItemCategory = EInventoryCategoryType.UseItem;
			itemBanner._itemName = name;
			itemBanner._displayTimer = 0f;
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
