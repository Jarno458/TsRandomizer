using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TsRandomizer.Archipelago;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Extensions
{
	static class DictionaryExtensions
	{
		public static bool TryParsePyramidKeysUnlock(this Dictionary<string, string> dataKeyStrings, string key, out Requirement requirement)
		{
			try
			{
				requirement = SlotDataParser.GetPyramidKeysGate(dataKeyStrings[key]);
				return true;
			}
			catch
			{
				requirement = Requirement.None;
				return false;
			}
		}

		public static bool TryParsePersonalItems(this Dictionary<string, string> dataKeyStrings, 
			string key, out Dictionary<ItemKey, ItemIdentifier> personalItems)
		{
			try
			{
				dataKeyStrings.TryGetValue(key, out var personalItemsJson);

				personalItems = JsonConvert
					.DeserializeObject<Dictionary<int, int>>(personalItemsJson)
					.ToDictionary(kvp => LocationMap.GetItemkey(kvp.Key), kvp => ItemMap.GetItemIdentifier(kvp.Value));

				return true;
			}
			catch
			{
				personalItems = null;
				return false;
			}
		}
	}
}
