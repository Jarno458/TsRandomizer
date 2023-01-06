using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TsRandomizer.Randomisation;
using TsRandomizer.Settings;

namespace TsRandomizer.Archipelago
{
	class SlotDataParser
	{
		readonly Dictionary<string, object> slotData;
		readonly string seedString;
		readonly uint slotId;

		public SlotDataParser(Dictionary<string, object> slotData, string seedString, int slotId)
		{
			this.slotData = slotData;
			this.seedString = seedString;
			this.slotId = (uint)slotId;
		}

		public Requirement GetPyramidKeysGate() =>
			 GetPyramidKeysGate((string)slotData["PyramidKeysGate"]);

		public static Requirement GetPyramidKeysGate(string pyramidKeysGate)
		{
			//TODO: remove when clients & server are update with correct value
			if (pyramidKeysGate == "GateMilitairyGate")
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
			uint seedId = 0;

			if (seedString != null && seedString.Length > 9)
				uint.TryParse(seedString.Substring(seedString.Length - 9), NumberStyles.Integer, CultureInfo.InvariantCulture, out seedId);

			seedId += slotId;
			
			return new Seed(seedId, new SeedOptions(slotData), new RandomFloodsFlags(slotData));
		}

		public SettingCollection GetSettings() =>
			GameSettingsLoader.LoadSettingsFromSlotData(slotData);

		public Dictionary<int, int> GetPersonalItems() => 
			((JObject)slotData["PersonalItems"]).ToObject<Dictionary<int, int>>();
	}
}
