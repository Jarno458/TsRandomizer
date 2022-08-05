using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace TsRandomizer
{
	struct SeedOptions
	{
		public const int Length = 8;

		public readonly uint Flags;

		public static SeedOptions None = new SeedOptions(0U);

		public bool StartWithJewelryBox => (Flags & 1 << 0) > 0;
		public bool ProgressiveVerticalMovement => (Flags & 1 << 1) > 0;
		public bool ProgressiveKeycard => (Flags & 1 << 2) > 0;
		public bool DownloadableItems => (Flags & 1 << 3) > 0;
		public bool EyeSpy => (Flags & 1 << 4) > 0;
		public bool StartWithMeyef => (Flags & 1 << 5) > 0;
		public bool StartWithTalaria => (Flags & 1 << 6) > 0;
		public bool SpecificKeys => (Flags & 1 << 7) > 0;
		public bool Inverted => (Flags & 1 << 8) > 0;
		public bool GasMaw => (Flags & 1 << 9) > 0;
		public bool GyreArchives => (Flags & 1 << 10) > 0;
		public bool Cantoran => (Flags & 1 << 11) > 0;
		public bool LoreChecks => (Flags & 1 << 12) > 0;
		public bool Tournament => (Flags & 1 << 13) > 0;

		//Non visable flags
		public bool Archipelago => (Flags & 1 << 16) > 0;

		public SeedOptions(uint flags)
		{
			Flags = flags;
		}

		public SeedOptions(Dictionary<string, object> slotData)
		{
			Flags = 1 << 16; //Archipelago

			var stringToFlagMapping = new Dictionary<string, uint>(11)
			{
				{"StartWithJewelryBox", 1U << 0},
				{"ProgressiveVerticalMovement", 1U << 1},
				{"ProgressiveKeycards", 1U << 2},
				{"DownloadableItems", 1U << 3},
				{"EyeSpy", 1U << 4},
				{"FacebookMode", 1U << 4}, //backward compatibility
				{"StartWithMeyef", 1U << 5},
				{"QuickSeed", 1U << 6},
				{"SpecificKeycards", 1U << 7},
				{"Inverted", 1U << 8},
				{"StinkyMaw", 1U << 9},
				{"GyreArchives", 1U << 10},
				{"Cantoran", 1U << 11},
				{"LoreChecks", 1U << 12},
				{"Tournament", 1U << 13}
			};

			foreach (var kvp in stringToFlagMapping)
			{
				var key = kvp.Key;
				var flag = kvp.Value;

				if (slotData.TryGetValue(key, out var value) && IsTrue(value))
					Flags |= flag;
			}
		}

		static bool IsTrue(object o)
		{
			if (o is bool b) return b;
			if (o is string s) return bool.Parse(s);
			if (o is int i) return i > 0;
			if (o is long l) return l > 0;

			return false;
		}

		public static bool TryParse(string seedString, out SeedOptions options)
		{
			if (seedString.Length == Seed.Length
				&& uint.TryParse(seedString.Substring(Seed.Length - Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedOptionsKey))
			{
				options = new SeedOptions(parsedOptionsKey);
				return true;
			}

			options = None;
			return false;
		}

		public override string ToString() =>
			Flags.ToString($"X{Length}");
	}
}
