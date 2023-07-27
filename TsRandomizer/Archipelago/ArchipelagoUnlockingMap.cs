using System.Linq;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameAbstractions.Saving;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.Randomisation;

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
					level.MarkRoomAsVisited(selectedGate.LevelId, selectedGate.LevelId);

					if (seed.Options.EnterSandman)
						level.MarkRoomAsVisited(PyramidTeleporterGates[1].LevelId, PyramidTeleporterGates[1].LevelId);
				};

				unlockingSpecification.Unlocks = selectedGate.Gate;

				if (seed.Options.EnterSandman)
					unlockingSpecification.AdditionalUnlocks |= PyramidTeleporterGates[1].Gate;
			}
			else
			{
				var pastGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSavePastPyramidsKeysUnlock);
				SetTeleporterPickupAction(allTeleporterGates, pastGate, CustomItemType.TimewornWarpBeacon);

				var presentGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSavePresentPyramidsKeysUnlock);
				SetTeleporterPickupAction(allTeleporterGates, presentGate, CustomItemType.ModernWarpBeacon);

				if (!seed.Options.EnterSandman)
					return;

				var timeGate = SlotDataParser.GetPyramidKeysGate(saveData, ArchipelagoItemLocationRandomizer.GameSaveTimePyramidsKeysUnlock);
				SetTeleporterPickupAction(allTeleporterGates, timeGate, CustomItemType.MysteriousWarpBeacon);
			}
		}
		
		void SetTeleporterPickupAction(TeleporterGate[] allGates, Requirement gateToUnlock, CustomItemType type)
		{
			var selectedGate = allGates.First(g => g.Gate == gateToUnlock);

			CustomItem.SetDescription(type, $"You feel the twin pyramid key attune to: {WarpNames.Get(selectedGate.Gate)}", "Twin Pyramid Key");

			var unlockingSpecification = new UnlockingSpecification(CustomItem.GetIdentifier(type), selectedGate.Gate)
			{
				OnPickup = level => level.MarkRoomAsVisited(selectedGate.LevelId, selectedGate.RoomId)
			};

			UnlockingSpecifications.Add(unlockingSpecification);
		}
	}
}