using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;

namespace TsRandomizerItemTracker
{
	class TrackerRenderer
	{
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

			numberOfItems = ItemTrackerState.NumberOfItems - --numberOfFireSourcesCombined - --numberOfPinkSourcesCombined;
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
				DrawItem(spriteBatch, state.Timestop, ItemInfo.Get(EInventoryRelicType.TimespinnerWheel));
				DrawItem(spriteBatch, state.TimeSpindle, ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle));
				DrawItem(spriteBatch, state.TimeGear1, ItemInfo.Get(EInventoryRelicType.TimespinnerGear1));
				DrawItem(spriteBatch, state.TimeGear2, ItemInfo.Get(EInventoryRelicType.TimespinnerGear2));
				DrawItem(spriteBatch, state.TimeGear3, ItemInfo.Get(EInventoryRelicType.TimespinnerGear3));
				DrawItem(spriteBatch, state.Dash, ItemInfo.Get(EInventoryRelicType.Dash));
				DrawItem(spriteBatch, state.DoubleJump, ItemInfo.Get(EInventoryRelicType.DoubleJump));
				DrawItem(spriteBatch, state.Lightwall, ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell));
				DrawItem(spriteBatch, state.CelestialSash, ItemInfo.Get(EInventoryRelicType.EssenceOfSpace));
				DrawItem(spriteBatch, state.PyramidKeys, ItemInfo.Get(EInventoryRelicType.PyramidsKey));
				DrawItem(spriteBatch, state.CardA, ItemInfo.Get(EInventoryRelicType.ScienceKeycardA));
				DrawItem(spriteBatch, state.CardB, ItemInfo.Get(EInventoryRelicType.ScienceKeycardB));
				DrawItem(spriteBatch, state.CardC, ItemInfo.Get(EInventoryRelicType.ScienceKeycardC));
				DrawItem(spriteBatch, state.CardD, ItemInfo.Get(EInventoryRelicType.ScienceKeycardD));
				DrawItem(spriteBatch, state.CardV, ItemInfo.Get(EInventoryRelicType.ScienceKeycardV));
				DrawItem(spriteBatch, state.CardE, ItemInfo.Get(EInventoryRelicType.ElevatorKeycard));
				DrawItem(spriteBatch, state.WaterMask, ItemInfo.Get(EInventoryRelicType.WaterMask));
				DrawItem(spriteBatch, state.GassMask, ItemInfo.Get(EInventoryRelicType.AirMask));

				DrawFireSource(spriteBatch, state);
				DrawPinkSource(spriteBatch, state);
			}
		}

		void DrawFireSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if(state.FireOrb)
				DrawItem(spriteBatch, state.FireOrb, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee));
			else if (state.FireSpell)
				DrawItem(spriteBatch, state.FireSpell, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell));
			else if (state.DinsFire)
				DrawItem(spriteBatch, state.DinsFire, ItemInfo.Get(EInventoryOrbType.Book, EOrbSlot.Spell));
			else if (state.FireRing)
				DrawItem(spriteBatch, state.FireRing, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive));
			else
				DrawItem(spriteBatch, false, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee));
		}

		void DrawPinkSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if (state.PinkOrb)
				DrawItem(spriteBatch, state.PinkOrb, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee));
			else if (state.PinkSpell)
				DrawItem(spriteBatch, state.PinkSpell, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Spell));
			else if (state.PinkRing)
				DrawItem(spriteBatch, state.PinkRing, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Passive));
			else
				DrawItem(spriteBatch, false, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee));
		}

		void ResetPosition()
		{
			xIndex = 0;
			yIndex = 0;
		}

		void DrawItem(SpriteBatch spriteBatch, bool obtained, ItemInfo itemInfo)
		{
			if(xIndex >= columnCount)
			{
				xIndex = 0;
				yIndex++;
			}

			var position = new Point(xIndex++ * IconSize, yIndex * IconSize);

			DrawItem(spriteBatch, position, obtained, itemInfo.AnimationIndex);
		}

		void DrawItem(SpriteBatch spriteBatch, Point point, bool obtained, int animationIndex)
		{
			var position = new Rectangle(point.X, point.Y, IconSize, IconSize);
			var spritePosition = menuIcons.FrameStarts[animationIndex];
			var sprite = new Rectangle(spritePosition.X, spritePosition.Y, menuIcons.FrameSize.X, menuIcons.FrameSize.Y);
			
			var color = obtained ? Color.White : Color.Black;
			color.A = obtained ? (byte)255 : (byte)50;

			spriteBatch.Draw(menuIcons.Texture, position, sprite, color);
		}
	}
}
