using System;
using System.Collections.Generic;
using TsRandomizer.Randomisation;
using TsRandomizer.ReplacementObjects;

namespace TsRandomizer.Archipelago
{
	static class LocationMap
	{
		static readonly Dictionary<long, ItemKey> MapLocationIdToItemKey;
		static readonly Dictionary<ItemKey, long> MapItemKeyToLocationId;

		static LocationMap()
		{
			MapLocationIdToItemKey = new Dictionary<long, ItemKey>(181) {
				// Present
				// Tutorial
				{1337000, ItemKey.TutorialMeleeOrb},
				{1337001, ItemKey.TutorialSpellOrb},
				// Lake desolation
				{1337002, new ItemKey(1, 1, 1528, 144)},
				{1337003, new ItemKey(1, 15, 264, 144)},
				{1337004, new ItemKey(1, 25, 296, 176)},
				{1337005, new ItemKey(1, 9, 600, 144 + TimespinnerWheel.YOffset)},
				{1337006, new ItemKey(1, 14, 40, 176)},
				// Lower lake desolation
				{1337007, new ItemKey(1, 2, 1016, 384)},
				{1337008, new ItemKey(1, 11, 72, 240)},
				{1337009, new ItemKey(1, 3, 56, 176)},
				// Upper lake desolation
				{1337010, new ItemKey(1, 17, 152, 96)},
				{1337011, new ItemKey(1, 21, 200, 144)},
				{1337012, new ItemKey(1, 20, 232, 96)},
				{1337013, new ItemKey(1, 20, 168, 240)},
				{1337014, new ItemKey(1, 22, 344, 160)},
				{1337015, new ItemKey(1, 18, 1320, 189)},
				{1337016, new ItemKey(1, 18, 1272, 192)},
				{1337017, new ItemKey(1, 18, 1368, 192)},
				{1337018, new RoomItemKey(1, 5)},
				// Libary
				{1337019, new ItemKey(2, 60, 328, 160)},
				{1337020, new ItemKey(2, 54, 296, 176)},
				{1337021, new ItemKey(2, 41, 404, 246)},
				{1337022, new ItemKey(2, 44, 680, 368)},
				{1337023, new ItemKey(2, 47, 216, 208)},
				{1337024, new ItemKey(2, 47, 152, 208)},
				{1337025, new ItemKey(2, 47, 88, 208)},
				// Libary top
				{1337026, new ItemKey(2, 56, 168, 192)},
				{1337027, new ItemKey(2, 56, 392, 192)},
				{1337028, new ItemKey(2, 56, 616, 192)},
				{1337029, new ItemKey(2, 56, 840, 192)},
				{1337030, new ItemKey(2, 56, 1064, 192)},
				// Varndagroth tower left
				{1337031, new ItemKey(2, 34, 232, 1200)},
				{1337032, new ItemKey(2, 40, 344, 176)},
				{1337033, new ItemKey(2, 32, 328, 160)},
				{1337034, new ItemKey(2, 7, 232, 144)},
				{1337035, new ItemKey(2, 25, 328, 192)},
				// Varndagroth tower right
				{1337036, new ItemKey(2, 15, 760, 192)},
				{1337037, new ItemKey(2, 20, 72, 1200)},
				{1337038, new ItemKey(2, 23, 72, 560)},
				{1337039, new ItemKey(2, 23, 1112, 112)},
				{1337040, new ItemKey(2, 23, 136, 304)},
				{1337041, new ItemKey(2, 11, 104, 192)},
				{1337042, new RoomItemKey(2, 29)},
				{1337043, new RoomItemKey(2, 52)},
				// Sealed Caves (Xarion)
				{1337044, new ItemKey(9, 10, 248, 848)},
				{1337045, new ItemKey(9, 19, 664, 704)},
				{1337046, new ItemKey(9, 39, 88, 192)},
				{1337047, new ItemKey(9, 41, 312, 192)},
				{1337048, new ItemKey(9, 42, 328, 192)},
				{1337049, new ItemKey(9, 12, 280, 160)},
				{1337050, new ItemKey(9, 48, 104, 160)},
				{1337051, new ItemKey(9, 15, 248, 192)},
				{1337052, new RoomItemKey(9, 13)},
				//Sealed Caves (Sirens)
				{1337053, new ItemKey(9, 5, 88, 496)},
				{1337054, new ItemKey(9, 3, 1848, 576)},
				{1337055, new ItemKey(9, 3, 744, 560)},
				{1337056, new ItemKey(9, 2, 184, 176)},
				{1337057, new ItemKey(9, 2, 104, 160)},
				// Military Fortress
				{1337058, new ItemKey(10, 3, 264, 128)},
				{1337059, new ItemKey(10, 11, 296, 192)},
				{1337060, new ItemKey(10, 4, 1064, 176)},
				{1337061, new ItemKey(10, 10, 104, 192)},
				{1337062, new ItemKey(10, 8, 1080, 176)},
				{1337063, new ItemKey(10, 7, 104, 192)},
				{1337064, new ItemKey(10, 7, 152, 192)},
				{1337065, new ItemKey(10, 18, 280, 189)},
				// The lab
				{1337066, new ItemKey(11, 36, 312, 192)},
				{1337067, new ItemKey(11, 3, 1528, 192)},
				{1337068, new ItemKey(11, 3, 72, 192)},
				{1337069, new ItemKey(11, 25, 104, 192)},
				{1337070, new ItemKey(11, 18, 824, 128)},
				{1337071, new RoomItemKey(11, 39)},
				{1337072, new RoomItemKey(11, 21)},
				{1337073, new RoomItemKey(11, 1)},
				{1337074, new ItemKey(11, 6, 328, 192)},
				{1337075, new ItemKey(11, 27, 296, 160)},
				{1337076, new RoomItemKey(11, 26)},
				// Emperors tower
				{1337077, new ItemKey(12, 5, 344, 192)},
				{1337078, new ItemKey(12, 3, 200, 160)},
				{1337079, new ItemKey(12, 25, 360, 176)},
				{1337080, new ItemKey(12, 22, 56, 192)},
				{1337081, new ItemKey(12, 9, 344, 928)},
				{1337082, new ItemKey(12, 19, 72, 192)},
				{1337083, new ItemKey(12, 13, 120, 176)},
				{1337084, new ItemKey(12, 11, 264, 208)},
				{1337085, new ItemKey(12, 11, 136, 205)},

				// Past
				// Refugee Camp
				{1337086, new RoomItemKey(3, 0)},
				{1337087, new ItemKey(3, 30, 296, 176)},
				{1337088, new ItemKey(3, 30, 232, 176)},
				{1337089, new ItemKey(3, 30, 168, 176)},
				// Forest
				{1337090, new ItemKey(3, 3, 648, 272)},
				{1337091, new ItemKey(3, 15, 248, 112)},
				{1337092, new ItemKey(3, 21, 120, 192)},
				{1337093, new ItemKey(3, 12, 776, 560)},
				{1337094, new ItemKey(3, 11, 392, 608)},
				{1337095, new ItemKey(3, 5, 184, 192)},
				{1337096, new ItemKey(3, 2, 584, 368)},
				{1337097, new ItemKey(4, 20, 264, 160)},
				{1337098, new ItemKey(3, 29, 248, 192)},
				// Upper Lake Sirine
				{1337099, new ItemKey(7, 16, 152, 96)},
				{1337100, new ItemKey(7, 19, 248, 96)},
				{1337101, new ItemKey(7, 19, 168, 240)},
				{1337102, new ItemKey(7, 27, 184, 144)},
				{1337103, new ItemKey(7, 13, 56, 176)},
				{1337104, new ItemKey(7, 30, 296, 176)},
				// Lower Lake Sirine
				{1337105, new ItemKey(7, 3, 440, 1232)},
				{1337106, new ItemKey(7, 7, 1432, 576)},
				{1337107, new ItemKey(7, 6, 520, 496)},
				{1337108, new ItemKey(7, 11, 88, 240)},
				{1337109, new ItemKey(7, 2, 1016, 384)},
				{1337110, new ItemKey(7, 20, 248, 96)},
				{1337111, new ItemKey(7, 9, 584, 189)},
				// Caves of Banishment (Maw)
				{1337112, new ItemKey(8, 19, 664, 704)},
				{1337113, new ItemKey(8, 12, 280, 160)},
				{1337114, new ItemKey(8, 48, 104, 160)},
				{1337115, new ItemKey(8, 39, 88, 192)},
				{1337116, new ItemKey(8, 41, 168, 192)},
				{1337117, new ItemKey(8, 41, 216, 192)},
				{1337118, new ItemKey(8, 41, 264, 192)},
				{1337119, new ItemKey(8, 41, 312, 192)},
				{1337120, new ItemKey(8, 42, 216, 189)},
				{1337121, new ItemKey(8, 15, 248, 192)},
				{1337122, new ItemKey(8, 31, 88, 400)},
				// Caves of Banishment (Sirens)
				{1337123, new ItemKey(8, 4, 664, 144)},
				{1337124, new ItemKey(8, 3, 808, 144)},
				{1337125, new ItemKey(8, 3, 744, 560)},
				{1337126, new ItemKey(8, 3, 1848, 576)},
				{1337127, new ItemKey(8, 5, 88, 496)},
				// Caste Ramparts
				{1337128, new ItemKey(4, 1, 456, 160)},
				{1337129, new ItemKey(4, 3, 136, 144)},
				{1337130, new ItemKey(4, 10, 56, 192)},
				{1337131, new ItemKey(4, 11, 344, 192)},
				{1337132, new ItemKey(4, 22, 104, 189)},
				// Caste Keep
				{1337133, new ItemKey(5, 9, 104, 189)},
				{1337134, new ItemKey(5, 10, 104, 192)},
				{1337135, new ItemKey(5, 14, 88, 208)},
				{1337136, new ItemKey(5, 44, 216, 192)},
				{1337137, new ItemKey(5, 45, 104, 192)},
				{1337138, new ItemKey(5, 15, 296, 192)},
				{1337139, new ItemKey(5, 41, 72, 160)},
				{1337140, new RoomItemKey(5, 5)},
				{1337141, new ItemKey(5, 22, 312, 176)},
				//Royal towers
				{1337142, new ItemKey(6, 19, 200, 176)},
				{1337143, new ItemKey(6, 27, 472, 384)},
				{1337144, new ItemKey(6, 1, 1512, 288)},
				{1337145, new ItemKey(6, 25, 360, 176)},
				{1337146, new ItemKey(6, 3, 120, 208)},
				{1337147, new ItemKey(6, 17, 200, 112)},
				{1337148, new ItemKey(6, 17, 56, 448)},
				{1337149, new ItemKey(6, 17, 360, 1840)},
				{1337150, new ItemKey(6, 13, 120, 176)},
				{1337151, new ItemKey(6, 22, 88, 208)},
				{1337152, new ItemKey(6, 11, 360, 544)},
				{1337153, new ItemKey(6, 23, 856, 208)},
				{1337154, new ItemKey(6, 14, 136, 208)},
				{1337155, new ItemKey(6, 14, 184, 205)},

				// Download Terminals
				{1337156, new ItemKey(2, 44, 120, 368)},
				{1337157, new ItemKey(2, 44, 792, 592)},
				// 1337158 Lost in time
				{1337159, new ItemKey(2, 44, 456, 368)},
				{1337160, new ItemKey(2, 58, 152, 208)},
				{1337161, new ItemKey(2, 58, 232, 208)},
				{1337162, new ItemKey(2, 58, 312, 208)},
				{1337163, new ItemKey(2, 44, 568, 176)},
				{1337164, new ItemKey(2, 18, 200, 192)},
				{1337165, new ItemKey(11, 6, 200, 192)},
				{1337166, new ItemKey(11, 15, 152, 176)},
				{1337167, new ItemKey(11, 16, 600, 192)},
				{1337168, new ItemKey(11, 34, 200, 192)},
				{1337169, new ItemKey(11, 37, 200, 192)},
				{1337170, new ItemKey(11, 38, 120, 176)},
				{1337171, new ItemKey(5, 20, 504, 48)},
				{1337172, new ItemKey(8, 3, 1256, 544)},
				{1337173, new RoomItemKey(8, 21)},
				{1337174, new ItemKey(7, 3, 120, 204)},
				{1337175, new RoomItemKey(7, 28)},
				{1337176, new RoomItemKey(7, 5)},

				// Lore Checks
				// Memories (Present)
				{1337177, new ItemKey(1, 10, 312, 81)},
				{1337178, new ItemKey(2, 5, 200, 145)},
				{1337179, new ItemKey(2, 45, 344, 145)},
				{1337180, new ItemKey(2, 51, 88, 177)},
				{1337181, new ItemKey(2, 25, 216, 145)},
				{1337182, new ItemKey(2, 46, 200, 145)},
				{1337183, new ItemKey(2, 11, 200, 161)},
				{1337184, new ItemKey(10, 3, 536, 97)},
				{1337185, new ItemKey(11, 7, 248, 129)},
				{1337186, new ItemKey(11, 7, 296, 129)},
				{1337187, new ItemKey(12, 19, 56, 145)},
				// Letters (Past)
				{1337188, new ItemKey(3, 12, 472, 161)},
				{1337189, new ItemKey(3, 15, 328, 97)},
				{1337190, new ItemKey(4, 18, 456, 497)},
				{1337191, new ItemKey(4, 11, 360, 161)},
				{1337192, new ItemKey(5, 41, 184, 177)},
				{1337193, new ItemKey(5, 44, 264, 161)},
				{1337194, new ItemKey(5, 14, 568, 177)},
				{1337195, new ItemKey(6, 17, 344, 433)},
				{1337196, new ItemKey(6, 14, 136, 177)},
				{1337197, new ItemKey(6, 25, 152, 145)},
				{1337198, new ItemKey(8, 36, 136, 145)},

				// 1337199 - 1337235 Reserved

				// Pyramid
				// Ancient Pyramid
				{1337236, new ItemKey(16, 5, 136, 192)}, //nightmare door
				// Temporal Gyre
				{1337237, new ItemKey(14, 9, 200, 125)},
				{1337238, new ItemKey(14, 7, 200, 205)},
				{1337239, new ItemKey(14, 14, 200, 832)},
				{1337240, new ItemKey(14, 17, 200, 832)},
				{1337241, new ItemKey(14, 20, 200, 832)},
				{1337242, new ItemKey(14, 8, 120, 176)},
				{1337243, new ItemKey(14, 9, 280, 176)},
				{1337244, new ItemKey(14, 6, 40, 208)},
				{1337245, new ItemKey(14, 7, 280, 208)},
				// Ancient Pyramid
				{1337246, new ItemKey(16, 14, 312, 192)},
				{1337247, new ItemKey(16, 3, 88, 192)},
				{1337248, new ItemKey(16, 22, 200, 192)},
				{1337249, new ItemKey(16, 16, 1512, 144)}
			};

			MapItemKeyToLocationId = new Dictionary<ItemKey, long>(MapLocationIdToItemKey.Count);

			foreach (var kvp in MapLocationIdToItemKey)
				MapItemKeyToLocationId.Add(kvp.Value, kvp.Key);
		}

		public static long GetLocationId(ItemKey key) =>
			MapItemKeyToLocationId.TryGetValue(key, out var locationId)
				? locationId
				: MapItemKeyToLocationId.TryGetValue(key.ToRoomItemKey(), out var roomLocationId)
					? roomLocationId
					: throw new Exception($"Key {key} does not map to an Archipelago itemlocation");

		public static ItemKey GetItemkey(long locationId) =>
			MapLocationIdToItemKey.TryGetValue(locationId, out var key)
				? key
				: throw new Exception($"Archipelago itemlocation {locationId} does not map to itemKey");
	}
}
