using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class Client
	{
		readonly ArchipelagoItemLocationMap itemLocationMap;

		readonly ArchipelagoSession session;

		volatile bool hasConnectionResult;
		ArchipelagoPacketBase connectionResult; //TODO handle connection sucses/failure

		volatile bool hasItemLocationInfo;
		LocationInfoPacket itemLocationInfoResult;

		public Client(ArchipelagoItemLocationMap itemLocationMap)
		{
			this.itemLocationMap = itemLocationMap;
			session = new ArchipelagoSession("ws://localhost:38281");

			session.PacketReceived += PackacedReceived;
		}

		public void Connect()
		{
			session.Connect();

			hasConnectionResult = false;
			connectionResult = null;

			while (!hasConnectionResult)
				Thread.Sleep(100);
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
			//var getDataRequest = new GetDataPackagePacket();
			//session.SendPacket(getDataRequest);

			var connectionRequest = new ConnectPacket
			{
				Game = "Timespinner",
				Name = "YourName1",
				Version = new Version(0, 1, 7),
				Uuid = "297802A3-63F5-433C-A200-11D03C870B55" //TODO Fixme, should be unique per save
			};

			session.SendPacket(connectionRequest);
		}

		void OnDataPackagePacketReceived(DataPackagePacket dataPacket)
		{
		}

		void OnReceivedItemsPacketReceived(ReceivedItemsPacket receivedItemsPacket)
		{
			//TODO handle index order

			foreach (var item in receivedItemsPacket.Items)
				itemLocationMap.RecieveItem(ItemMap.GetItemIdentifier(item.Item));
		}

		void OnPrintPacketReceived(PrintPacket printPacket)
		{
		}

		void OnPrinJsontPacketReceived(PrintJsonPacket printJsonPacket)
		{
		}

		public Dictionary<ItemKey, ItemIdentifier> GetAllItems()
		{
			var items = new Dictionary<ItemKey, ItemIdentifier>();

			RequestAllItems();

			hasItemLocationInfo = false;
			itemLocationInfoResult = null;

			while (!hasItemLocationInfo)
				Thread.Sleep(100);

			foreach (var locationInfo in itemLocationInfoResult.Locations)
				items.Add(LocationMap.GetItemkey(locationInfo.Location), ItemMap.GetItemIdentifier(locationInfo.Item));

			return items;
		}

		void RequestAllItems()
		{
			var peekAllItems = new LocationScoutsPacket
			{
				Locations = LocationMap.AllIds.ToList()
			};

			session.SendPacket(peekAllItems);
		}

		public void UpdateChecks()
		{
			var locationChecks = new LocationChecksPacket
			{
				Locations = itemLocationMap
					.Where(l => l.IsPickedUp)
					.Select(l => LocationMap.GetLocationId(l.Key))
					.ToList()
			};

			session.SendPacket(locationChecks);
		}
	}
}
