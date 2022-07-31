using System;
using System.Collections.Generic;
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
				session.MessageLog.OnMessageReceived += OnMessageReceived;

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

		static void OnMessageReceived(LogMessage message)
		{
			var parts = message.Parts.Select(p => new Part(p.Text, FromDrawingColor(p.Color))).ToArray();

			ScreenManager.Console.Add(parts);

			switch (message)
			{
				case ItemSendLogMessage itemMessage:
					ScreenManager.Log.Add(IsMe(itemMessage.SendingPlayerSlot), IsMe(itemMessage.ReceivingPlayerSlot), itemMessage.Item.Flags, parts);
					break;
				default:
					if (!ScreenManager.IsConsoleOpen)
						ScreenManager.Log.AddSystemMessage(parts);
					break;
			}
		}

		static Color FromDrawingColor(System.Drawing.Color drawingColor) => new Color(drawingColor.R, drawingColor.G, drawingColor.B, drawingColor.A);

		static void SendPacket(ArchipelagoPacketBase packet) => session?.Socket?.SendPacket(packet);

		public static void Say(string message) => SendPacket(new SayPacket { Text = message });

		static bool IsMe(int slot) => slot == session.ConnectionInfo.Slot;

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
