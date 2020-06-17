using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.LevelObjects
{
	[TimeSpinnerType("Timespinner.GameObjects.Events.Cutscene.CutsceneKeep1")]
	// ReSharper disable once UnusedMember.Global
	class CutsceneKeep1 : LevelObject
	{
		bool hasReplacedItemScript;

		dynamic Incubus => ((object)Object._incubus).AsDynamic();
		dynamic Succubus => ((object)Object._succubus).AsDynamic();

		public CutsceneKeep1(Mobile typedObject, ItemInfo itemInfo) : base(typedObject, itemInfo)
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
		
		//ShapeshifterBoss.PlaceKeycard
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
