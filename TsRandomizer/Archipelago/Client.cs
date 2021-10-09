using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Microsoft.Xna.Framework;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.Archipelago
{
	static class Client
	{
		const int ConnectionTimeoutInSeconds = 6;

		static ArchipelagoSession session;

		static volatile bool hasConnectionResult;
		static ArchipelagoPacketBase connectionResult;

		static volatile bool hasItemLocationInfo;
		static LocationInfoPacket locationScoutResult;

		static DataCache chache = new DataCache();

		static volatile int slot = -1;

		static int playerCount;

		static ConcurrentDictionary<int, ItemIdentifier> receivedItems = new ConcurrentDictionary<int, ItemIdentifier>();

		static string serverUrl;
		static string userName;
		static string password;
		static string uuid;
		static Func<IEnumerable<ItemKey>> getCheckedLocations;

		static ConnectionResult cachedConnectionResult;

		public static bool IsConnected;

		public static Permissions ForfeitPermissions = 0;

		public static ConnectionResult Connect(
			string server, string user, string pass, 
			Func<IEnumerable<ItemKey>> getCheckedLocationsCallback, string connectionId)
		{
			if (IsConnected && session.Connected && cachedConnectionResult != null)
			{
				if (serverUrl == server && userName == user && password == pass)
					return cachedConnectionResult;
				
				Disconnect();
			}

			serverUrl = server;
			userName = user;
			password = pass;
			uuid = connectionId ?? Guid.NewGuid().ToString("N");
			getCheckedLocations = getCheckedLocationsCallback;
			
			session = new ArchipelagoSession(serverUrl);
			session.PacketReceived += PackacedReceived;

			session.ConnectAsync();

			chache.LoadCache();

			hasConnectionResult = false;
			connectionResult = null;

			var connectedStartedTime = DateTime.UtcNow;

			while (!hasConnectionResult)
			{
				if (DateTime.UtcNow - connectedStartedTime > TimeSpan.FromSeconds(ConnectionTimeoutInSeconds))
				{
					Disconnect();

					return new ConnectionFailed("Connection Timedout");
				}

				Thread.Sleep(100);
			}

			if (connectionResult is ConnectionRefusedPacket refused)
			{
				Disconnect();

				return new ConnectionFailed(string.Join(", ", refused.Errors));
			}
			if (connectionResult is ConnectedPacket success)
			{
				IsConnected = true;
				playerCount = success.Players.Count;
				
				cachedConnectionResult = new Connected(success, uuid);
				return cachedConnectionResult;
			}

			Disconnect();

			return new ConnectionFailed("Unknown package, probably due to version missmatch");
		}

		public static void Disconnect()
		{
			session?.DisconnectAsync();

			serverUrl = null;
			userName = null;
			password = null;
			uuid = null;

			slot = -1;

			IsConnected = false;

			chache = new DataCache();
			receivedItems = new ConcurrentDictionary<int, ItemIdentifier>();

			hasConnectionResult = false;
			hasItemLocationInfo = false;

			session = null;

			ForfeitPermissions = 0;

			cachedConnectionResult = null;
		}

		public static ItemIdentifier GetNextItem(int currentIndex)
		{
			return receivedItems.Count > currentIndex 
				? receivedItems[currentIndex + 1] 
				: null;
		}

		public static void SetStatus(ArchipelagoClientState status)
		{
			SendPacket(new StatusUpdatePacket { Status = status });
		}

		static void PackacedReceived(ArchipelagoPacketBase packet)
		{
			switch (packet)
			{
				case RoomInfoPacket roomInfoPacket: OnRoomInfoPacketReceived(roomInfoPacket); break;
				case DataPackagePacket dataPacket: OnDataPackagePacketReceived(dataPacket); break;
				case ConnectionRefusedPacket connectionRefusedPacket: OnConnectionRefusedPacketReceived(connectionRefusedPacket); break;
				case ConnectedPacket connectedPacket: OnConnectedPacketReceived(connectedPacket); break;
				case LocationInfoPacket locationInfoPacket: OnLocationInfoPacketReceived(locationInfoPacket); break;
				case ReceivedItemsPacket receivedItemsPacket: OnReceivedItemsPacketReceived(receivedItemsPacket); break;
				case PrintPacket printPacket: OnPrintPacketReceived(printPacket); break;
				case PrintJsonPacket printJsonPacket: OnPrinJsontPacketReceived(printJsonPacket); break;
			}
		}

		static void SendPacket(ArchipelagoPacketBase packet)
		{
			session?.SendPacket(packet);
		}

		public static void Forfeit()
		{
			SendPacket(new SayPacket { Text = "!forfeit" });
		}

		public static Dictionary<int, int> ScoutLocations(IEnumerable<int> locationIdsToScout)
		{
			SendPacket(new LocationScoutsPacket { Locations = locationIdsToScout.ToList() });

			hasItemLocationInfo = false;
			locationScoutResult = null;

			var connectedStartedTime = DateTime.UtcNow;

			while (!hasItemLocationInfo)
			{
				if (DateTime.UtcNow - connectedStartedTime > TimeSpan.FromSeconds(ConnectionTimeoutInSeconds))
					return null;

				Thread.Sleep(100);
			}

			if (locationScoutResult == null)
				throw new Exception("Failed to retreive personal items");

			var items = new Dictionary<int, int>();

			foreach (var locationInfo in locationScoutResult.Locations)
			{
				if (locationInfo.Player != slot)
					continue;

				items.Add(locationInfo.Location, locationInfo.Item);
			}

			return items;
		}
	
		static void OnRoomInfoPacketReceived(RoomInfoPacket packet)
		{
			chache.UpdatePlayerNames(packet.Players);
		
			if (packet is RoomUpdatePacket)
				return;

			if (packet.Permissions != null && packet.Permissions.TryGetValue("forfeit", out var permissions))
				ForfeitPermissions = permissions;

			chache.Verify(packet.DataPackageVersions);

			var connectionRequest = new ConnectPacket
			{
				Game = "Timespinner",
				Name = userName,
				Password = password,
				Version = new Version(0, 1, 8),
				Uuid = uuid,
				Tags = new List<string>(0)
			};

			session.SendPacket(connectionRequest);
		}

		static void OnConnectedPacketReceived(ConnectedPacket connectedPacket)
		{
			slot = connectedPacket.Slot;

			chache.UpdatePlayerNames(connectedPacket.Players);

			hasConnectionResult = true;
			connectionResult = connectedPacket;
		}

		static void OnConnectionRefusedPacketReceived(ConnectionRefusedPacket connectionRefusedPacket)
		{
			hasConnectionResult = true;
			connectionResult = connectionRefusedPacket;
		}

		public static void RequestGameData(List<string> gamesToExcludeFromUpdate)
		{
			var getGameDataPacket = new GetDataPackagePacket
			{
				Exclusions = gamesToExcludeFromUpdate
			};

			session.SendPacket(getGameDataPacket);
		}

		static void OnDataPackagePacketReceived(DataPackagePacket dataPacket)
		{
			chache.Update(dataPacket.DataPackage.Games);
		}

		static void OnLocationInfoPacketReceived(LocationInfoPacket locationInfoPacket)
		{
			hasItemLocationInfo = true;
			locationScoutResult = locationInfoPacket;
		}

		static void OnReceivedItemsPacketReceived(ReceivedItemsPacket receivedItemsPacket)
		{
			if (receivedItemsPacket.Index != receivedItems.Count)
				ReSync();
			else
				foreach (var item in receivedItemsPacket.Items)
					receivedItems.TryAdd(receivedItems.Count + 1, ItemMap.GetItemIdentifier(item.Item));
		}

		static void ReSync()
		{
			var checkedLocations = getCheckedLocations();
			if (checkedLocations == null)
				return;

			Interlocked.Exchange(ref receivedItems, new ConcurrentDictionary<int, ItemIdentifier>());

			var locationsCheckedPacket = new LocationChecksPacket
			{
				Locations = getCheckedLocations()
					.Select(LocationMap.GetLocationId)
					.ToList()
			};

			session.SendMultiplePackets(new SyncPacket(), locationsCheckedPacket);
		}

		static void OnPrintPacketReceived(PrintPacket printPacket)
		{
			if (printPacket.Text == null)
				return;

			var lines = printPacket.Text.Split('\n');

			foreach (var line in lines)
				ScreenManager.Log.Add(line);
		}

		static void OnPrinJsontPacketReceived(PrintJsonPacket printJsonPacket)
		{
			if (playerCount > 20 && !MessageIsAboutCurrentPlayer(printJsonPacket))
				return;

			var parts = new List<Part>();

			foreach (var messagePart in printJsonPacket.Data)
				parts.Add(new Part(GetMessage(messagePart), GetColor(messagePart)));

			ScreenManager.Log.Add(parts.ToArray());
		}

		static bool MessageIsAboutCurrentPlayer(PrintJsonPacket printJsonPacket)
		{
			return printJsonPacket.Data.Any(
				p => p.Type.HasValue && p.Type == JsonMessagePartType.PlayerId
				     && int.TryParse(p.Text, out var playerId)
				     && playerId == slot);
		}

		static string GetMessage(JsonMessagePart messagePart)
		{
			switch (messagePart.Type)
			{
				case JsonMessagePartType.PlayerId:
					return int.TryParse(messagePart.Text, out var playerSlot) 
						? chache.GetPlayerName(playerSlot)
						: messagePart.Text;
				case JsonMessagePartType.ItemId:
					return int.TryParse(messagePart.Text, out var itemId)
						? chache.GetItemName(itemId)
						: messagePart.Text;
				case JsonMessagePartType.LocationId:
					return int.TryParse(messagePart.Text, out var locationId)
						? chache.GetLocationName(locationId)
						: messagePart.Text;
				default:
					return messagePart.Text;
			}
		}

		static Color GetColor(JsonMessagePart messagePart)
		{
			switch (messagePart.Color)
			{
				case JsonMessagePartColor.Red:
					return Color.Red;
				case JsonMessagePartColor.Green:
					return Color.Green;
				case JsonMessagePartColor.Yellow:
					return Color.Yellow;
				case JsonMessagePartColor.Blue:
					return Color.Blue;
				case JsonMessagePartColor.Magenta:
					return Color.Magenta;
				case JsonMessagePartColor.Cyan:
					return Color.Cyan;
				case JsonMessagePartColor.Black:
					return Color.DarkGray;
				case JsonMessagePartColor.White:
					return Color.White;
				case null:
					return GetColorFromPartType(messagePart);
				default:
					return Color.White;
			}
		}

		static Color GetColorFromPartType(JsonMessagePart messagePart)
		{
			switch (messagePart.Type)
			{
				case JsonMessagePartType.PlayerId:
					return (int.TryParse(messagePart.Text, out var playerId) && playerId == slot)
						? Color.Yellow
						: Color.Orange;
				case JsonMessagePartType.ItemId:
					return Color.Crimson;
				case JsonMessagePartType.LocationId:
					return Color.Aquamarine;
				default:
					return Color.White;
			}
		}

		public static void UpdateChecks(ItemLocationMap itemLocationMap)
		{
			var locationsCheckedPacket = new LocationChecksPacket
			{
				Locations = itemLocationMap
					.Where(l => l.IsPickedUp && !(l is ExteralItemLocation))
					.Select(l => LocationMap.GetLocationId(l.Key))
					.ToList()
			};

			ReconnectIfNeeded();

			session.SendPacket(locationsCheckedPacket);
		}

		static void ReconnectIfNeeded()
		{
			if (IsConnected && session.Connected)
				return;

			Connect(serverUrl, userName, password, getCheckedLocations, uuid);
		}
	}
}
