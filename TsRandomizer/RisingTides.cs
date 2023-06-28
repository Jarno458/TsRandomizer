using System;
using System.Collections.Generic;

namespace TsRandomizer
{
	class RisingTides
	{
		public bool BasementHigh { get; set; }
		public bool Basement { get; set; }
		public bool Xarion { get; set; }
		public bool Maw { get; set; }
		public bool PyramidShaft { get; set; }
		public bool BackPyramid { get; set; }
		public bool CastleMoat { get; set; }
		public bool CastleCourtyard { get; set; }
		public bool LakeDesolation { get; set; }
		public bool DryLakeSerene { get; set; }

		public bool Lab { get; set; }
		public bool LakeSereneBridge { get; set; }

		// used for deserialization
		public RisingTides(){}

		public RisingTides(uint seedId, SeedOptions options)
		{
			if (!options.RisingTides)
				return;

			var random = new Random(~(int)seedId);

			BasementHigh = random.Next() % 2 == 0;
			Basement = random.Next() % 3 == 0;
			Xarion = random.Next() % 3 == 0;
			Maw = random.Next() % 3 == 0;
			PyramidShaft = random.Next() % 3 == 0;
			BackPyramid = random.Next() % 3 == 0;
			CastleMoat = random.Next() % 3 == 0;
			CastleCourtyard = random.Next() % 3 == 0;
			LakeDesolation = random.Next() % 3 == 0;
			DryLakeSerene = random.Next() % 3 == 0;

			Lab = true;
			LakeSereneBridge = true;
			DryLakeSerene = true;
		}

		public RisingTides(Dictionary<string, object> slotData)
		{
			if (slotData.TryGetValue("Basement", out var basementFlood))
			{
				Basement = ToInt(basementFlood) > 0;
				BasementHigh = ToInt(basementFlood) == 2;
			}
			if (slotData.TryGetValue("Xarion", out var xarion))
				Xarion = IsTrue(xarion);
			if (slotData.TryGetValue("Maw", out var maw))
				Maw = IsTrue(maw);
			if (slotData.TryGetValue("PyramidShaft", out var pyramidShaft))
				PyramidShaft = IsTrue(pyramidShaft);
			if (slotData.TryGetValue("BackPyramid", out var backPyramid))
				BackPyramid = IsTrue(backPyramid);
			if (slotData.TryGetValue("CastleMoat", out var castleMoat))
				CastleMoat = IsTrue(castleMoat);
			if (slotData.TryGetValue("CastleCourtyard", out var castleCourtyard))
				CastleCourtyard = IsTrue(castleCourtyard);
			if (slotData.TryGetValue("LakeDesolation", out var lakeDesolation))
				LakeDesolation = IsTrue(lakeDesolation);
			if (slotData.TryGetValue("DryLakeSerene", out var dryLakeSerene))
				DryLakeSerene = IsTrue(dryLakeSerene);
			if (slotData.TryGetValue("Lab", out var lab))
				LakeDesolation = IsTrue(lab);
			if (slotData.TryGetValue("LakeSereneBridge", out var lakeSereneBridge))
				DryLakeSerene = IsTrue(lakeSereneBridge);
		}

		static bool IsTrue(object o)
		{
			if (o is bool b) return b;
			if (o is string s) return bool.Parse(s);
			if (o is int i) return i > 0;
			if (o is long l) return l > 0;

			return false;
		}

		static int ToInt(object o, int fallback = 0)
		{
			if (o is int i) return i;
			if (o is long l) return (int)l;

			return fallback;
		}
	}
}