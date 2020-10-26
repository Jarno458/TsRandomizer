using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;

namespace TsRandomizer.Screens.SeedSelection
{
	class SeedOptionsCollection : InventoryRelicCollection
	{
		static readonly Dictionary<int, SeedOptionInfo> Options = new Dictionary<int, SeedOptionInfo>
		{
			{ 1, new SeedOptionInfo { Name = "Start with Jewelry Box", Description = "Start with Jewelry Box unlocked" } },
			{ 2, new SeedOptionInfo { Name = "Progressive vertical movement", Description = "Always find vertical movement in the following order Succubus Hairpin -> Celestial Sash -> Light Wall" } },
			{ 4, new SeedOptionInfo { Name = "Progressive keycards", Description = "Always find Security Keycard's in the following order D -> C -> B -> A" } },
			//{ 8, new SeedOptionInfo { Name = "Memorysanity", Description = "" } },
		};

		public SeedOptionsCollection(SeedOptions seedOptions)
		{
			foreach (var option in Options)
			{
				AddItem(option.Key);

				Inventory[option.Key].IsActive = (seedOptions.Flags & option.Key) > 0;
			}
		}

		public static SeedOptionInfo GetSeedOptionInfo(int option)
		{
			return Options[option];
		}

		public sealed override void AddItem(int item)
		{
			base.AddItem(item);
		}

		public static implicit operator SeedOptions(SeedOptionsCollection collection)
		{
			uint flags = 0;

			for (var i = 0; i < Options.Count; i++)
			{
				var optionKey = Options.Keys.ElementAt(i);
				var isActive = collection.Inventory[optionKey].IsActive;

				flags |= (isActive ? 1U : 0U) << i;
			}

			return new SeedOptions(flags);
		}

		public override string ToString()
		{
			return ((SeedOptions)this).ToString();
		}
	}
}