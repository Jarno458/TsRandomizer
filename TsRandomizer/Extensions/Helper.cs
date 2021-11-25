using System;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;

namespace TsRandomizer.Extensions
{
	static class Helper
	{
		public static EInventoryOrbType[] GetAllOrbs()
		{
			return ((EInventoryOrbType[])Enum.GetValues(typeof(EInventoryOrbType)))
				.Where(o => o != EInventoryOrbType.None && o != EInventoryOrbType.Monske)
				.ToArray();
		}
	}
}
