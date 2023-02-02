using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.LevelObjects.ItemManipulators
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneKeep1")]
	// ReSharper disable once UnusedMember.Global
	class CutsceneKeep1 : ItemManipulator
	{
		bool hasReplacedItemScript;

		dynamic Incubus => ((object)Dynamic._incubus).AsDynamic();
		dynamic Succubus => ((object)Dynamic._succubus).AsDynamic();

		public CutsceneKeep1(Mobile typedObject, GameplayScreen gameplayScreen, ItemLocation itemLocation) 
			: base(typedObject, gameplayScreen, itemLocation)
		{
		}

		protected override void OnUpdate()
		{
			if (ItemInfo == null || hasReplacedItemScript)
				return;

			var rewardItemDelegate = Scripts.Single(s => s.AsDynamic().ScriptType == EScriptType.Delegate);
			rewardItemDelegate.AsDynamic().Delegate = new Action(RemoveDemonsAndSpawnItem);

			hasReplacedItemScript = true;
		}
		
		void RemoveDemonsAndSpawnItem()
		{
			Incubus.Disappear();
			Succubus.Disappear();

			var sandStreamerEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.SandStreamerEvent");
			var sandStreamerEnumType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Misc.ESandStreamerType");
			var bossDeathEnumValue = Enum.Parse(sandStreamerEnumType, "BossDeath");
			var sandStreamerEvent = Activator.CreateInstance(sandStreamerEventType, Level, ((Rectangle)Incubus.Bbox).Center, bossDeathEnumValue);

			Level.AddEvent((GameEvent)sandStreamerEvent);

			var itemDropPickupType = TimeSpinnerType.Get("Timespinner.GameObjects.Items.ItemDropPickup");
			var itemPosition = new Point(((Point)Succubus.Position).X, ((Rectangle)Succubus.Bbox).Top);
			var itemDropPickup = Activator.CreateInstance(itemDropPickupType, ItemInfo.BestiaryItemDropSpecification, Level, itemPosition, -1);

			LevelReflected.RequestAddObject((Item)itemDropPickup);
		}
	}
}
