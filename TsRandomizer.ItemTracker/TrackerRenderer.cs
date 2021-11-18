using System;
using System.Collections.Generic;
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
		static readonly Dictionary<ItemIdentifier, int> AnimationIndexes = new Dictionary<ItemIdentifier, int>(ItemTrackerState.NumberOfItems);

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

			numberOfItems = ItemTrackerState.NumberOfItems - (numberOfFireSourcesCombined-1) - (numberOfPinkSourcesCombined-1);
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
				DrawItem(spriteBatch, state.PyramidKeys, new ItemIdentifier(EInventoryRelicType.PyramidsKey));
				DrawItem(spriteBatch, state.EyeRing, new ItemIdentifier(EInventoryOrbType.Eye, EOrbSlot.Passive));
				DrawItem(spriteBatch, state.CardA, new ItemIdentifier(EInventoryRelicType.ScienceKeycardA));
				DrawItem(spriteBatch, state.CardB, new ItemIdentifier(EInventoryRelicType.ScienceKeycardB));
				DrawItem(spriteBatch, state.CardC, new ItemIdentifier(EInventoryRelicType.ScienceKeycardC));
				DrawItem(spriteBatch, state.CardD, new ItemIdentifier(EInventoryRelicType.ScienceKeycardD));
				DrawItem(spriteBatch, state.CardV, new ItemIdentifier(EInventoryRelicType.ScienceKeycardV));
				DrawItem(spriteBatch, state.Tablet, new ItemIdentifier(EInventoryRelicType.Tablet));
				DrawItem(spriteBatch, state.CardE, new ItemIdentifier(EInventoryRelicType.ElevatorKeycard));
				DrawItem(spriteBatch, state.WaterMask, new ItemIdentifier(EInventoryRelicType.WaterMask));
				DrawItem(spriteBatch, state.GassMask, new ItemIdentifier(EInventoryRelicType.AirMask));
				DrawItem(spriteBatch, state.Kobo, new ItemIdentifier(EInventoryFamiliarType.Kobo));
				DrawItem(spriteBatch, state.MerchantCrow, new ItemIdentifier(EInventoryFamiliarType.MerchantCrow));
				DrawFireSource(spriteBatch, state);
				DrawPinkSource(spriteBatch, state);
			}
		}

		void DrawFireSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if(state.FireOrb)
				DrawItem(spriteBatch, state.FireOrb, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Melee));
			else if (state.FireSpell)
				DrawItem(spriteBatch, state.FireSpell, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Spell));
			else if (state.DinsFire)
				DrawItem(spriteBatch, state.DinsFire, new ItemIdentifier(EInventoryOrbType.Book, EOrbSlot.Spell));
			else if (state.FireRing)
				DrawItem(spriteBatch, state.FireRing, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Passive));
			else
				DrawItem(spriteBatch, false, new ItemIdentifier(EInventoryOrbType.Flame, EOrbSlot.Melee));
		}

		void DrawPinkSource(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			if (state.PinkOrb)
				DrawItem(spriteBatch, state.PinkOrb, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Melee));
			else if (state.PinkSpell)
				DrawItem(spriteBatch, state.PinkSpell, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Spell));
			else if (state.PinkRing)
				DrawItem(spriteBatch, state.PinkRing, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Passive));
			else
				DrawItem(spriteBatch, false, new ItemIdentifier(EInventoryOrbType.Pink, EOrbSlot.Melee));
		}

		void ResetPosition()
		{
			xIndex = 0;
			yIndex = 0;
		}

		void DrawItem(SpriteBatch spriteBatch, bool obtained, ItemIdentifier itemInfo)
		{
			if(xIndex >= columnCount)
			{
				xIndex = 0;
				yIndex++;
			}

			var position = new Point(xIndex++ * IconSize, yIndex * IconSize);

			DrawItem(spriteBatch, position, obtained, GetAnimationIndex(itemInfo));
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

		static int GetAnimationIndex(ItemIdentifier itemInfo)
		{
			if (AnimationIndexes.TryGetValue(itemInfo, out int index))
				return index;

			var animationIndex = new SingleItemInfo(itemInfo).AnimationIndex;

			AnimationIndexes.Add(itemInfo, animationIndex);

			return animationIndex;
		}
	}
}
