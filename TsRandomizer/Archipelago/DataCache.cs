using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;

namespace TsRandomizer.Archipelago
{
	class DataCache
	{
		const string ArchipelagoCacheFolder = "ArchipelagoCache";

		static readonly string CacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), ArchipelagoCacheFolder);

		readonly Dictionary<string, GameInfo> gameInfos = new Dictionary<string, GameInfo>();

		readonly ConcurrentDictionary<int, string> itemNames = new ConcurrentDictionary<int, string>();
		readonly ConcurrentDictionary<int, string> locationNames = new ConcurrentDictionary<int, string>();
		readonly ConcurrentDictionary<int, string> playerNames = new ConcurrentDictionary<int, string>();

		public string GetItemName(int itemId) =>
			itemNames.TryGetValue(itemId, out var itemName) ? itemName : $"{itemId}";
		public string GetLocationName(int locationId) =>
			locationNames.TryGetValue(locationId, out var locationName) ? locationName : $"{locationId}";
		public string GetPlayerName(int slotId) =>
			playerNames.TryGetValue(slotId, out var playerName) ? playerName : $"Slot #{slotId}";

		public void LoadCache()
		{
			try
			{
				foreach (var filePath in Directory.EnumerateFiles(CacheFolderPath))
				{
					var gameInfo = TryLoadInfoForFile(filePath);
					var gameName = Path.GetFileNameWithoutExtension(filePath);

					if (gameInfo != null && gameName != null)
						gameInfos[gameName] = gameInfo;
				}
			}
			catch
			{
				// Ignored
			}

			UpdateItemAndLocationLists();
		}

		void UpdateItemAndLocationLists()
		{
			foreach (var gameInfo in gameInfos)
			{
				foreach (var kvp in gameInfo.Value.ItemNames)
					itemNames.TryAdd(kvp.Key, kvp.Value);

				foreach (var kvp in gameInfo.Value.LocationNames)
					locationNames.TryAdd(kvp.Key, kvp.Value);
			}
		}

		static GameInfo TryLoadInfoForFile(string filePath)
		{
			try
			{
				using (var reader = new StreamReader(filePath))
					return new GameInfo(JsonConvert.DeserializeObject<GameData>(reader.ReadToEnd()));
			}
			catch
			{
				return null;
			}
		}

		public void Verify(Dictionary<string, int> versionsPerGame)
		{
			var gamesToExcludeFromUpdate = new List<string>();

			foreach (var kvp in versionsPerGame)
			{
				var game = kvp.Key;
				var gameDataVersion = kvp.Value;

				if(gameInfos.TryGetValue(game, out var gameInfo) && gameInfo.Version == gameDataVersion)
					gamesToExcludeFromUpdate.Add(game);
			}

			if (gamesToExcludeFromUpdate.Count != versionsPerGame.Count)
				Client.RequestGameData(gamesToExcludeFromUpdate);
		}

		public void Update(Dictionary<string, GameData> newState)
		{
			foreach (var kvp in newState)
			{
				var game = kvp.Key;
				var gameData  = kvp.Value;

				gameInfos[game] = new GameInfo(gameData);

				TrySaveCache(game, gameData);
			}

			UpdateItemAndLocationLists();
		}

		public void UpdatePlayerNames(List<NetworkPlayer> players)
		{
			if(players == null)
				return;

			foreach (var player in players)
			{
				playerNames.TryAdd(player.Slot, string.IsNullOrEmpty(player.Alias)
					? player.Name
					: $"{player.Alias} ({player.Name})");
			}
		}

		static void TrySaveCache(string game, GameData gameData)
		{
			try
			{
				if (!Directory.Exists(CacheFolderPath))
					Directory.CreateDirectory(CacheFolderPath);

				using (var writer = new StreamWriter(Path.Combine(CacheFolderPath, $"{game}.json")))
					writer.Write(JsonConvert.SerializeObject(gameData));
			}
			catch
			{
				// ignored
			}
		}
	}

	class GameInfo
	{
		public Dictionary<int, string> ItemNames { get; }
		public Dictionary<int, string> LocationNames { get; }
		public int Version { get; }

		public GameInfo(GameData gameData)
		{
			ItemNames = gameData.ItemLookup.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
			LocationNames = gameData.LocationLookup.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
			Version = gameData.Version;
		}
	}
}
