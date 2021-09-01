using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;

namespace TsRandomizer.Archipelago
{
	class Client
	{
		ArchipelagoSession session;

		public Client()
		{
			session = new ArchipelagoSession("ws://45.83.104.96:61047");

			session.PacketReceived += PackacedReceived;

			session.Connect();
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

				case ConnectedPacket connectedPacket:
					OnConnectedPacketReceived(connectedPacket);
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
			var getDataRequest = new GetDataPackagePacket();
			session.SendPacket(getDataRequest);

			var connectionRequest = new ConnectPacket
			{
				Game = "Timespinner",
				Name = "YourName1",
				Version = new Version(0, 1, 7),
				Uuid = "297802A3-63F5-433C-A200-11D03C870B55"
			};

			session.SendPacket(connectionRequest);
		}

		void OnDataPackagePacketReceived(DataPackagePacket dataPacket)
		{
		}

		void OnConnectedPacketReceived(ConnectedPacket connectedPacket)
		{
		}
		void OnReceivedItemsPacketReceived(ReceivedItemsPacket receivedItemsPacket)
		{
		}

		void OnPrintPacketReceived(PrintPacket printPacket)
		{
		}

		void OnPrinJsontPacketReceived(PrintJsonPacket printJsonPacket)
		{
		}
	}
}
