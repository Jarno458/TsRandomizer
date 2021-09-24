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
	class Client
	{
		const int ConnectionTimeoutInSeconds = 99999;

		readonly ItemLocationMap itemLocations;
		readonly ArchipelagoSession session;

		bool connectionTestOnly;

		volatile bool hasConnectionResult;
		ArchipelagoPacketBase connectionResult;

		volatile bool hasItemLocationInfo;
		LocationInfoPacket itemLocationInfoResult;

		static readonly DataCache Chache = new DataCache();

		volatile int slot;

		ConcurrentQueue<ItemIdentifier> receivedItemsQueue = new ConcurrentQueue<ItemIdentifier>();

		int receivedItemIndex;

		public Client(ItemLocationMap itemLocationMap)
		{
			itemLocations = itemLocationMap;
			session = new ArchipelagoSession("ws://localhost:38281");

			session.PacketReceived += PackacedReceived;
		}

		public ConnectionResult Connect(bool testOnly)
		{
			connectionTestOnly = testOnly;

			if (!connectionTestOnly)
				Chache.LoadCache();

			session.ConnectAsync();

			hasConnectionResult = false;
			connectionResult = null;

			var connectedStartedTime = DateTime.UtcNow;

			while (!hasConnectionResult)
			{
				if (DateTime.UtcNow - connectedStartedTime > TimeSpan.FromSeconds(ConnectionTimeoutInSeconds))
					return ConnectionResult.FromFailure("Connection Timedout");

				Thread.Sleep(100);
			}

			if (connectionTestOnly)
				session.DisconnectAsync();

			if (connectionResult is ConnectionRefusedPacket refused)
				return ConnectionResult.FromFailure(string.Join(", ", refused.Errors));
			if (connectionResult is ConnectedPacket success)
			{
				slot = success.Slot;

				return ConnectionResult.FromSuccess(success.SlotData, success.ItemsChecked);
			}
			
			return ConnectionResult.FromFailure("Unknown package, probably due to version missmatch");
		}

		public IEnumerable<ItemIdentifier> GetReceivedItems()
		{
			while(receivedItemsQueue.TryDequeue(out var itemIdentifier))
				yield return itemIdentifier;
		}

		void PackacedReceived(ArchipelagoPacketBase packet)
		{
			switch (packet)
			{
				case RoomInfoPacket roomInfoPacket:
					OnRoomInfoPacketReceived(roomInfoPacket);
					break;

				case DataPackagePacket dataPacket:
					OnDataPackagePacketReceived(dataPacket);
					break;

				case ConnectionRefusedPacket connectionRefusedPacket:
					hasConnectionResult = true;
					connectionResult = connectionRefusedPacket;
					break;

				case ConnectedPacket connectedPacket:
					hasConnectionResult = true;
					connectionResult = connectedPacket;
					break;

				case LocationInfoPacket locationInfoPacket:
					hasItemLocationInfo = true;
					itemLocationInfoResult = locationInfoPacket;
					break;

				case ReceivedItemsPacket receivedItemsPacket:
					OnReceivedItemsPacketReceived(receivedItemsPacket);
					break;

				case PrintPacket printPacket:
					OnPrintPacketReceived(printPacket);
					break;

				case PrintJsonPacket printJsonPacket:
					OnPrinJsontPacketReceived(printJsonPacket);
					break;
			}
		}

		void OnRoomInfoPacketReceived(RoomInfoPacket packet)
		{
			if (!connectionTestOnly)
			{
				Chache.UpdatePlayerNames(packet.Players);
			
				if (packet is RoomUpdatePacket)
					return;

				Chache.Verify(this, packet.DataPackageVersions);
			}

			var connectionRequest = new ConnectPacket
			{
				Game = "Timespinner",
				Name = "YourName1",
				Version = new Version(0, 1, 7),
				Uuid = "297802A3-63F5-433C-A200-11D03C870B56", //TODO Fixme, should be unique per save
				Tags = new List<string>(0)
			};

			session.SendPacket(connectionRequest);
		}

		public void RequestGameData(List<string> gamesToExcludeFromUpdate)
		{
			var getGameDataPacket = new GetDataPackagePacket
			{
				Exclusions = gamesToExcludeFromUpdate
			};

			session.SendPacket(getGameDataPacket);
		}

		static void OnDataPackagePacketReceived(DataPackagePacket dataPacket)
		{
			Chache.Update(dataPacket.DataPackage.Games);
		}

		void OnReceivedItemsPacketReceived(ReceivedItemsPacket receivedItemsPacket)
		{
			if(connectionTestOnly)
				return;

			if (receivedItemsPacket.Index != receivedItemIndex)
				ReSync();

			foreach (var item in receivedItemsPacket.Items)
				receivedItemsQueue.Enqueue(ItemMap.GetItemIdentifier(item.Item));

			receivedItemIndex = receivedItemsPacket.Index++;
		}

		void ReSync()
		{
			receivedItemIndex = 0;

			Interlocked.Exchange(ref receivedItemsQueue, new ConcurrentQueue<ItemIdentifier>());

			session.SendMultiplePackets(new SyncPacket(), GetLocationChecksPacket());
		}

		void OnPrintPacketReceived(PrintPacket printPacket)
		{
			if (connectionTestOnly)
				return;

			if (printPacket.Text == null)
				return;

			var lines = printPacket.Text.Split('\n');

			foreach (var line in lines)
				ScreenManager.Log.Add(line);
		}

		void OnPrinJsontPacketReceived(PrintJsonPacket printJsonPacket)
		{
			if (connectionTestOnly)
				return;

			var parts = new List<Part>();

			foreach (var messagePart in printJsonPacket.Data)
				parts.Add(new Part(GetMessage(messagePart), GetColor(messagePart)));

			ScreenManager.Log.Add(parts.ToArray());
		}

		static string GetMessage(JsonMessagePart messagePart)
		{
			switch (messagePart.Type)
			{
				case JsonMessagePartType.PlayerId:
					return Chache.GetPlayerName(int.Parse(messagePart.Text));
				case JsonMessagePartType.ItemId:
					return Chache.GetItemName(int.Parse(messagePart.Text));
				case JsonMessagePartType.LocationId:
					return Chache.GetLocationName(int.Parse(messagePart.Text));
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
					return GetColorFromPartType(messagePart.Type);
				default:
					return Color.White;
			}
		}

		static Color GetColorFromPartType(JsonMessagePartType? messagePartType)
		{
			switch (messagePartType)
			{
				case JsonMessagePartType.PlayerId:
					return Color.Orange;
				case JsonMessagePartType.ItemId:
					return Color.Crimson;
				case JsonMessagePartType.LocationId:
					return Color.Aquamarine;
				default:
					return Color.White;
			}
		}

		public Dictionary<ItemKey, ItemIdentifier> GetAllNonCheckedItems()
		{
			var items = new Dictionary<ItemKey, ItemIdentifier>();

			RequestAllNonCheckedItems();

			hasItemLocationInfo = false;
			itemLocationInfoResult = null;

			var connectedStartedTime = DateTime.UtcNow;

			while (!hasItemLocationInfo)
			{
				if (DateTime.UtcNow - connectedStartedTime > TimeSpan.FromSeconds(ConnectionTimeoutInSeconds))
					return null;

				Thread.Sleep(100);
			}

			foreach (var locationInfo in itemLocationInfoResult.Locations)
				items.Add(LocationMap.GetItemkey(locationInfo.Location), ItemMap.GetItemIdentifier(locationInfo.Item));

			return items;
		}

		void RequestAllNonCheckedItems()
		{
			var peekAllNonCheckedItems = new LocationScoutsPacket
			{
				Locations = itemLocations
					.Where(l => !l.IsPickedUp)
					.Select(l => LocationMap.GetLocationId(l.Key))
					.ToList()
			};

			session.SendPacket(peekAllNonCheckedItems);
		}

		public void UpdateChecks()
		{
			session.SendPacket(GetLocationChecksPacket());
		}

		LocationChecksPacket GetLocationChecksPacket()
		{
			return new LocationChecksPacket
			{
				Locations = itemLocations
					.Where(l => l.IsPickedUp && !(l is ExteralItemLocation))
					.Select(l => LocationMap.GetLocationId(l.Key))
					.ToList()
			};
		}
	}

	class ConnectionResult
	{
		ConnectionResult(bool success, string errorMessage, Dictionary<string, object> slotData, List<int> checkedLocations)
		{
			Success = success;
			ErrorMessage = errorMessage;
			SlotData = slotData;
			CheckedLocations = checkedLocations;
		}

		public static ConnectionResult FromSuccess(Dictionary<string, object> slotdata, List<int> checkedLocations)
		{
			return new ConnectionResult(true, "", slotdata, checkedLocations);
		}

		public static ConnectionResult FromFailure(string errorMessage)
		{
			return new ConnectionResult(false, errorMessage, null, null);
		}

		public bool Success { get; }
		public string ErrorMessage { get; }
		public Dictionary<string, object> SlotData { get; }
		public List<int> CheckedLocations { get; }
	}
}
