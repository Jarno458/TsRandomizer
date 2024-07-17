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
using Timespinner.GameObjects.BaseClasses;
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

		static readonly MethodInfo RefreshBaseStatsMethod = TimeSpinnerType
			.Get("Timespinner.GameAbstractions.Saving.CharacterStats")
			.GetMethod("RefreshBaseStats", BindingFlags.NonPublic | BindingFlags.Instance);

		static readonly Type TitleBackgroundScreenType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.Screens.MainMenu.TitleBackgroundScreen");

		static readonly Type GamePadWrapper = TimeSpinnerType.Get("Timespinner.GameAbstractions.GamePadWrapper");

		int hpCap;
		int auraCap;
		int levelCap;
		DeathLinker deathLinkService;

		int numberOfGifts;
		DateTime lastBlinkTime = DateTime.Now;

		public Seed Seed { get; private set; }
		public SettingCollection Settings { get; private set; }

		public GameSave Save => (GameSave)Dynamic.SaveFile;
		public Level Level => (Level)Dynamic._level;
		dynamic LevelReflected => Level.AsDynamic();

		public ItemLocationMap ItemLocations { get; private set; }

		public GCM GameContentManager { get; private set; }

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
			gameContentManager.ChangeLabWaterColor();
			SpriteManager.ReloadCustomSprites(Level, GameContentManager, settings);

			Seed = saveFileSeed ?? Seed.Zero;

			ScreenManager.Console.AddLine($"Loading Seed: {Seed}");

			Settings = settings;
			hpCap = Convert.ToInt32(Settings.HpCap.Value);
			auraCap = Convert.ToInt32(Settings.AuraCap.Value);
			levelCap = Convert.ToInt32(settings.LevelCap.Value) - 1;

			try
			{
				ItemLocations = Randomizer.Randomize(Seed, Settings, fillingMethod, Level.GameSave);
			}
			catch (Exception e)
			{
				SendBackToMainMenu(e.Message);
				return;
			}

			ItemLocations.Initialize(Level.GameSave);

			if (Settings.EnableMapFromStart.Value)
				Level.Minimap.SetAllKnown(true);

			ItemTrackerUplink.UpdateState(ItemTrackerState.FromItemLocationMap(ItemLocations));

			ItemManipulator.Initialize(ItemLocations);

			if (settings.DamageRando.Value != "Off")
				OrbDamageManager.PopulateOrbLookups(Level.GameSave, settings.DamageRando.Value, settings.DamageRandoOverrides.Value);

			BestiaryManager.UpdateBestiary(Level, settings);
			if (!saveFile.GetSaveBool("IsFightingBoss"))
				BestiaryManager.RefreshBossSaveFlags(Level);

			if (Seed.Options.Archipelago)
				HandleArchipelago(settings);

#if DEBUG
			saveFile.DataKeyBools["TS_INSTAGIB"] = true;

			ScreenManager.Console.AddCommand(new TeleportCommand(() => Level));
			ScreenManager.Console.AddCommand(new GiveRelicCommand(() => Level, () => ItemLocations));
			ScreenManager.Console.AddCommand(new GiveOrbCommand(() => Level, () => ItemLocations));
			ScreenManager.Console.AddCommand(new GiveFamiliarCommand(() => Level, () => ItemLocations));
			ScreenManager.Console.AddCommand(new InstaGibCommand(() => Level));
#endif
		}

		void HandleLevelCap(GameSave saveFile)
		{
			var stats = saveFile.CharacterStats.AsDynamic();

			stats.MaxLevel = levelCap;

			if (stats.Level > levelCap)
			{
				stats.Level = levelCap;
				RefreshBaseStatsMethod.Invoke(saveFile.CharacterStats, null);
			}
		}

		void HandleArchipelago(SettingCollection settings)
		{
			Client.SetStatus(ArchipelagoClientState.ClientPlaying);

			var service = Client.GetDeathLinkService();

			deathLinkService = new DeathLinker(settings, service);

			ScreenManager.Console.AddCommand(new ToggleDeathLinkCommand(service, () => Level));
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

			LevelObject.Update(Level, this, ItemLocations, Seed, Settings, ScreenManager);

			FamiliarManager.Update(Level);

			deathLinkService?.Update(Level, ScreenManager);

			if (Settings.MeleeAutofire.Value)
				GamePadWrapper.GetProperty("IsMeleeDown").SetValue(Level.MainHero.AsDynamic()._gamePadWrapper, false, null);

			if (Settings.ExtraEarringsXP.Value > 0)
			{
				OrbExperienceManager.UpdateHitRegistry(Level.MainHero);
				OrbExperienceManager.UpdateOrbXp(Level, Level.MainHero, Settings.ExtraEarringsXP.Value);
			}

			HandleLevelCap(Save);

			UpdateGenericScripts(Level);

#if DEBUG
			TimespinnerAfterDark(input);
#endif
			HandleCurrentGifts();
		}

		void UpdateGenericScripts(Level level)
		{
			if (hpCap <= level.MainHero.MaxHP)
				level.MainHero.MaxHP = hpCap;

			if (auraCap <= level.MainHero.Aura)
				level.MainHero.Aura = auraCap;

			if (Settings.DamageRando.Value != "Off")
				OrbDamageManager.UpdateOrbDamage(level.GameSave, level.MainHero);

			if (level.MainHero.CurrentState == EAFSM.Skydashing
				&& level.MainHero.Velocity.Y == 0
				&& level.MainHero.AsDynamic()._isHittingHeadOnCeiling)
				level.GameSave.AddConcussion();
		}

		void HandleCurrentGifts()
		{
			if (!Seed.Options.Archipelago)
				return;

			var currentNumberOfGifts = Client.GetGiftingService().NumberOfGifts;
			if (currentNumberOfGifts > numberOfGifts)
			{
				if (numberOfGifts == 0)
					lastBlinkTime = DateTime.Now;

				ScreenManager.Jukebox.PlayCue(ESFX.CrowCaw);
			}

			numberOfGifts = currentNumberOfGifts;
		}

		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			using (spriteBatch.BeginUsing())
			{
				DrawReceivedGifts(spriteBatch, menuFont);
				DrawRoomId(spriteBatch, menuFont);
			}
		}

		void DrawReceivedGifts(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (!Seed.Options.Archipelago || numberOfGifts == 0)
				return;

			var pauseMenuScreen = ScreenManager.FirstOrDefault<PauseMenuScreen>();
			if (!GameScreen.IsActive && (pauseMenuScreen == null || !pauseMenuScreen.GameScreen.IsActive))
				return;

			var zoom = (int)TimeSpinnerGame.Constants.InGameZoom;
			var PauseMenuTexture = GameContentManager.SpPauseMenu;
			var exclaimationMarkSourceRetangle = new Rectangle(227, 33, 8, 8);

			var gameplayScreenSize = ScreenManager.SmallScreenRect;
			var buttomRight = new Vector2(gameplayScreenSize.X + gameplayScreenSize.Width, gameplayScreenSize.Y + gameplayScreenSize.Height);
			var position = (pauseMenuScreen != null)
				? pauseMenuScreen.GiftingHintPosition
				: new Vector2(buttomRight.X - (zoom * 20), buttomRight.Y - (zoom * 20));
			var textPosition = new Vector2(position.X + (numberOfGifts < 10 ? 5 * zoom : 3 * zoom), position.Y + (2 * zoom));
			var scale = new Vector2(zoom, zoom);
			var iconColor = Color.White;

			var blinkInterval = (int)Settings.GiftingReminderInterval.Value;
			if (blinkInterval != 0 && (DateTime.Now - lastBlinkTime).Seconds >= blinkInterval)
			{
				lastBlinkTime = DateTime.Now;
				iconColor = Color.DarkGray;
				ScreenManager.Jukebox.PlayCue(ESFX.MeyefMeow);
			}

			spriteBatch.Draw(PauseMenuTexture.Texture, position, exclaimationMarkSourceRetangle, iconColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
			spriteBatch.DrawString(menuFont, numberOfGifts.ToString(), textPosition, new Color(240, 240, 208), 0f, Vector2.Zero, scale / 1.5f, SpriteEffects.None, 1);
		}

		void DrawRoomId(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
#if DEBUG
			if (ItemLocations == null)
				return;

			int levelId = LevelReflected._id;
			int roomId = ((RoomSpecification)LevelReflected.CurrentRoom).ID;

			var text = $"Level: {levelId}, Room ID: {roomId}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Red, inGameZoom);
#endif
		}


		public void HideItemPickupBar() => ((object)Dynamic._itemGetBanner).AsDynamic()._displayTimer = 3f;

		public void ChangeItemPickupBar(string name) => ((object)Dynamic._itemGetBanner).AsDynamic()._itemName = name;

		public void ShowItemPickupBar(string name)
		{
			var itemBanner = ((object)Dynamic._itemGetBanner).AsDynamic();

			itemBanner._viewButton = null; // Wont allow you to open menu
			itemBanner.IsMapReveal = false;
			itemBanner.ItemCategory = EInventoryCategoryType.UseItem;
			itemBanner._itemName = name;
			itemBanner._displayTimer = 0f;
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
