using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Inventory;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.RoomTriggers.Triggers
{
	[RoomTriggerTrigger(2, 51)]
	class LibraryIfritWarp : RoomTrigger
	{
		static readonly Type YorneNpcType = TimeSpinnerType.Get("Timespinner.GameObjects.NPCs.YorneNPC");

		public override void OnRoomLoad(RoomState roomState)
		{
			if (!roomState.SeedOptions.GyreArchives) 
				return;

			if (roomState.Level.GameSave.HasFamiliar(EInventoryFamiliarType.Kobo))
			{
				// Portrait room to Ifrit
				RoomTriggerHelper.SpawnGyreWarp(roomState.Level, 200, 200); 
				return;
			};

			if (((Dictionary<int, NPCBase>)roomState.Level.AsDynamic()._npcs).Values.Any(npc => npc.GetType() == YorneNpcType)) 
				return;

			// Dialog for needing Kobo
			SpawnYorne(roomState.Level); 
		}

		public static void SpawnYorne(Level level)
		{
			var position = new Point(240, 215);
			var yorne = (NPCBase)YorneNpcType.CreateInstance(false, level, position, -1, new ObjectTileSpecification());

			level.AsDynamic().RequestAddObject(yorne);
		}
	}
}
