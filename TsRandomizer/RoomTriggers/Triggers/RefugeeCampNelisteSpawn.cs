using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(3, 0)]
	class RefugeeCampNelisteSpawn : RoomTrigger
	{
		static readonly Type NelisteNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.AstrologerNPC");

		public override void OnRoomLoad(RoomState roomState)
		{
			if (roomState.RoomItemLocation.IsPickedUp
			    || roomState.Level.GameSave.DataKeyBools.ContainsKey("HasUsedCityTS")
			    || !roomState.Level.GameSave.HasCutsceneBeenTriggered("Forest3_Haristel")
			    || ((Dictionary<int, NPCBase>)roomState.Level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == NelisteNpcType)) return;

			SpawnNeliste(roomState.Level);
		}
		
		public static void SpawnNeliste(Level level)
		{
			var position = new Point(720, 368);
			var neliste = (NPCBase)NelisteNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(neliste);
		}
	}
}
