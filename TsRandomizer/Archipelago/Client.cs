using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		static volatile int slot = -1;

		static string serverUrl;
		static string userName;
		static string password;
		static string uuid;

		static ConnectionResult cachedConnectionResult;

		public static bool IsConnected;

		public static Permissions ForfeitPermissions = 0;

		public static ConnectionResult Connect(string server, string user, string pass, string connectionId)
		{
			if (IsConnected && session.Socket.Connected && cachedConnectionResult != null)
			{
				if (serverUrl == server && userName == user && password == pass)
					return cachedConnectionResult;
				
				Disconnect();
			}

			serverUrl = server;
			userName = user;
			password = pass;
			uuid = connectionId ?? Guid.NewGuid().ToString("N");
			
			session = ArchipelagoSessionFactory.CreateSession(new Uri(serverUrl));
			session.Socket.PacketReceived += PackacedReceived;

			session.AttemptConnectAndLogin("Timespinner", userName, new Version(0, 2, 0), new List<string>(0), password, uuid);

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
				
				cachedConnectionResult = new Connected(success, uuid);
				return cachedConnectionResult;
			}

			Disconnect();

			return new ConnectionFailed("Unknown package, probably due to version missmatch");
		}

		public static void Disconnect()
		{
			session?.Socket?.DisconnectAsync();

			serverUrl = null;
			userName = null;
			password = null;
			uuid = null;

			slot = -1;

			IsConnected = false;

			hasConnectionResult = false;

			session = null;

			ForfeitPermissions = 0;

			cachedConnectionResult = null;
		}

		public static ItemIdentifier GetNextItem(int currentIndex)
		{
			return session.Items.AllItemsReceived.Count > currentIndex 
				? ItemMap.GetItemIdentifier(session.Items.AllItemsReceived[currentIndex + 1].Item)
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
				case ConnectionRefusedPacket connectionRefusedPacket: OnConnectionRefusedPacketReceived(connectionRefusedPacket); break;
				case ConnectedPacket connectedPacket: OnConnectedPacketReceived(connectedPacket); break;
				case PrintPacket printPacket: OnPrintPacketReceived(printPacket); break;
				case PrintJsonPacket printJsonPacket: OnPrinJsontPacketReceived(printJsonPacket); break;
			}
		}

		static void SendPacket(ArchipelagoPacketBase packet)
		{
			session?.Socket?.SendPacket(packet);
		}

		public static void Forfeit()
		{
			SendPacket(new SayPacket { Text = "!forfeit" });
		}

		static void OnRoomInfoPacketReceived(RoomInfoPacket packet)
		{
			if (packet.Permissions != null && packet.Permissions.TryGetValue("forfeit", out var permissions))
				ForfeitPermissions = permissions;
		}

		static void OnConnectedPacketReceived(ConnectedPacket connectedPacket)
		{
			slot = connectedPacket.Slot;

			hasConnectionResult = true;
			connectionResult = connectedPacket;
		}

		static void OnConnectionRefusedPacketReceived(ConnectionRefusedPacket connectionRefusedPacket)
		{
			slot = -1;

			hasConnectionResult = true;
			connectionResult = connectionRefusedPacket;
		}

		static void OnPrintPacketReceived(PrintPacket printPacket)
		{
			if (printPacket.Text == null)
				return;

			var lines = printPacket.Text.Split('\n');

			foreach (var line in lines)
				ScreenManager.Log.Add(true, new Part(line));
		}

		static void OnPrinJsontPacketReceived(PrintJsonPacket printJsonPacket)
		{
			var parts = new List<Part>();

			foreach (var messagePart in printJsonPacket.Data)
				parts.Add(new Part(GetMessage(messagePart), GetColor(messagePart)));

			ScreenManager.Log.Add(MessageIsAboutCurrentPlayer(printJsonPacket), parts.ToArray());
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
						? session.Players.GetPlayerAliasAndName(playerSlot)
						: messagePart.Text;
				case JsonMessagePartType.ItemId:
					return int.TryParse(messagePart.Text, out var itemId)
						? session.Items.GetItemName(itemId)
						: messagePart.Text;
				case JsonMessagePartType.LocationId:
					return int.TryParse(messagePart.Text, out var locationId)
						? session.Locations.GetLocationNameFromId(locationId)
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
			Task.Factory.StartNew(() => { UpdateChecksTask(itemLocationMap); });
		}

		public static void UpdateChecksTask(ItemLocationMap itemLocationMap)
		{
			var locations = itemLocationMap
				.Where(l => l.IsPickedUp && !(l is ExteralItemLocation))
				.Select(l => LocationMap.GetLocationId(l.Key))
				.ToArray();

			ReconnectIfNeeded();

			session.Locations.CompleteLocationChecks(locations);
		}

		static void ReconnectIfNeeded()
		{
			if (IsConnected && session.Socket.Connected)
				return;

			Connect(serverUrl, userName, password, uuid);
		}
	}
}
