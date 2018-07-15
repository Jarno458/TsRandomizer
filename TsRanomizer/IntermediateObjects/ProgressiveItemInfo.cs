namespace TsRanodmizer.IntermediateObjects
{
	class ProgressiveItemInfo : ItemInfo
	{
		readonly ItemInfo[] itemProgression;

		int progressionState;

		public ProgressiveItemInfo(params ItemInfo[] items)
		{
			itemProgression = items;

			LootType = itemProgression[progressionState].LootType;
			ItemId = itemProgression[progressionState].ItemId;
		}

		public override void IsObtained()
		{
			itemProgression[progressionState].IsObtained();

			progressionState++;

			LootType = itemProgression[0].LootType;
			ItemId = itemProgression[0].ItemId;
		}
	}
}