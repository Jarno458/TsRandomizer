using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.MapMenuScreen")]
	partial
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

		ItemLocationMap itemLocations;
		bool isShowingAviableLocations;

		LookupDictionairy<Roomkey, MinimapRoomState> preservedRoomStates;

		MinimapSpecification Minimap => Dynamic._minimap;

		public MinimapScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
			itemLocations = itemLocationMap;

			Dynamic._removeMarkerText = (string)Dynamic._removeMarkerText + " / Show where to go next";

			foreach (var roomkey in DisabledCheckpoints)
				foreach (var block in GetRoom(roomkey).Blocks.Values)
					block.IsCheckpoint = false;

			foreach (var roomkey in BossRooms)
				foreach (var block in GetRoom(roomkey).Blocks.Values)
					block.IsBoss = true;
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
