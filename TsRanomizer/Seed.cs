using System;
using System.Globalization;

namespace TsRanodmizer
{
	class Seed
	{
		public static Seed Current;

		readonly uint value;

		static Seed()
		{
			Current = new Seed();
		}

		public Seed(uint seed)
		{
			value = seed;
		}

		public Seed() : this(new Random().Next())
		{
		}

		public Seed(int seed) : this((uint)seed)
		{
		}

		public static bool TrySetFromText(string text)
		{
			if (!uint.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint parsedValue))
				return false;

			Current = new Seed(parsedValue);
			return true;
		}

		public override string ToString()
		{
			return value.ToString("X8");
		}

		public static implicit operator int(Seed seed)
		{
			return (int)seed.value;
		}

		public static implicit operator uint(Seed seed)
		{
			return seed.value;
		}

		public static implicit operator Seed(uint seed)
		{
			return new Seed(seed);
		}

		public static implicit operator Seed(int seed)
		{
			return new Seed(seed);
		}
	}
}
