using System.Globalization;

namespace TsRandomizer
{
	struct SeedOptions
	{
		public const int Length = 2;

		public readonly uint Flags;

		public static SeedOptions None = new SeedOptions(0U);

		public bool StartWithJewelryBox => (Flags & 1 << 0) > 0;
		public bool ProgressiveVerticalMovement => (Flags & 1 << 1) > 0;
		public bool ProgressiveKeycard => (Flags & 1 << 2) > 0;
		public bool DownloadableItems => (Flags & 1 << 3) > 0;
		public bool RequireEyeOrbRing => (Flags & 1 << 4) > 0;
		public bool StartWithMeyef => (Flags & 1 << 5) > 0; 
		public bool StartWithTalaria => (Flags & 1 << 6) > 0;
		public bool SpecificKeys => (Flags & 1 << 7) > 0;

		public SeedOptions(uint flags)
		{
			Flags = flags;
		}

		public static bool TryParse(string seedString, out SeedOptions options)
		{
			if (seedString.Length == Seed.Length
			    && uint.TryParse(seedString.Substring(Seed.Length - Length, Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedOptionsKey))
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