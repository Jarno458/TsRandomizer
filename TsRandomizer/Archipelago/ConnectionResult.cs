using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Packets;
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
		public ItemKey[] CheckedLocations { get; }
		public ItemKey[] UncheckedLocations { get; }
		public Dictionary<int, int> PersonalLocations { get; }
		public Requirement PyramidKeysGate { get; }
		public Seed Seed { get; }
		public string ConnectionId { get; }

		public Connected(ConnectedPacket packet, string connectionId) : base(true)
		{
			CheckedLocations = packet.ItemsChecked.Select(LocationMap.GetItemkey).ToArray();
			UncheckedLocations = packet.MissingChecks.Select(LocationMap.GetItemkey).ToArray();

			ConnectionId = connectionId;

			var slotDataParser = new SlotDataParser(packet);

			PyramidKeysGate = slotDataParser.GetPyramidKeysGate();
			Seed = slotDataParser.GetSeed();
			PersonalLocations = slotDataParser.GetPersonalItems();
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