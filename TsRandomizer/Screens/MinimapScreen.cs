using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.MapMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class MinimapScreen : Screen
	{
		ItemLocationMap itemLocations;
		bool isShowingAviableLocations;

		LookupDictionairy<RoomItemKey, MinimapRoomState> preservedRoomStates;

		public MinimapScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			itemLocations = itemLocationMap;

			Dynamic._removeMarkerText = (string)Dynamic._removeMarkerText + " / Show where to go next";

			TweakMapBlocks();
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			var shouldShowItemLocationHints = input.IsPressSecondary(null);

			if ((isShowingAviableLocations && shouldShowItemLocationHints) 
			    || (!shouldShowItemLocationHints && !isShowingAviableLocations))
				return;

			if (!isShowingAviableLocations && shouldShowItemLocationHints)
			{
				MarkAvailableItemLocations();
				MarkBasicMapKnowledge();
				isShowingAviableLocations = true;
			}
			else
			{
				ResetMinimap();
				isShowingAviableLocations = false;
			}
		}

		public override void Unload()
		{
			ResetMinimap();
		}

		void MarkAvailableItemLocations()
		{
			preservedRoomStates = new LookupDictionairy<RoomItemKey, MinimapRoomState>(r => r.RoomKey);

			var visableAreas = (List<EMinimapEraType>)Dynamic._availableEras;
			var areas = ((MinimapSpecification)Dynamic._minimap).Areas;
			
			foreach (var itemLocation in GetAvailableItemLocations())
			{
				var roomKey = itemLocation.Key.ToRoomItemKey();
				if(preservedRoomStates.Contains(roomKey))
					continue;

				var room = areas.GetRoom(roomKey);

				MakeSureEraIsVisable(visableAreas, room);
				preservedRoomStates.Add(new MinimapRoomState(roomKey, room));

				foreach (var block in room.Blocks.Values)
				{
					if(block.IsSolidWall)
						continue;

					block.IsKnown = true;
					block.IsVisited = true;

					if (block.IsTransition || block.IsCheckpoint || block.IsBoss)
					{
						block.IsTransition = false;
						block.IsCheckpoint = false;
						block.RoomColor = EMinimapRoomColor.Yellow;
					}
					else
					{
						block.RoomColor = EMinimapRoomColor.Orange;
					}
				}
			}
		}

		void MarkBlockAsBossOrTimespinner(MinimapBlock block)
		{
			block.RoomColor = EMinimapRoomColor.DotRed;
			block.IsVisited = true;
		}

		void MarkBasicMapKnowledge()
		{
			// Reveal remaining map and show checkpoints/portals
			foreach (var area in ((MinimapSpecification)Dynamic._minimap).Areas)
			{
				foreach (var room in area.Rooms)
				{
					var roomKey = new RoomItemKey(area.LevelID, room.RoomID);
					if(preservedRoomStates.Contains(roomKey))
						continue;

					preservedRoomStates.Add(new MinimapRoomState(roomKey, room));

					room.SetKnown(true);
					foreach (var block in room.Blocks.Values)
					{
						if (block.IsCheckpoint || block.IsTransition)
						{
							block.IsVisited = true;
						}
						if (!block.IsVisited && (block.IsBoss || block.IsTimespinner))
						{
							MarkBlockAsBossOrTimespinner(block);
						}
					}
				}
			}
		}

		void ResetMinimap()
		{
			if(preservedRoomStates == null)
				return;

			var areas = ((MinimapSpecification)Dynamic._minimap).Areas;

			foreach (var roomState in preservedRoomStates)
				roomState.ApplyTo(areas.GetRoom(roomState.RoomKey));

			preservedRoomStates = null;
		}

		IEnumerable<ItemLocation> GetAvailableItemLocations()
		{
			var obtainedRequirements = itemLocations.GetAvailableRequirementsBasedOnObtainedItems();
			var locations = itemLocations.Where(l => !l.IsPickedUp && l.Gate.CanBeOpenedWith(obtainedRequirements));
			return locations;
		}

		static void MakeSureEraIsVisable(ICollection<EMinimapEraType> visableAreas, MinimapRoom room)
		{
			switch (room.DefaultColor)
			{
				case EMinimapRoomColor.Blue:
					if (!visableAreas.Contains(EMinimapEraType.Past))
						visableAreas.Add(EMinimapEraType.Past);
					break;
				case EMinimapRoomColor.Green:
					if (!visableAreas.Contains(EMinimapEraType.Other))
						visableAreas.Add(EMinimapEraType.Other);
					break;
			}
		}

		void TweakMapBlocks()
		{
			// Turn off display of SaveStatues broken to prevent softlocks
			foreach (var block in Dynamic._minimap.GetRoomFromLevelAndRoom(2,20).Blocks.Values)
			{
				block.IsCheckpoint = false;
			}
			foreach (var block in Dynamic._minimap.GetRoomFromLevelAndRoom(16,21).Blocks.Values)
			{
				block.IsCheckpoint = false;
			}

			// Mark missing boss rooms
			// Golden Idol 5,5
			foreach (var block in Dynamic._minimap.GetRoomFromLevelAndRoom(5,5).Blocks.Values)
			{
				block.IsBoss = true;
			}

			// The Maw Antechamber 8,13
			foreach (var block in Dynamic._minimap.GetRoomFromLevelAndRoom(8,13).Blocks.Values)
			{
				block.IsBoss = true;
			}

			// Xarion 9,7
			foreach (var block in Dynamic._minimap.GetRoomFromLevelAndRoom(9,7).Blocks.Values)
			{
				block.IsBoss = true;
			}

		}

		class MinimapRoomState
		{
			public readonly RoomItemKey RoomKey;

			readonly Dictionary<Point, MiniMapBlockState> blockStates;

			public MinimapRoomState(RoomItemKey key, MinimapRoom room)
			{
				RoomKey = key;
				blockStates = room.Blocks.ToDictionary(b => b.Key, b => new MiniMapBlockState(b.Value));
			}

			public void ApplyTo(MinimapRoom room)
			{
				foreach (var blockState in blockStates)
					blockState.Value.ApplyTo(room.Blocks[blockState.Key]);
			}
		}

		class MiniMapBlockState
		{
			readonly EMinimapRoomColor color;
			readonly bool isKnown;
			readonly bool isVisited;
			readonly bool isTransition;

			public MiniMapBlockState(MinimapBlock value)
			{
				color = value.RoomColor;
				isKnown = value.IsKnown;
				isVisited = value.IsVisited;
				isTransition = value.IsTransition;
			}

			public void ApplyTo(MinimapBlock block)
			{
				block.RoomColor = color;
				block.IsKnown = isKnown;
				block.IsVisited = isVisited;
				block.IsTransition = isTransition;
			}
		}
	}
}
