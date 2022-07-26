using System;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Microsoft.Xna.Framework;
using TsRandomizer.Commands;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.Archipelago
{
	static class Client
	{
		static ArchipelagoSession session;

		static string serverUrl;
		static string userName;
		static string password;

		static LoginResult cachedConnectionResult;

		public static bool IsConnected;

		public static Permissions ForfeitPermissions => session.RoomState.ForfeitPermissions;
		public static Permissions CollectPermissions => session.RoomState.CollectPermissions;

		public static string ConnectionId => session.ConnectionInfo.Uuid;

		public static string SeedString => session.RoomState.Seed;

		public static DeathLinkService GetDeathLinkService() => session.CreateDeathLinkService();

		public static string GetCurrentPlayerName() => session.Players.GetPlayerAliasAndName(session.ConnectionInfo.Slot);

		public static LocationCheckHelper LocationCheckHelper => session.Locations;

		public static DataStorageHelper DataStorage => session.DataStorage;

		public static LoginResult Connect(string server, string user, string pass = null, string connectionId = null)
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

			try
			{
				session = ArchipelagoSessionFactory.CreateSession(new Uri(serverUrl));
				session.Socket.PacketReceived += PackageReceived;

				var result = session.TryConnectAndLogin("Timespinner", userName, 
					ItemsHandlingFlags.IncludeStartingInventory, tags: new string[0] , password: password);

				IsConnected = result.Successful;
				cachedConnectionResult = result;

				if (result.Successful)
				{
#if DEBUG
					ScreenManager.Console.AddCommand(new ScoutCommand());
					ScreenManager.Console.AddCommand(new GetKeyCommand());
#endif
				}
			}
			catch (AggregateException e)
			{
				IsConnected = false;
				cachedConnectionResult = new LoginFailure(e.GetBaseException().Message);
			}

			return cachedConnectionResult;
		}

		public static void Disconnect()
		{
			session?.Socket?.DisconnectAsync();

			serverUrl = null;
			userName = null;
			password = null;

			IsConnected = false;

			session = null;

			cachedConnectionResult = null;
		}

		public static NetworkItem? GetNextItem(int currentIndex) =>
			session.Items.AllItemsReceived.Count > currentIndex 
				? session.Items.AllItemsReceived[currentIndex]
				: default(NetworkItem?);

		public static void SetStatus(ArchipelagoClientState status) => SendPacket(new StatusUpdatePacket { Status = status });

		static void PackageReceived(ArchipelagoPacketBase packet)
		{
			switch (packet)
			{
				case PrintPacket printPacket: OnPrintPacketReceived(printPacket); break;
				case ItemPrintJsonPacket printJsonPacket: OnItemPrintJsonPacketReceived(printJsonPacket); break;
				case PrintJsonPacket printJsonPacket: OnPrintJsonPacketReceived(printJsonPacket); break;
			}
		}

		static void SendPacket(ArchipelagoPacketBase packet) => session?.Socket?.SendPacket(packet);

		public static void Say(string message) => SendPacket(new SayPacket { Text = message });

		static void OnPrintPacketReceived(PrintPacket printPacket)
		{
			if (printPacket.Text == null)
				return;

			var lines = printPacket.Text.Split('\n');

			foreach (var line in lines)
			{
				ScreenManager.Console.Add(new Part(line));
				
				if(!ScreenManager.IsConsoleOpen)
					ScreenManager.Log.AddSystemMessage(new Part(line));
			}
		}

		static void OnItemPrintJsonPacketReceived(ItemPrintJsonPacket printJsonPacket) => 
			OnPrintJsonPacketReceived(printJsonPacket, ItemIsSendByMe(printJsonPacket), ItemIsReceivedByMe(printJsonPacket), printJsonPacket.Item.Flags);

		static void OnPrintJsonPacketReceived(PrintJsonPacket printJsonPacket, 
			bool isSendByMe = false, bool isReceivedByMe = false, ItemFlags itemFlags = ItemFlags.None)
		{
			var parts = printJsonPacket.Data
				.Select(messagePart => new Part(GetMessage(messagePart), GetColor(messagePart)))
				.ToArray();

			ScreenManager.Console.Add(parts);
			ScreenManager.Log.Add(isSendByMe, isReceivedByMe, itemFlags, parts);
		}

		static bool ItemIsSendByMe(ItemPrintJsonPacket printJsonPacket) => printJsonPacket.Item.Player == session.ConnectionInfo.Slot;

		static bool ItemIsReceivedByMe(ItemPrintJsonPacket printJsonPacket) => printJsonPacket.ReceivingPlayer == session.ConnectionInfo.Slot;

		static string GetMessage(JsonMessagePart messagePart)
		{
			switch (messagePart.Type)
			{
				case JsonMessagePartType.PlayerId:
					return int.TryParse(messagePart.Text, out var playerSlot) 
						? session.Players.GetPlayerAliasAndName(playerSlot) ?? $"Slot: {playerSlot}"
						: messagePart.Text;
				case JsonMessagePartType.ItemId:
					return int.TryParse(messagePart.Text, out var itemId)
						? session.Items.GetItemName(itemId) ?? $"Item: {itemId}"
						: messagePart.Text;
				case JsonMessagePartType.LocationId:
					return int.TryParse(messagePart.Text, out var locationId)
						? session.Locations.GetLocationNameFromId(locationId) ?? $"Location: {locationId}"
						: messagePart.Text;
				default:
					return messagePart.Text;
			}
		}

		static Color GetColor(JsonMessagePart messagePart)
		{
			if (messagePart.Type != JsonMessagePartType.Color)
				return GetColorFromPartType(messagePart);

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
				default:
					return Color.White;
			}
		}

		static Color GetColorFromPartType(JsonMessagePart messagePart)
		{
			switch (messagePart.Type)
			{
				case JsonMessagePartType.PlayerId:
					return (int.TryParse(messagePart.Text, out var playerId) && playerId == session.ConnectionInfo.Slot)
						? Color.Yellow
						: Color.Orange;
				case JsonMessagePartType.PlayerName:
					return session.Players.GetPlayerName(session.ConnectionInfo.Slot) == messagePart.Text
						? Color.Yellow
						: Color.Orange;
				case JsonMessagePartType.ItemId:
				case JsonMessagePartType.ItemName:
					switch (messagePart.Flags)
					{
						case ItemFlags.Advancement:
							return Color.Crimson * 6;
						case ItemFlags.NeverExclude:
							return Color.Crimson * 2.5f;
						case ItemFlags.Trap:
							return Color.Red;
						default:
							return Color.Crimson;
					}
				case JsonMessagePartType.LocationId:
				case JsonMessagePartType.LocationName:
					return Color.Aquamarine;
				case JsonMessagePartType.EntranceName:
					return Color.DarkOliveGreen;
				default:
					return Color.White;
			}
		}

		public static void UpdateChecks(ItemLocationMap itemLocationMap) => 
			Task.Factory.StartNew(() => { UpdateChecksTask(itemLocationMap); });

		static void UpdateChecksTask(ItemLocationMap itemLocationMap)
		{
			var locations = itemLocationMap
				.Where(l => l.IsPickedUp && !(l is ExternalItemLocation))
				.Select(l => LocationMap.GetLocationId(l.Key))
				.ToArray();

			ReconnectIfNeeded();

			session.Locations.CompleteLocationChecks(locations);
		}

		static void ReconnectIfNeeded()
		{
			if (IsConnected && session.Socket.Connected)
				return;

			Connect(serverUrl, userName, password, session.ConnectionInfo.Uuid);
		}
	}
}
