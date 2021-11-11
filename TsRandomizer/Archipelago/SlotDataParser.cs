using System.Collections.Generic;
using System.Reflection;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class SlotDataParser
	{
		readonly Dictionary<string, object> slotData;

		public SlotDataParser(ConnectedPacket connectedPacket)
		{
			slotData = connectedPacket.SlotData;
		}

		public Requirement GetPyramidKeysGate()
		{
			return GetPyramidKeysGate((string)slotData["PyramidKeysGate"]);
		}

		public static Requirement GetPyramidKeysGate(string pyramidKeysGate)
		{
			//TODO: remove when clients & server are update with correct value
			if (pyramidKeysGate == "GateMilitaryGate")
				return Requirement.GateMilitaryGate;
			if (pyramidKeysGate == "GateLakeSirineLeft")
				return Requirement.GateLakeSereneLeft;
			if (pyramidKeysGate == "GateLakeSirineRight")
				return Requirement.GateLakeSereneRight;

			return (Requirement)typeof(Requirement)
				.GetField(pyramidKeysGate, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.GetValue(null);
		}

		public Seed GetSeed()
		{
			return new Seed(0, new SeedOptions(slotData));
		}

		public Dictionary<int, int> GetPersonalItems()
		{
			return ((JObject)slotData["PersonalItems"]).ToObject<Dictionary<int, int>>();
		}
	}
}
