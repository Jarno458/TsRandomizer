﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Archipelago.MultiClient.Net;
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

		public SlotDataParser(LoginSuccessful login)
		{
			slotData = login.SlotData;
			seedString = Client.SeedString;
			slotId = (uint)login.Slot;
		}

		public Requirement GetPyramidKeysGate() =>
			GetPyramidKeysGate(slotData, "PyramidKeysGate");
		public Requirement GetPastPyramidKeysGate() =>
			GetPyramidKeysGate(slotData, "PastGate");
		public Requirement GetPresentPyramidKeysGate() =>
			GetPyramidKeysGate(slotData, "PresentGate");
		public Requirement GetTimePyramidKeysGate() =>
			GetPyramidKeysGate(slotData, "TimeGate");

		public static Requirement GetPyramidKeysGate(Dictionary<string, object> slotData, string gateName)
		{
			if (!slotData.TryGetValue(gateName, out var slotDataGate))
				return Requirement.None;

			var pyramidKeysGate = (string)slotDataGate;

			//TODO: remove when clients & server are update with correct value
			if (pyramidKeysGate == "GateMilitairyGate")
				return Requirement.GateMilitaryGate;
			if (pyramidKeysGate == "GateLakeSereneLeft")
				return Requirement.GateLakeSereneLeft;
			if (pyramidKeysGate == "GateLakeSereneRight")
				return Requirement.GateLakeSereneRight;

			var req = (Requirement?)typeof(Requirement)
				.GetField(pyramidKeysGate, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				?.GetValue(null);

			if (!req.HasValue)
				throw new Exception($"cannot find requirement for pyramid gate {pyramidKeysGate}");

			return req.Value;
		}

		public Seed GetSeed()
		{
			uint seedId = 0;

			if (seedString != null && seedString.Length > 9)
				uint.TryParse(seedString.Substring(seedString.Length - 9), NumberStyles.Integer, CultureInfo.InvariantCulture, out seedId);

			seedId += slotId;
			
			return new Seed(seedId, new SeedOptions(slotData), new RisingTides(slotData));
		}

		public SettingCollection GetSettings() =>
			GameSettingsLoader.LoadSettingsFromSlotData(slotData);

		public Dictionary<int, int> GetPersonalItems() => 
			((JObject)slotData["PersonalItems"]).ToObject<Dictionary<int, int>>();
	}
}
