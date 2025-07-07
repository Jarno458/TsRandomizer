using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;


namespace TsRandomizer.RoomTriggers.Triggers.Bosses
{
	[RoomTriggerTrigger(7, 5)]
	class Cantoran : BossRoomTrigger
	{
		static readonly Type CantoranNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.Misc.AelanaNPC");
		static readonly Type OrbPedestalEventType = TimeSpinnerType.Get("Timespinner.GameObjects.Events.Treasure.OrbPedestalEvent");
		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.Seed.Options.Cantoran)
				return;

			// Spawn Warp Prompt
			if (!roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Cantoran") &&
				roomState.Level.GameSave.GetSaveBool("TSRando_IsPinkBirdDead"))
				SpawnCantoranPrompt(roomState.Level);
			// Mark the Pink Bird as being fought
			if (!roomState.Level.GameSave.GetSaveBool("TSRando_IsPinkBirdDead"))
				roomState.Level.GameSave.SetValue("TSRando_IsPinkBirdDead", true);

			var eventTypes = ((Dictionary<int, GameEvent>)roomState.Level.AsDynamic()._levelEvents).Values.Select(e => e.GetType());
			if (!roomState.RoomItemLocation.IsPickedUp
					&& roomState.Level.GameSave.GetSaveBool("TSRando_IsBossDead_Cantoran")
					// On first load, the event will not be in the events list, but will be in the new objects list
					&& !(eventTypes.Contains(OrbPedestalEventType) || roomState.Level.AsDynamic()._newObjects.Count > 0))
				RoomTriggerHelper.SpawnItemDropPickup(roomState.Level, roomState.RoomItemLocation.ItemInfo, 170, 194);
		}

		public static void SpawnCantoranPrompt(Level level)
		{
			var position = new Point(280, 200);
			var cantoran  = (NPCBase)CantoranNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());
			cantoran.AsDynamic()._npcTriggerType = NPCBase.ENPCTriggerType.Talk;
			cantoran.AsDynamic()._sprite = level.GCM.SpCantoranBoss;
			cantoran.SpriteFrameOffset = -62;

			level.AsDynamic().RequestAddObject(cantoran);

			RoomTriggerHelper.SpawnGlowingFloor(level, new Point(200, 210));
		}
	}
}
