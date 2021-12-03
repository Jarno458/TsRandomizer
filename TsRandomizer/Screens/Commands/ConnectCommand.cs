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

namespace TsRandomizer.Screens.Commands
{
	class ConnectCommand : ConsoleCommand
	{
		static readonly Type GameDifficultyMenuType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.MainMenu.GameDifficultyMenu");
		static readonly Type LoadingScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.BaseClasses.LoadingScreen");
		static readonly Type GamePlayScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen");

		public override string Command => "connect";
		public override string ParameterUsage => "<server>:<port> <username>";

		readonly ScreenManager screenManager;

		public static bool IsWaitingForDifficulty;
		public static Seed Seed;
		public static Action<GameSave> OnDifficultySelectedHook;

		public ConnectCommand(ScreenManager screenManager)
		{
			this.screenManager = screenManager;
		}

		public override bool Handle(GameConsole console, string[] parameters)
		{
			if (parameters.Length < 1 || parameters.Length > 2)
				return false;

			var saveFileManager = ((object)screenManager.AsDynamic().SaveFileManager).AsDynamic();
			if (saveFileManager.AreSaveFilesFull())
			{
				console.AddLine("No free save slots found", Color.Yellow);
				return true;
			}

			string userName;
			if (parameters.Length == 2)
				userName = parameters[1];
			else
			{
				//TODO FIX ME!!!
				userName = "Jarno";
			}

			var serverUriParts = parameters[0].Split(':');
			if (serverUriParts.Length != 2)
				return false;

			var serverUri = new Uri($"ws://{serverUriParts[0]}:{serverUriParts[1]}");

			var connectionResult = Client.Connect(serverUri.ToString(), userName);

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
			OnDifficultySelectedHook = saveGame => {
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveServerKey] = serverUri.ToString();
				saveGame.DataKeyStrings[ArchipelagoItemLocationRandomizer.GameSaveUserKey] = userName;
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

			screenManager.AddScreen(gameDifficultyMenu, null);

			return true;
		}

		void OnDifficultySelected(GameSave.EGameDifficultyType difficulty)
		{
			var saveFileManager = ((object)screenManager.AsDynamic().SaveFileManager).AsDynamic();
			int saveFileIndex = saveFileManager.GetNextSaveIndex();
			GameSave save = GameSave.CreateNewSave(saveFileIndex, difficulty);
			GameConfigSave configSave = saveFileManager.ConfigSave;

			var gameplayScreen = (GameScreen)GamePlayScreenType.CreateInstance(false, save, configSave);

			var loadMethod = LoadingScreenType.GetPublicStaticMethod("Load");
			loadMethod.InvokeStatic(screenManager, true, null, new[] { gameplayScreen });
		}
	}
}
