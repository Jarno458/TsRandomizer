using System.Globalization;

namespace TsRandomizer
{
	struct SeedOptions
	{
		public const int Length = 2;

		public readonly uint Flags;

		public static SeedOptions None = new SeedOptions(0U);

		public bool StartWithJewelryBox => (Flags & 1) > 0;
		public bool ProgressiveVerticalMovement => (Flags & 2) > 0;
		public bool ProgressiveKeycard => (Flags & 4) > 0;

		public SeedOptions(uint flags)
		{
			Flags = flags;
		}

		public static bool TryParse(string seedString, out SeedOptions options)
		{
			if (seedString.Length == Seed.Length
			    && uint.TryParse(seedString.Substring(8, Length), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out var parsedOptionsKey))
			{
				options = new SeedOptions(parsedOptionsKey);
				return true;
			}

			options = None;
			return false;
		}

		public override string ToString()
		{
			return Flags.ToString($"X{Length}");
		}
	}
}