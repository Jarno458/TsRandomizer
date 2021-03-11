using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TsRandomizerItemTracker
{
	class MouseInputHandler
	{
		const double DoubleClickMaxDelay = 250d;

		readonly Action doubleClickHandler;
		readonly Action rightClickHandler;
		readonly Action<int> scrollHandler;

		double previousLeftClickTimer;

		MouseState previousState;

		public MouseInputHandler(Action doubleClickHandler, Action rightClickHandler, Action<int> scrollHandler)
		{
			this.doubleClickHandler = doubleClickHandler;
			this.rightClickHandler = rightClickHandler;
			this.scrollHandler = scrollHandler;
		}

		public void Update(GameTime gameTime)
		{
			var currentState = Mouse.GetState();

			previousLeftClickTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

			if (WasMouseLeftClick(currentState))
			{
				if (WasDoubleClick(currentState))
				{
					doubleClickHandler();
					previousLeftClickTimer = DoubleClickMaxDelay;
				}
				else
				{
					previousLeftClickTimer = 0;
				}
			}

			if (WasMouseRightClick(currentState))
				rightClickHandler();

			var scrolledAmount = previousState.ScrollWheelValue - currentState.ScrollWheelValue;
			if (scrolledAmount != 0)
				scrollHandler(scrolledAmount);

			previousState = currentState;
		}

		bool WasMouseLeftClick(MouseState currentState)
		{
			return previousState.LeftButton == ButtonState.Pressed && currentState.LeftButton == ButtonState.Released;
		}

		bool WasMouseRightClick(MouseState currentState)
		{
			return previousState.RightButton == ButtonState.Pressed && currentState.RightButton == ButtonState.Released;
		}

		bool WasDoubleClick(MouseState currentState)
		{
			return WasMouseLeftClick(currentState) && previousLeftClickTimer < DoubleClickMaxDelay;
		}
	}
}
