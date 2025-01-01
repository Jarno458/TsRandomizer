using System;
using System.Collections.Generic;
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
		public bool EnterSandman => (Flags & 1 << 15) > 0;
		public bool Archipelago => (Flags & 1 << 16) > 0;
		public bool DadPercent => (Flags & 1 << 17) > 0;
		public bool RisingTides => (Flags & 1 << 18) > 0;
		public bool UnchainedKeys => (Flags & 1 << 19) > 0;
		public bool TrappedChests => (Flags & 1 << 20) > 0;
		public bool BackToTheFuture => (Flags & 1 << 21) > 0;
		public bool PrismBreak => (Flags & 1 << 22) > 0;
		public bool LockKeyAmadeus => (Flags & 1 << 23) > 0;
		public bool RiskyWarps => (Flags & 1 << 24) > 0;
		public bool PyramidStart => (Flags & 1 << 25) > 0;

		public SeedOptions(uint flags)
		{
			Flags = flags;
		}

		public SeedOptions(Dictionary<string, object> slotData)
		{
			Flags = 1 << 16; //Archipelago

			var stringToFlagMapping = new Dictionary<string, uint>(25)
			{
				{"StartWithJewelryBox", 1U << 0},
				{"ProgressiveVerticalMovement", 1U << 1},
				{"ProgressiveKeycards", 1U << 2},
				{"DownloadableItems", 1U << 3},
				{"EyeSpy", 1U << 4},
				{"StartWithMeyef", 1U << 5},
				{"QuickSeed", 1U << 6},
				{"SpecificKeycards", 1U << 7},
				{"Inverted", 1U << 8},
				{"StinkyMaw", 1U << 9},
				{"GyreArchives", 1U << 10},
				{"Cantoran", 1U << 11},
				{"LoreChecks", 1U << 12},
				{"Tournament", 1U << 13},
				// 14 FastPyramid, merged with EnterSandman
				{"EnterSandman", 1U << 15},
				// 16 Archipelago, automaticly set above
				{"DadPercent", 1U << 17},
				{"RisingTides", 1U << 18},
				{"UnchainedKeys", 1U << 19},
				{"TrappedChests", 1U << 20},
				{"PresentAccessWithWheelAndSpindle", 1U << 21}, // Alternative flag name for backwards compatability
				{"BackToTheFuture", 1U << 21},
				{"PrismBreak", 1U << 22},
				{"LockKeyAmadeus", 1U << 23},
				{"RiskyWarps", 1U << 24},
				{"PyramidStart", 1U << 25},
			};

			foreach (var kvp in stringToFlagMapping)
			{
				var key = kvp.Key;
				var flag = kvp.Value;

				if (slotData.TryGetValue(key, out var value) && IsTrue(value))
					Flags |= flag;
			}
		}

		public static SeedOptions CreateRandom()
		{
			var randomValue = (uint)new Random().Next();
			randomValue &= ~(1U << 13); //Tournament

			return new SeedOptions(randomValue);
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
