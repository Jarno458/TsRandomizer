﻿using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	abstract class LaserAccessKey : CustomItem
	{
		public override int AnimationIndex => 46;

		protected override bool RemoveFromInventory => false;

		public LaserAccessKey(ItemUnlockingMap unlockingMap, CustomItemType itemType) : base(unlockingMap, itemType)
		{
		}

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);
			level.JukeBox.PlayCue(Timespinner.GameAbstractions.ESFX.EnvLabPowerDown);
		} 
	}
	
	class LaserAccessA : LaserAccessKey
	{
		public LaserAccessA(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessA)
		{
			SetDescription("ALEANNA-class laser operating manual. Includes deactivaton codes.", null);
		}
	}

	class LaserAccessI : LaserAccessKey
	{
		public LaserAccessI(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessI)
		{
			SetDescription("IDOL-class laser operating manual. Includes deactivaton codes.", null);
		}
	}

	class LaserAccessM : LaserAccessKey
	{
		public LaserAccessM(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LaserAccessM)
		{
			SetDescription("MAW-class laser operating manual. Includes deactivaton codes.", null);
		}
	}

	class LabAccessGenza : LaserAccessKey
	{
		public override int AnimationIndex => new ItemIdentifier(EInventoryEquipmentType.LabGlasses).GetAnimationIndex();
		public LabAccessGenza(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LabAccessGenza)
		{
			SetDescription("Access credentials for Genza's personal wing of the laboratory.", null);
		}
	}

	class LabAccessExperiment : LaserAccessKey
	{
		public override int AnimationIndex => new ItemIdentifier(EInventoryFamiliarType.Demon).GetAnimationIndex();
		public LabAccessExperiment(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LabAccessExperiment)
		{
			SetDescription("Access credentials for Experiment 13's containment area.", null);
		}
	}

	class LabAccessResearch: LaserAccessKey
	{
		public override int AnimationIndex => new ItemIdentifier(EInventoryEquipmentType.LabCoat).GetAnimationIndex();
		public LabAccessResearch(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LabAccessResearch)
		{
			SetDescription("Access credentials for the lower research wing of the laboratory.", null);
		}
	}

	class LabAccessDynamo : LaserAccessKey
	{
		public override int AnimationIndex => new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Melee).GetAnimationIndex();
		public LabAccessDynamo(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LabAccessDynamo)
		{
			SetDescription("Access credentials for the lab's power maintenance room.", null);
		}
	}
}
