using System;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using TsRandomizer.Commands;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using XnaColor = Microsoft.Xna.Framework.Color;
using MessagePartColor = Archipelago.MultiClient.Net.Models.Color;

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

		public static int Slot => session.ConnectionInfo.Slot;
		public static int Team => session.ConnectionInfo.Team;

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
				session = ArchipelagoSessionFactory.CreateSession(serverUrl);
				session.MessageLog.OnMessageReceived += OnMessageReceived;
				session.Socket.ErrorReceived += Socket_ErrorReceived;
				session.Socket.SocketOpened += Socket_SocketOpened;
				session.Socket.SocketClosed += Socket_SocketClosed;

				var result = session.TryConnectAndLogin("Timespinner", userName, 
					ItemsHandlingFlags.IncludeStartingInventory, password: password);

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

		static void Socket_ErrorReceived(Exception e, string message)
		{
			ScreenManager.Console.AddLine($"Socket Error: {message}", XnaColor.Red);
			ScreenManager.Console.AddLine($"Socket Exception: {e.Message}", XnaColor.Red);

			foreach (var line in e.StackTrace.Split('\n'))
				ScreenManager.Console.AddLine($"    {line}");
		}
		static void Socket_SocketOpened() =>
			ScreenManager.Console.AddLine($"Socket opened to: {session.Socket.Uri}", XnaColor.Gray);
		static void Socket_SocketClosed(string reason) =>
			ScreenManager.Console.AddLine($"Socket closed: {reason}", XnaColor.Gray);

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
					ScreenManager.Log.Add(itemMessage.IsSenderTheActivePlayer, 
						itemMessage.Receiver.IsSharingGroupWith(Team, Slot), itemMessage.Item.Flags, parts);
					break;
				case CountdownLogMessage countdown:
					_ = new Countdown(countdown.RemainingSeconds);
					break;
				default:
					if (!ScreenManager.IsConsoleOpen)
						ScreenManager.Log.AddSystemMessage(parts);
					break;
			}
		}

		static XnaColor FromDrawingColor(MessagePartColor drawingColor) => new XnaColor(drawingColor.R, drawingColor.G, drawingColor.B, 255);

		static void SendPacket(ArchipelagoPacketBase packet) => session?.Socket?.SendPacket(packet);

		public static void Say(string message) => SendPacket(new SayPacket { Text = message });

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
