using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.Core.Specifications.Minimap;
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
		static readonly Roomkey[] DisabledCheckpoints =
		{
			new Roomkey(2, 20),
			new Roomkey(16, 21)
		};

		static readonly Roomkey[] BossRooms =
		{
			new Roomkey(5, 5),
			new Roomkey(8, 13),
			new Roomkey(9, 7)
		};

		static readonly Roomkey[] GyreRooms =
		{
			// Gyre Path
			new Roomkey(14, 11),
			new Roomkey(14, 12),
			new Roomkey(14, 13),
			new Roomkey(14, 14),
			new Roomkey(14, 15),
			new Roomkey(14, 16),
			new Roomkey(14, 17),
			new Roomkey(14, 18),
			new Roomkey(14, 19),
			new Roomkey(14, 20),
			new Roomkey(14, 21),
			new Roomkey(14, 22),
			new Roomkey(14, 23),
			// Ravenlord
			new Roomkey(14, 8),
			new Roomkey(14, 4),
			new Roomkey(14, 9),
			new Roomkey(14, 24),
			// Ifrit
			new Roomkey(14, 6),
			new Roomkey(14, 5),
			new Roomkey(14, 7),
			new Roomkey(14, 25)
		};

		static readonly Roomkey[] FalseWarpRooms =
		{
			new Roomkey(11, 4), // Lab cabinet
			new Roomkey(2, 51) // Backer memory room
		};

		ItemLocationMap itemLocations;
		bool isShowingAviableLocations;

		LookupDictionairy<Roomkey, MinimapRoomState> preservedRoomStates;

		MinimapSpecification Minimap => ((object)Dynamic._minimapHud).AsDynamic()._minimap;

		public MinimapScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		static MinimapSpecification DeepClone(MinimapSpecification spec)
		{
			var clone = new MinimapSpecification();

			foreach (var area in spec.Areas)
			{
				var cloneArea = new MinimapArea
				{
					DefaultColor = area.DefaultColor,
					LevelID = area.LevelID
				};

				foreach (var room in area.Rooms)
				{
					var cloneRoom = new MinimapRoom(cloneArea)
					{
						IsDebug = room.IsDebug,
						DefaultColor = room.DefaultColor,
						RoomID = room.RoomID,
						Width = room.Width,
						Height = room.Height,
						Position = room.Position
					};


					foreach (var kvp in room.Blocks)
					{
						var point = kvp.Key;
						var block = kvp.Value;

						var cloneBlock = new MinimapBlock(cloneRoom)
						{
							IsKnown = block.IsKnown,
							IsVisited = block.IsVisited,
							IsBoss = block.IsBoss,
							IsTimespinner = block.IsTimespinner,
							IsTransition = block.IsTransition,
							IsCheckpoint = block.IsCheckpoint,
							IsSolidWall = block.IsSolidWall,
							HasSolidWallToNW = block.HasSolidWallToNW,
							RoomColor = block.RoomColor,
							Position = block.Position
						};

						var dynamicCloneBlock = cloneBlock.AsDynamic();

						dynamicCloneBlock._walls = block.Walls;
						dynamicCloneBlock._doors = block.Doors;
						dynamicCloneBlock._secretDoors = block.SecretDoors;

						cloneRoom.Blocks.Add(point, cloneBlock);
					}

					cloneArea.Rooms.Add(cloneRoom);
				}

				clone.Areas.Add(cloneArea);
			}

			foreach (var kvp in spec.Markers)
			{
				var point = kvp.Key;
				var marker = kvp.Value;

				var cloneMarker = new MinimapMarker
				{
					IsVisible = marker.IsVisible,
					EraColor = marker.EraColor,
					MarkerColor = marker.MarkerColor,
					Location = marker.Location,
					DrawLocationPosition = marker.DrawLocationPosition
				};

				clone.Markers.Add(point, cloneMarker);
			}

			foreach (var revealGroup in spec.RevealGroups)
			{
				var cloneRevealGroup = new MinimapRevealGroup
				{
					ID = revealGroup.ID
				};

				var cloneRoomList = (List<int>)cloneRevealGroup.AsDynamic()._rooms;
				foreach (var room in revealGroup.Rooms)
					cloneRoomList.Add(room);

				clone.RevealGroups.Add(cloneRevealGroup);
			}

			clone.PopulateAllBlocks();

			return clone;
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			var hud = ((object) Dynamic._minimapHud).AsDynamic();

			foreach (var roomkey in GyreRooms)
				GetRoom(roomkey).IsDebug = false;

			hud._minimap = DeepClone(Dynamic._minimap);

			itemLocations = itemLocationMap;

			Dynamic._removeMarkerText = (string)Dynamic._removeMarkerText + " / Show where to go next";

			foreach (var roomkey in DisabledCheckpoints)
				foreach (var block in GetRoom(roomkey).Blocks.Values)
					block.IsCheckpoint = false;

			foreach (var roomkey in BossRooms)
				foreach (var block in GetRoom(roomkey).Blocks.Values)
					block.IsBoss = true;

			foreach (var roomkey in FalseWarpRooms)
				foreach (var block in GetRoom(roomkey).Blocks.Values)
					block.IsTimespinner = true;
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
			preservedRoomStates = new LookupDictionairy<Roomkey, MinimapRoomState>(r => r.RoomKey);

			var visableAreas = (List<EMinimapEraType>)Dynamic._availableEras;

			foreach (var itemLocation in GetAvailableItemLocations())
			{
				var roomKey = new Roomkey(itemLocation.Key.LevelId, itemLocation.Key.RoomId); //somehow they keys dont match if we use itemLocation.Key directly
				if (preservedRoomStates.Contains(roomKey))
					continue;

				var room = GetRoom(roomKey);

				MakeSureEraIsVisable(visableAreas, room);
				preservedRoomStates.Add(new MinimapRoomState(roomKey, room));

				foreach (var block in room.Blocks.Values)
				{
					if (block.IsSolidWall)
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

		static void MarkBlockAsBossOrTimespinner(MinimapBlock block)
		{
			block.RoomColor = EMinimapRoomColor.DotRed;
			block.IsVisited = true;
		}

		void MarkBasicMapKnowledge()
		{
			foreach (var area in Minimap.Areas)
			{
				foreach (var room in area.Rooms)
				{
					var roomKey = new Roomkey(area.LevelID, room.RoomID);
					if (preservedRoomStates.Contains(roomKey))
						continue;

					preservedRoomStates.Add(new MinimapRoomState(roomKey, room));

					room.SetKnown(true);

					foreach (var block in room.Blocks.Values)
					{
						if (block.IsCheckpoint || block.IsTransition)
							block.IsVisited = true;

						if (!block.IsVisited && (block.IsBoss || block.IsTimespinner))
							MarkBlockAsBossOrTimespinner(block);
					}
				}
			}
		}

		void ResetMinimap()
		{
			if (preservedRoomStates == null)
				return;

			foreach (var roomState in preservedRoomStates)
				roomState.ApplyTo(GetRoom(roomState.RoomKey));

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

		MinimapRoom GetRoom(Roomkey roomKey) =>
			Minimap.GetRoomFromLevelAndRoom(roomKey.LevelId, roomKey.RoomId);

		class MinimapRoomState
		{
			public readonly Roomkey RoomKey;

			readonly Dictionary<Point, MiniMapBlockState> blockStates;

			public MinimapRoomState(Roomkey key, MinimapRoom room)
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
