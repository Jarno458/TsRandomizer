using System;
using Archipelago.MultiClient.Net;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Archipelago;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation.ItemPlacers;
using TsRandomizer.Screens;
using TsRandomizer.Settings;
using ScreenManager = TsRandomizer.Screens.ScreenManager;

namespace TsRandomizer.Commands
{
	class ConnectCommand : ConsoleCommand
	{
		static readonly Type GameDifficultyMenuType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu");
		static readonly Type LoadingScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.LoadingScreen");
		static readonly Type GamePlayScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");

		public override string Command => "connect";
		public override string ParameterUsage => "<server>:<port> <username> <password?>";

		readonly ScreenManager screenManager;

		public static bool IsWaitingForDifficulty;
		public static Seed Seed;
		public static SettingCollection Settings;
		public static Action<GameSave> OnDifficultySelectedHook;

		public ConnectCommand(ScreenManager screenManager)
		{
			this.screenManager = screenManager;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			string user;
			string password;

			if (parameters.Length == 2)
			{
				user = parameters[1];
				password = null;
			}
			else if (parameters.Length == 2)
			{
				user = parameters[1];
				password = parameters[2];
			}
			else
			{
				return false;
			}

			var saveFileManager = ((object)screenManager.AsDynamic().SaveFileManager).AsDynamic();
			if (saveFileManager.AreSaveFilesFull())
			{
				console.AddLine("No free save slots found", Color.Yellow);
				return true;
			}

			string server;
			string port;

			var serverUriParts = parameters[0].Split(':');
			if (serverUriParts.Length == 1)
			{
				server = serverUriParts[0];
				port = "38281";
			}
			else if (serverUriParts.Length == 2)
			{
				server = serverUriParts[0];
				port = serverUriParts[1];
			}
			else
			{
				return false;
			}

			var serverUri = new Uri($"ws://{server}:{port}");

			var connectionResult = Client.Connect(serverUri.ToString(), user, password);

			if (!connectionResult.Successful)
			{
				console.AddLine($"Connection Failed: {string.Join(", ", ((LoginFailure)connectionResult).Errors)}", Color.Yellow);
				return true;
			}

			console.AddLine("Connected!");

			var connected = (LoginSuccessful)connectionResult;
			var slotDataParser = new SlotDataParser(connected.SlotData, Client.SeedString);

			IsWaitingForDifficulty = true;
			Seed = slotDataParser.GetSeed();
			Settings = slotDataParser.GetSettings();
			OnDifficultySelectedHook = saveGame => {
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveServerKey] = serverUri.ToString();
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveUserKey] = user;
				if (string.IsNullOrEmpty(password))
					saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePasswordKey] = password;
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveConnectionId] = Client.ConnectionId;
				saveGame.DataKeyInts[ArchipelagoItemLocationMap.GameItemIndex] = 0;
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePyramidsKeysUnlock] =
					slotDataParser.GetPyramidKeysGate().ToString();
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSavePersonalItemIds] =
					JsonConvert.SerializeObject(slotDataParser.GetPersonalItems());
			};

			object safeFileManager = screenManager.AsDynamic().SaveFileManager;

			var gameDifficultyMenu =
				(GameScreen)GameDifficultyMenuType.CreateInstance(false, safeFileManager, (Action<GameSave.EGameDifficultyType>)OnDifficultySelected);

			screenManager.AddScreen(gameDifficultyMenu, PlayerIndex.One);

			console.Close();

			return true;
		}

		void OnDifficultySelected(GameSave.EGameDifficultyType difficulty)
		{
			var saveFileManager = ((object)screenManager.AsDynamic().SaveFileManager).AsDynamic();
			int saveFileIndex = saveFileManager.GetNextSaveIndex();
			var save = GameSave.CreateNewSave(saveFileIndex, difficulty);

			GameConfigSave configSave = saveFileManager.ConfigSave;

			var gameplayScreen = (GameScreen)GamePlayScreenType.CreateInstance(false, save, configSave);

			var loadMethod = LoadingScreenType.GetPublicStaticMethod("Load");
			loadMethod.InvokeStatic(screenManager, true, PlayerIndex.One, new[] { gameplayScreen });
		}
	}
}
