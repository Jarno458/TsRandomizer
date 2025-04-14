using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.IntermediateObjects.CustomItems;
using TsRandomizer.ItemTracker;
using TsRandomizer.Randomisation;

namespace TsRandomizerItemTracker
{
	class TrackerRenderer
	{
		static readonly Dictionary<ItemIdentifier, int> AnimationIndexes = new Dictionary<ItemIdentifier, int>(ItemTrackerState.NumberOfItems);
		
        static TrackerRenderer()
        {
            CustomItem.Initialize();

            var unlockingMap = new DefaultItemUnlockingMap(Seed.Zero);
            foreach (var item in CustomItem.GetAllCustomItems(unlockingMap))
                AnimationIndexes.Add(item.Identifier, item.AnimationIndex);
        }

		public int IconSize { get; set; } = 32;

		readonly SpriteSheet menuIcons;

		readonly int numberOfItems;

		int xIndex;
		int yIndex;

		int columnCount = 5;
		
		public TrackerRenderer(GCM gcm, ContentManager contentManager)
		{
			menuIcons = (SpriteSheet)gcm.AsDynamic().Get("Sprites/Items/MenuIcons", contentManager);

			var numberOfFireSourcesCombined = 4;
			var numberOfPinkSourcesCombined = 3;
            var numberOfPyramidKeysSourcesCombined = 4;
			var numberOfHangarLasersCombined = 3;
			var numberOfLabLasersCombined = 4;

			numberOfItems = ItemTrackerState.NumberOfItems 
                            - (numberOfFireSourcesCombined-1) 
                            - (numberOfPinkSourcesCombined-1)
                            - (numberOfPyramidKeysSourcesCombined-1)
							- (numberOfHangarLasersCombined-1)
							- (numberOfLabLasersCombined-1);
		}

		public void SetWidth(int clientBoundsWidth)
		{
			columnCount = clientBoundsWidth / IconSize;

			if(columnCount > numberOfItems)
				columnCount = numberOfItems;
			else if (columnCount == 0)
				columnCount = 1;
		}

		public Point GetSize()
		{
			return new Point(IconSize * columnCount, IconSize * (int)Math.Ceiling((decimal)numberOfItems / columnCount));
		}

		public void Draw(SpriteBatch spriteBatch, ItemTrackerState state)
        {

            ResetPosition();

			using (spriteBatch.BeginUsing(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp))
			{
				DrawItem(spriteBatch, state.Timestop, new ItemIdentifier(EInventoryRelicType.TimespinnerWheel));
				DrawItem(spriteBatch, state.TimeSpindle, new ItemIdentifier(EInventoryRelicType.TimespinnerSpindle));
				DrawItem(spriteBatch, state.TimeGear1, new ItemIdentifier(EInventoryRelicType.TimespinnerGear1));
				DrawItem(spriteBatch, state.TimeGear2, new ItemIdentifier(EInventoryRelicType.TimespinnerGear2));
				DrawItem(spriteBatch, state.TimeGear3, new ItemIdentifier(EInventoryRelicType.TimespinnerGear3));
				DrawItem(spriteBatch, state.Dash, new ItemIdentifier(EInventoryRelicType.Dash));
				DrawItem(spriteBatch, state.DoubleJump, new ItemIdentifier(EInventoryRelicType.DoubleJump));
				DrawItem(spriteBatch, state.Lightwall, new ItemIdentifier(EInventoryOrbType.Barrier, EOrbSlot.Spell));
				DrawItem(spriteBatch, state.CelestialSash, new ItemIdentifier(EInventoryRelicType.EssenceOfSpace));
                DrawPyramidKeys(spriteBatch, state);
				DrawItem(spriteBatch, state.WaterMask, new ItemIdentifier(EInventoryRelicType.WaterMask));
				DrawItem(spriteBatch, state.GasMask, new ItemIdentifier(EInventoryRelicType.AirMask));
                DrawItem(spriteBatch, state.EyeRing, new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Passive));
                DrawItem(spriteBatch, state.CardA, new ItemIdentifier(EInventoryRelicType.ScienceKeycardA));
				DrawItem(spriteBatch, state.CardB, new ItemIdentifier(EInventoryRelicType.ScienceKeycardB));
				DrawItem(spriteBatch, state.CardC, new ItemIdentifier(EInventoryRelicType.ScienceKeycardC));
				DrawItem(spriteBatch, state.CardD, new ItemIdentifier(EInventoryRelicType.ScienceKeycardD));
				DrawItem(spriteBatch, state.CardV, new ItemIdentifier(EInventoryRelicType.ScienceKeycardV));
				DrawItem(spriteBatch, state.Tablet, new ItemIdentifier(EInventoryRelicType.Tablet));
				DrawItem(spriteBatch, state.CardE, new ItemIdentifier(EInventoryRelicType.ElevatorKeycard));
				DrawItem(spriteBatch, state.Kobo, new ItemIdentifier(EInventoryFamiliarType.Kobo));
				DrawItem(spriteBatch, state.MerchantCrow, new ItemIdentifier(EInventoryFamiliarType.MerchantCrow));
				DrawFireSource(spriteBatch, state);
				DrawPinkSource(spriteBatch, state);
				DrawLaserAccess(spriteBatch, state);
				DrawLabAccess(spriteBatch, state);
                DrawItem(spriteBatch, state.DrawbridgeKey, CustomItem.GetIdentifier(CustomItemType.DrawbridgeKey));
				DrawItem(spriteBatch, state.LanternCube, CustomItem.GetIdentifier(CustomItemType.CubeOfBodie));
			}
		}

        void DrawPyramidKeys(SpriteBatch spriteBatch, ItemTrackerState state)
        {
            DrawItem(spriteBatch, state.PyramidKeys, new ItemIdentifier(EInventoryRelicType.PyramidsKey));

			if (!state.PastWarp && !state.PresentWarp && !state.PyramidWarp)
				return;

            DrawSubItem(spriteBatch, state.PastWarp, new ItemIdentifier(EInventoryRelicType.PyramidsKey), 2, new Point(0,1), Color.Cyan);
            DrawSubItem(spriteBatch, state.PresentWarp, new ItemIdentifier(EInventoryRelicType.PyramidsKey), 2, new Point(1, 1), Color.Fuchsia);
            DrawSubItem(spriteBatch, state.PyramidWarp, new ItemIdentifier(EInventoryRelicType.PyramidsKey), 2, new Point(1, 0), Color.LimeGreen);
        }

		void DrawLaserAccess(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			DrawItem(spriteBatch, state.LaserA && state.LaserI && state.LaserM, new ItemIdentifier(EInventoryUseItemType.None), Color.MistyRose);
			DrawSubItem(spriteBatch, state.LaserA, new ItemIdentifier(EInventoryUseItemType.PlaceHolderItem1), 2, new Point(0, 0), Color.HotPink);
			DrawSubItem(spriteBatch, state.LaserI, new ItemIdentifier(EInventoryUseItemType.PlaceHolderItem1), 2, new Point(1, 0), Color.LightSalmon);
			DrawSubItem(spriteBatch, state.LaserM, new ItemIdentifier(EInventoryUseItemType.PlaceHolderItem1), 2, new Point(0, 1), Color.Crimson);
		}

		void DrawLabAccess(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			//needed to correctly update xIndex
			DrawItem(spriteBatch, false, new ItemIdentifier(EInventoryUseItemType.None), Color.Black);
			DrawSubItem(spriteBatch, state.LabGenza, new ItemIdentifier(EInventoryEquipmentType.LabGlasses), 2, new Point(0, 0), Color.White);
			DrawSubItem(spriteBatch, state.LabDynamo, new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Melee), 2, new Point(1, 0), Color.White);
			DrawSubItem(spriteBatch, state.LabResearch, new ItemIdentifier(EInventoryEquipmentType.LabCoat), 2, new Point(0, 1), Color.White);
			DrawSubItem(spriteBatch, state.LabExperiment, new ItemIdentifier(EInventoryFamiliarType.Demon), 2, new Point(1, 1), Color.White);
		}


		void DrawFireSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if(state.DinsFire)
				DrawItem(spriteBatch, state.DinsFire, new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell));
			else if (state.FireRing)
				DrawItem(spriteBatch, state.FireRing, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Passive));
			else if (state.FireOrb)
				DrawItem(spriteBatch, state.FireOrb, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Melee));
			else if (state.FireSpell)
				DrawItem(spriteBatch, state.FireSpell, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Spell));
			else
				DrawItem(spriteBatch, false, new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell));
		}

		void DrawPinkSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if (state.PinkRing)
				DrawItem(spriteBatch, state.PinkRing, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Passive));
			else if (state.PinkSpell)
				DrawItem(spriteBatch, state.PinkSpell, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Spell));
			else if (state.PinkOrb)
				DrawItem(spriteBatch, state.PinkOrb, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Melee));
			else
				DrawItem(spriteBatch, false, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Passive));
		}

		void ResetPosition()
		{
			xIndex = 0;
			yIndex = 0;
		}

		void DrawItem(SpriteBatch spriteBatch, bool obtained, ItemIdentifier itemInfo)
		{
			DrawItem(spriteBatch, obtained, itemInfo, Color.White);
		}

		void DrawItem(SpriteBatch spriteBatch, Rectangle destination, bool obtained, int animationIndex, Color obtainedColor)
		{
			var spritePosition = menuIcons.FrameStarts[animationIndex];
			var sprite = new Rectangle(spritePosition.X, spritePosition.Y, menuIcons.FrameSize.X, menuIcons.FrameSize.Y);
			
			var color = obtained ? obtainedColor : Color.Black;
			color.A = obtained ? (byte)255 : (byte)50;

			spriteBatch.Draw(menuIcons.Texture, destination, sprite, color);
		}

        void DrawItem(SpriteBatch spriteBatch, bool obtained, ItemIdentifier itemInfo, Color color)
        {
			if (xIndex >= columnCount)
			{
				xIndex = 0;
				yIndex++;
			}

			var position = new Point(xIndex++ * IconSize, yIndex * IconSize);

			DrawItem(spriteBatch, new Rectangle(position.X, position.Y, IconSize, IconSize), 
                obtained, GetAnimationIndex(itemInfo), color);
		}

        void DrawSubItem(SpriteBatch spriteBatch, bool obtained, ItemIdentifier itemInfo, 
            int subGridSize, Point subItemCoordinate, Color color)
        {
            var gridSectionSize = IconSize / subGridSize;
			var position = new Point(
                (xIndex - 1) * IconSize + (gridSectionSize * subItemCoordinate.X),
                yIndex       * IconSize + (gridSectionSize * subItemCoordinate.Y));

            DrawItem(spriteBatch, new Rectangle(position.X, position.Y, gridSectionSize, gridSectionSize), 
                obtained, GetAnimationIndex(itemInfo), color);

            //xIndex++;
        }

		static int GetAnimationIndex(ItemIdentifier itemInfo)
		{
			if (AnimationIndexes.TryGetValue(itemInfo, out int index))
				return index;

			var animationIndex = itemInfo.GetAnimationIndex();
			// Handle None item
			if (animationIndex < 0)
				animationIndex = 230;

			AnimationIndexes.Add(itemInfo, animationIndex);

			return animationIndex;
		}
	}
}
