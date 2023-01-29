using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;
using TsRandomizer.Randomisation.ItemPlacers;

namespace TsRandomizer.Archipelago
{
	class ArchipelagoUnlockingMap : ItemUnlockingMap
	{
		public ArchipelagoUnlockingMap(Seed seed, GameSave saveGame) : base(seed)
		{
			var allTeleporterGates = PresentTeleporterGates
				.Union(PastTeleporterGates)
				.Union(PyramidTeleporterGates)
				.ToArray();

			var saveData = saveGame.DataKeyStrings.ToDictionary(k => k.Key, v => (object)v.Value);

			if (!seed.Options.UnchainedKeys)
			{
				var gateToUnlock = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSavePyramidsKeysUnlock);

				var unlockingSpecification = UnlockingSpecifications[new ItemIdentifier(EInventoryRelicType.PyramidsKey)];

				var selectedGate = allTeleporterGates.First(g => g.Gate == gateToUnlock);

				unlockingSpecification.OnPickup = level => {
					UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);

					if (seed.Options.EnterSandman)
					{
						UnlockFirstPyramidPortal(level);

						unlockingSpecification.AdditionalUnlocks |= PyramidTeleporterGates[1].Gate;
					}
				};

				unlockingSpecification.Unlocks = selectedGate.Gate;
			}
			else
			{
				var pastGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSavePastPyramidsKeysUnlock);
				SetTeleporterPickupAction(seed, allTeleporterGates, pastGate, CustomItem.GetIdentifier(CustomItemType.TimewornWarpBeacon));

				var presentGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSavePresentPyramidsKeysUnlock);
				SetTeleporterPickupAction(seed, allTeleporterGates, presentGate, CustomItem.GetIdentifier(CustomItemType.ModernWarpBeacon));

				if (!seed.Options.EnterSandman)
					return;

				var timeGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSaveTimePyramidsKeysUnlock);
				SetTeleporterPickupAction(seed, allTeleporterGates, timeGate, CustomItem.GetIdentifier(CustomItemType.MysteriousWarpBeacon));
			}
		}
		
		void SetTeleporterPickupAction(Seed seed, TeleporterGate[] allGates, Requirement gateToUnlock, ItemIdentifier item)
		{
			var selectedGate = allGates.First(g => g.Gate == gateToUnlock);

			var unlockingSpecification = new UnlockingSpecification(item, Requirement.None, Requirement.Teleport);

			unlockingSpecification.OnPickup = level => {
				UnlockRoom(level, selectedGate.LevelId, selectedGate.RoomId);
			};

			unlockingSpecification.Unlocks = selectedGate.Gate;

			UnlockingSpecifications.Add(unlockingSpecification);
		}
	}
}