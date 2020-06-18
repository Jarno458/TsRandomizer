using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameStateManagement.ScreenManager;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;
using TsRanodmizer.Randomisation;

namespace TsRanodmizer.Screens
{
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.PauseMenu.MapMenuScreen")]
	// ReSharper disable once UnusedMember.Global
	class MinimapScreen : Screen
	{
		ItemLocationMap itemLocations;
		bool isShowingAviableLocations;

		readonly ControllerMapping controllerMapping;

		LookupDictionairy<RoomItemKey, MinimapRoomState> preservedRoomStates;

		public MinimapScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
			controllerMapping = screenManager.MenuControllerMapping;
		}

		public override void Initialize(ItemLocationMap itemLocationMap)
		{
			itemLocations = itemLocationMap;

			Reflected._removeMarkerText = (string)Reflected._removeMarkerText + " / Show where to go next";
		}

		public override void Update(GameTime gameTime, InputState input)
		{
			var mapping = controllerMapping.Mappings[(int)ButtonMapping.EDestinationType.Secondary];
			var shouldShowItemLocationHints = mapping.IsButtonDown(input.CurrentGamePadStates[0], input.CurrentKeyboardStates[0]);

			if ((isShowingAviableLocations && shouldShowItemLocationHints) 
			    || (!shouldShowItemLocationHints && !isShowingAviableLocations))
				return;

			if (!isShowingAviableLocations && shouldShowItemLocationHints)
			{
				MarkAvailableItemLocations();
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

			var visableAreas = (List<EMinimapEraType>)Reflected._availableEras;
			var areas = ((MinimapSpecification)Reflected._minimap).Areas;
			
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

					if (block.IsTransition)
					{
						block.IsTransition = false;
						block.RoomColor = EMinimapRoomColor.Yellow;
					}
					else
					{
						block.RoomColor = EMinimapRoomColor.Orange;
					}
				}
			}
		}

		void ResetMinimap()
		{
			if(preservedRoomStates == null)
				return;

			var areas = ((MinimapSpecification)Reflected._minimap).Areas;

			foreach (var roomState in preservedRoomStates)
				roomState.ApplyTo(areas.GetRoom(roomState.RoomKey));

			preservedRoomStates = null;
		}

		IEnumerable<ItemLocation> GetAvailableItemLocations()
		{
			var obtainedRequirements = GetAvailableRequirementsBasedOnObtainedItems();
			var locations = itemLocations.Where(l => !l.IsPickedUp && l.Gate.CanBeOpenedWith(obtainedRequirements));
			return locations;
		}

		Requirement GetAvailableRequirementsBasedOnObtainedItems()
		{
			var requirements = itemLocations
				.Where(l => l.IsPickedUp)
				.Aggregate(Requirement.None, (a, b) => a | b.Unlocks);
			return requirements;
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
