using System;
using System.Collections.Generic;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Archipelago
{
	static class ItemMap
	{
		static readonly Dictionary<int, ItemIdentifier> MapItemIdToItemItemIdentifier;
		static readonly Dictionary<ItemIdentifier, int> MapItemIdentifierToItemId;

		static ItemMap()
		{
			MapItemIdToItemItemIdentifier = new Dictionary<int, ItemIdentifier>(181) {
				{1337000, new ItemIdentifier(EInventoryOrbType.Nether, EOrbSlot.Melee)},
				{1337001, new ItemIdentifier(EInventoryOrbType.Blood, EOrbSlot.Melee)},
				{1337002, new ItemIdentifier(EInventoryOrbType.Moon, EOrbSlot.Melee)},
				{1337003, new ItemIdentifier(EInventoryOrbType.Wind, EOrbSlot.Melee)},
				{1337004, new ItemIdentifier(EInventoryOrbType.Ice, EOrbSlot.Melee)},
				{1337005, new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Melee)},
				{1337006, new ItemIdentifier(EInventoryOrbType.Empire, EOrbSlot.Melee)},
				{1337007, new ItemIdentifier(EInventoryOrbType.Nether, EOrbSlot.Spell)},
				{1337008, new ItemIdentifier(EInventoryOrbType.Ice, EOrbSlot.Spell)},
				{1337009, new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell)},
				{1337010, new ItemIdentifier(EInventoryOrbType.Empire, EOrbSlot.Spell)},
				{1337011, new ItemIdentifier(EInventoryOrbType.Empire, EOrbSlot.Passive)},
				{1337012, new ItemIdentifier(EInventoryOrbType.Ice, EOrbSlot.Passive)}
			};

			MapItemIdentifierToItemId = new Dictionary<ItemIdentifier, int>(MapItemIdToItemItemIdentifier.Count);

			foreach (var kvp in MapItemIdToItemItemIdentifier)
				MapItemIdentifierToItemId.Add(kvp.Value, kvp.Key);
		}

		public static int GetItemId(ItemIdentifier itemIdentifier) =>
			MapItemIdentifierToItemId.TryGetValue(itemIdentifier, out var locationId)
				? locationId
				: throw new Exception("itemIdentifier does not map to Archipelago itemId");


		public static ItemIdentifier GetItemIdentifier(int itemId) =>
			MapItemIdToItemItemIdentifier.TryGetValue(itemId, out var key)
				? key
				: new ItemIdentifier(EInventoryUseItemType.EssenceCrystal);
	}
}
