using System;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoItemLocationMap : ItemLocationMap
	{
		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options) : base(itemInfoProvider, itemUnlockingMap, options)
		{
		}

		public override bool IsBeatable()
		{
			return true; //Beatability is handled by Archipelago seed generator
		}

		public override void BaseOnSave(GameSave gameSave)
		{
		}

		public override ProgressionChain GetProgressionChain()
		{
			throw new InvalidOperationException("Progression chains arent supported for Archipelago seeds");
		}
	}
}
