using System;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events.Doors;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.Other
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Doors.KeycardDoorEvent")]
	// ReSharper disable once UnusedMember.Global
	class KeycardDoorEvent : LevelObject
	{
		bool requiresSpecificKey;

		public KeycardDoorEvent(Mobile typedObject) : base(typedObject)
		{
		}

		protected override void Initialize(Seed seed)
		{
			requiresSpecificKey = seed.Options.SpecificKeys;
		}

		protected override void OnUpdate(GameplayScreen gameplayScreen)
		{
			if(!requiresSpecificKey || Level.GameSave.GetSaveBool(Dynamic.GetSaveKey))
				return;

			var requirementToOpenDoor = GetCorrespondingRequirement(Dynamic._keycardType);

			Dynamic._isOpened = !gameplayScreen.ItemLocations
				.GetAvailableRequirementsBasedOnObtainedItems().Contains(requirementToOpenDoor);
		}

		Requirement GetCorrespondingRequirement(EKeycardType keycardType)
		{
			switch (keycardType)
			{
				case EKeycardType.A_Black:
				case EKeycardType.A_Broken:
					return Requirement.CardA;
				case EKeycardType.B_Red:
					return Requirement.CardB;
				case EKeycardType.C_Green:
					return Requirement.CardC;
				case EKeycardType.D_Blue:
					return Requirement.CardD;
				case EKeycardType.V_Pink:
					return Requirement.CardV;
				default:
					throw new ArgumentOutOfRangeException(nameof(keycardType), keycardType, null);
			}
		}
	}
}
