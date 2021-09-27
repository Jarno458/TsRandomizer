using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net.Packets;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	abstract class ConnectionResult
	{
		public bool Success { get; }

		protected ConnectionResult(bool success)
		{
			Success = success;
		}
	}

	class Connected : ConnectionResult
	{
		public Dictionary<string, object> SlotData { get; }
		public ItemKey[] CheckedLocations { get; }
		public ItemKey[] UncheckedLocations { get; }
		public Dictionary<ItemKey, ItemIdentifier> PersonalLocations { get; }

		public Connected(ConnectedPacket packet) : base(true)
		{
			SlotData = packet.SlotData;
			CheckedLocations = packet.ItemsChecked.Select(LocationMap.GetItemkey).ToArray();
			UncheckedLocations = packet.MissingChecks.Select(LocationMap.GetItemkey).ToArray();

			if (SlotData.TryGetValue("PersonalItems", out object personalItemsDictionary))
			{
				var itemPerLocation = (Dictionary<int, int>) personalItemsDictionary;

				PersonalLocations = itemPerLocation.ToDictionary(
					kvp => LocationMap.GetItemkey(kvp.Key),
					kvp => ItemMap.GetItemIdentifier(kvp.Value));
			}
			else
			{
				PersonalLocations = GetAllPersonalItems(packet);
			}
		}

		public static Dictionary<ItemKey, ItemIdentifier> GetAllPersonalItems(ConnectedPacket connectedPacket)
		{
			var items = new Dictionary<ItemKey, ItemIdentifier>();

			RequestAllItems(connectedPacket);

			Client.HasItemLocationInfo = false;
			Client.LocationScoutResult = null;

			var connectedStartedTime = DateTime.UtcNow;

			while (!Client.HasItemLocationInfo)
			{
				if (DateTime.UtcNow - connectedStartedTime > TimeSpan.FromSeconds(Client.ConnectionTimeoutInSeconds))
					return null;

				Thread.Sleep(100);
			}

			if(Client.LocationScoutResult == null)
				return new Dictionary<ItemKey, ItemIdentifier>();

			foreach (var locationInfo in Client.LocationScoutResult.Locations)
			{
				if (locationInfo.Player != connectedPacket.Slot)
					continue;

				items.Add(LocationMap.GetItemkey(locationInfo.Location), ItemMap.GetItemIdentifier(locationInfo.Item));
			}

			return items;
		}

		static void RequestAllItems(ConnectedPacket connectedPacket)
		{
			var peekAllNonCheckedItems = new LocationScoutsPacket
			{
				Locations = connectedPacket.MissingChecks
					.Concat(connectedPacket.ItemsChecked).ToList()
			};

			Client.SendPacket(peekAllNonCheckedItems);
		}
	}

	class ConnectionFailed : ConnectionResult
	{
		public string ErrorMessage { get; }

		public ConnectionFailed(string errorMessage) : base(false)
		{
			ErrorMessage = errorMessage;
		}
	}
}