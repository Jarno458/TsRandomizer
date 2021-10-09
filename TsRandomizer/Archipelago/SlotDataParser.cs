using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class SlotDataParser
	{
		readonly ConnectedPacket connectedPacket;
		readonly Dictionary<string, object> slotData;

		public SlotDataParser(ConnectedPacket connectedPacket)
		{
			this.connectedPacket = connectedPacket;
			slotData = connectedPacket.SlotData;
		}

		public Requirement GetPyramidKeysGate()
		{
			return GetPyramidKeysGate((string) slotData["PyramidKeysGate"]);
		}

		public static Requirement GetPyramidKeysGate(string pyramidKeysGate)
		{
			return (Requirement)typeof(Requirement)
				.GetField(pyramidKeysGate, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.GetValue(null);
		}

		public Seed GetSeed()
		{
			bool IsEnabled(object o)
			{
				if (o is bool b)
					return b;
				if (o is string s)
					return bool.Parse(s);
				if (o is int i)
					return i > 0;
				if (o is long l)
					return l > 0;

				return false;
			}

			uint flags = 0;

			if (slotData.TryGetValue("StartWithJewelryBox", out var startWithJewelryBox) && IsEnabled(startWithJewelryBox))
				flags |= 1 << 0;
			if (slotData.TryGetValue("ProgressiveVerticalMovement", out var progressiveVerticalMovement) && IsEnabled(progressiveVerticalMovement))
				flags |= 1 << 1;
			if (slotData.TryGetValue("ProgressiveKeycards", out var progressiveKeycards) && IsEnabled(progressiveKeycards))
				flags |= 1 << 2;
			if (slotData.TryGetValue("DownloadableItems", out var downloadableItems) && IsEnabled(downloadableItems))
				flags |= 1 << 3;
			if (slotData.TryGetValue("FacebookMode", out var facebookMode) && IsEnabled(facebookMode))
				flags |= 1 << 4;
			if (slotData.TryGetValue("StartWithMeyef", out var startWithMeyef) && IsEnabled(startWithMeyef))
				flags |= 1 << 5;
			if (slotData.TryGetValue("QuickSeed", out var quickSeed) && IsEnabled(quickSeed))
				flags |= 1 << 6;
			if (slotData.TryGetValue("SpecificKeycards", out var specificKeycards) && IsEnabled(specificKeycards))
				flags |= 1 << 7;
			if (slotData.TryGetValue("Inverted", out var inverted) && IsEnabled(inverted))
				flags |= 1 << 8;
			if (slotData.TryGetValue("StinkyMaw", out var stinkyMaw) && IsEnabled(stinkyMaw))
				flags |= 1 << 9;

			return new Seed(0, new SeedOptions(flags));
		}

		public Dictionary<int, int> GetPersonalItems()
		{
			return slotData.TryGetValue("PersonalItems", out var personalItemsDictionary)
				? ((JObject)personalItemsDictionary).ToObject<Dictionary<int, int>>()
				: Client.ScoutLocations(connectedPacket.MissingChecks.Concat(connectedPacket.LocationsChecked));
		}
	}
}
