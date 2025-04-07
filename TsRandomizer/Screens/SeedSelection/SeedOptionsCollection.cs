using System.Collections.Generic;
using System.Linq;
using Timespinner.GameAbstractions.Inventory;

namespace TsRandomizer.Screens.SeedSelection
{
	class SeedOptionsCollection : InventoryRelicCollection
	{
		static readonly Dictionary<int, SeedOptionInfo> Options = new Dictionary<int, SeedOptionInfo>
		{
			{ 1 << 0, new SeedOptionInfo { Name = "Start with Jewelry Box", Description = "Start with Jewelry Box unlocked." } },
			{ 1 << 1, new SeedOptionInfo { Name = "Progressive Vertical Movement", Description = "Always find vertical movement in the following order: Succubus Hairpin -> Light Wall -> Celestial Sash." } },
			{ 1 << 2, new SeedOptionInfo { Name = "Progressive Keycards", Description = "Always find Security Keycards in the following order D -> C -> B -> A." } },
			{ 1 << 3, new SeedOptionInfo { Name = "Downloadable Items", Description = "With the tablet you will be able to download items from terminals." } },
			{ 1 << 4, new SeedOptionInfo { Name = "Eye Spy", Description = "Requires Oculus Ring in inventory to be able to break hidden walls." } },
			{ 1 << 5, new SeedOptionInfo { Name = "Start with Meyef", Description = "Start with Meyef, ideal for when you want to play multiplayer" } },
			{ 1 << 6, new SeedOptionInfo { Name = "Quick Seed", Description = "Start with Talaria Attachment, Nyoom!" } },
			{ 1 << 7, new SeedOptionInfo { Name = "Specific Keycards", Description = "Keycards can only open corresponding doors." } },
			{ 1 << 8, new SeedOptionInfo { Name = "Inverted", Description = "Start in the past." } },
			{ 1 << 9, new SeedOptionInfo { Name = "Stinky Maw", Description = "Require Gas Mask for Maw." } },
			{ 1 << 10, new SeedOptionInfo { Name = "Gyre Archives", Description = "Temporal Gyre locations are in logic. New warps are gated by Merchant Crow and Kobo." } },
			{ 1 << 11, new SeedOptionInfo { Name = "Cantoran", Description = "Cantoran's fight and check are available upon revisiting his room." } },
			{ 1 << 12, new SeedOptionInfo { Name = "Lore Checks", Description = "Memories in the present and letters in the past contain items." } },
			{ 1 << 13, new SeedOptionInfo { Name = "Tournament", Description = "Forces your settings to be the predefined tournament settings." } },
			{ 1 << 15, new SeedOptionInfo { Name = "Enter Sandman", Description = "Ancient Pyramid is unlocked by the Twin Pyramid Key, but the final boss door opens if you have all 5 Timespinner pieces." } },
			{ 1 << 17, new SeedOptionInfo { Name = "Dad Percent", Description = "The win condition is beating the boss of Emperor's Tower" } },
			{ 1 << 18, new SeedOptionInfo { Name = "Rising Tides", Description = "Random areas are flooded or drained" } },
			{ 1 << 19, new SeedOptionInfo { Name = "Unchained Keys", Description = "Start with Twin Pyramid Key, which does not give free warp; warp items for Past, Present, (and ??? with Enter Sandman) can be found." } },
			{ 1 << 20, new SeedOptionInfo { Name = "Trapped Chests", Description = "Items can be traps. Toggle available traps in the 'Traps' settings." } },
			{ 1 << 21, new SeedOptionInfo { Name = "Back to the Future", Description = "When inverted, allows using the refugee camp warp when both the Timespinner Wheel and Spindle are acquired." } },
			{ 1 << 22, new SeedOptionInfo { Name = "Prism Break", Description = "Lasers blocking the Military Hangar are disabled via items, rather than by defeating bosses." } },
			{ 1 << 23, new SeedOptionInfo { Name = "Lock Key Amadeus", Description = "Lasers in Amadeus' Laboratory are disabled via items, rather than by de-powering the lab. Experiments will spawn in the lab." } },
			{ 1 << 24, new SeedOptionInfo { Name = "Risky Warps", Description = "Expanded free-warp eligible locations, including Azure Queen, Xarion, Amadeus' Laboratory, and Emperor's Tower." } },
			{ 1 << 25, new SeedOptionInfo { Name = "Pyramid Start", Description = "Start in ???. Takes priority over Inverted. Additional chests in Dark Forest and Pyramid. Sandman door behaves as it does in Enter Sandman." } },
			{ 1 << 26, new SeedOptionInfo { Name = "Gate Keep", Description = "The castle drawbridge starts raised, and can be lowered via item." } },
			{ 1 << 27, new SeedOptionInfo { Name = "Royal Roadblock", Description = "The Royal Towers entrance door requires a royal orb (Plasma Orb, Plasma Geyser, or Royal Ring) to enter." } },
			{ 1 << 28, new SeedOptionInfo { Name = "Pure Torcher", Description = "All lanterns contain checks. (Except tutorial)" } },
		};

		public SeedOptionsCollection(SeedOptions seedOptions)
		{
			foreach (var option in Options)
			{
				AddItem(option.Key);

				Inventory[option.Key].IsActive = (seedOptions.Flags & option.Key) > 0;
			}
		}

		public static SeedOptionInfo GetSeedOptionInfo(int option) =>
			Options[option];

		public sealed override void AddItem(int item) =>
			base.AddItem(item);

		public static implicit operator SeedOptions(SeedOptionsCollection collection)
		{
			uint flags = 0;

			for (var i = 0; i < Options.Count; i++)
			{
				var optionKey = Options.Keys.ElementAt(i);
				var isActive = collection.Inventory[optionKey].IsActive;

				if (isActive)
					flags |= (uint)optionKey;
			}

			return new SeedOptions(flags);
		}

		public override string ToString() =>
			((SeedOptions)this).ToString();
	}
}
