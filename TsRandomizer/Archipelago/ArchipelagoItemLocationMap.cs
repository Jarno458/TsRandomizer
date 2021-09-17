using System;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoItemLocationMap : ItemLocationMap
	{
		readonly ItemInfoProvider itemInfoProvider;

		public ArchipelagoItemLocationMap(ItemInfoProvider itemInfoProvider, ItemUnlockingMap itemUnlockingMap, SeedOptions options) : base(itemInfoProvider, itemUnlockingMap, options)
		{
			this.itemInfoProvider = itemInfoProvider;
		}

		public override bool IsBeatable()
		{
			return true; //Beatability is handled by Archipelago seed generator
		}

		public override void BaseOnSave(GameSave gameSave)
		{
			base.BaseOnSave(gameSave);


			//var x = new ItemIdentifier(EItemType.M);

		}

		public override ProgressionChain GetProgressionChain()
		{
			throw new InvalidOperationException("Progression chains arent supported for Archipelago seeds");
		}

		public void RecieveItem(ItemIdentifier itemIdentifier)
		{
			var key = new ItemKey(0, 0, 0, ItemMap.GetItemId(itemIdentifier));

			Add(key, "Archipelago Recieved", null);

			this[key].SetItem(itemInfoProvider.Get(itemIdentifier));

			this[key].SetPickedUp();
			//this[key].

			//set pickedup and yield item to player
		}
	}
}
