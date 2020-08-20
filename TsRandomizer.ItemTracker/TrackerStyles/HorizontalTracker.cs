using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
using Timespinner.GameAbstractions.Inventory;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.ItemTracker;

namespace TsRandomizerItemTracker.TrackerStyles
{
	class HorizontalTracker : ITrakcerStyle
	{
		const int IconSize = 32;

		readonly SpriteSheet menuIcons;
		readonly SpriteFont font;

		int index;
		
		public HorizontalTracker(SpriteSheet menuIcons, SpriteFont font)
		{
			this.menuIcons = menuIcons;
			this.font = font;
		}

		public Point GetSize()
		{
			return new Point(32 * ItemTrackerState.NumberOfItems, 32);
		}

		public void Draw(SpriteBatch spriteBatch, ItemTrackerState state)
		{
			index = 0;

			DrawItem(spriteBatch, state.Timestop, ItemInfo.Get(EInventoryRelicType.TimespinnerWheel).AnimationIndex);
			DrawItem(spriteBatch, state.TimeSpindle, ItemInfo.Get(EInventoryRelicType.TimespinnerSpindle).AnimationIndex);
			DrawItem(spriteBatch, state.TimeGear1, ItemInfo.Get(EInventoryRelicType.TimespinnerGear1).AnimationIndex);
			DrawItem(spriteBatch, state.TimeGear2, ItemInfo.Get(EInventoryRelicType.TimespinnerGear2).AnimationIndex);
			DrawItem(spriteBatch, state.TimeGear3, ItemInfo.Get(EInventoryRelicType.TimespinnerGear3).AnimationIndex);
			DrawItem(spriteBatch, state.Dash, ItemInfo.Get(EInventoryRelicType.Dash).AnimationIndex);
			DrawItem(spriteBatch, state.DoubleJump, ItemInfo.Get(EInventoryRelicType.DoubleJump).AnimationIndex);
			DrawItem(spriteBatch, state.Lightwall, ItemInfo.Get(EInventoryOrbType.Barrier, EOrbSlot.Spell).AnimationIndex);
			DrawItem(spriteBatch, state.CelestialSash, ItemInfo.Get(EInventoryRelicType.EssenceOfSpace).AnimationIndex);
			DrawItem(spriteBatch, state.FireOrb, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Melee).AnimationIndex);
			DrawItem(spriteBatch, state.FireSpell, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Spell).AnimationIndex);
			DrawItem(spriteBatch, state.FireRing, ItemInfo.Get(EInventoryOrbType.Flame, EOrbSlot.Passive).AnimationIndex);
			DrawItem(spriteBatch, state.DinsFire, ItemInfo.Get(EInventoryOrbType.Book, EOrbSlot.Spell).AnimationIndex);
			DrawItem(spriteBatch, state.CardA, ItemInfo.Get(EInventoryRelicType.ScienceKeycardA).AnimationIndex);
			DrawItem(spriteBatch, state.CardB, ItemInfo.Get(EInventoryRelicType.ScienceKeycardB).AnimationIndex);
			DrawItem(spriteBatch, state.CardC, ItemInfo.Get(EInventoryRelicType.ScienceKeycardC).AnimationIndex);
			DrawItem(spriteBatch, state.CardD, ItemInfo.Get(EInventoryRelicType.ScienceKeycardD).AnimationIndex);
			DrawItem(spriteBatch, state.CardV, ItemInfo.Get(EInventoryRelicType.ScienceKeycardV).AnimationIndex);
			DrawItem(spriteBatch, state.CardE, ItemInfo.Get(EInventoryRelicType.ElevatorKeycard).AnimationIndex);
			DrawItem(spriteBatch, state.WaterMask, ItemInfo.Get(EInventoryRelicType.WaterMask).AnimationIndex);
			DrawItem(spriteBatch, state.GassMask, ItemInfo.Get(EInventoryRelicType.AirMask).AnimationIndex);
			DrawItem(spriteBatch, state.PyramidKeys, ItemInfo.Get(EInventoryRelicType.PyramidsKey).AnimationIndex);
			DrawItem(spriteBatch, state.PinkOrb, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Melee).AnimationIndex);
			DrawItem(spriteBatch, state.PinkSpell, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Spell).AnimationIndex);
			DrawItem(spriteBatch, state.PinkRing, ItemInfo.Get(EInventoryOrbType.Pink, EOrbSlot.Passive).AnimationIndex);
		}

		void DrawItem(SpriteBatch spriteBatch, bool obtained, int animationIndex)
		{
			DrawItem(spriteBatch, index++, obtained, animationIndex);
		}

		void DrawItem(SpriteBatch spriteBatch, int i, bool obtained, int animationIndex)
		{
			var drawPoint = new Point(0,0);
			var position = new Rectangle(drawPoint.X + (i * IconSize), drawPoint.Y, IconSize, IconSize);
			var spritePosition = menuIcons.FrameStarts[animationIndex];
			var sprite = new Rectangle(spritePosition.X, spritePosition.Y, menuIcons.FrameSize.X, menuIcons.FrameSize.Y);
			var color = obtained ? Color.White : Color.Black;

			spriteBatch.Draw(menuIcons.Texture, position, sprite, color, 0, Vector2.Zero, SpriteEffects.None, 1);
		}
	}
}
