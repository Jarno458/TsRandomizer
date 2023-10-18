using Timespinner.Core.Specifications;

namespace TsRandomizer.Extensions
{
	public static class MinimapSpecificationExtensions
	{
		public static void SetAllKnown(this MinimapSpecification minimapSpec, bool value)
		{
			foreach (MinimapArea area in minimapSpec.Areas)
				foreach (MinimapRoom room in area.Rooms)
					room.SetKnown(value);
		}
	}
}
