using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	class MessageBox
	{
		static readonly Type MessageBoxScreenType = TimeSpinnerType.Get("Timespinner.GameStateManagement.MessageBoxScreen");
		static readonly EventInfo MessageBoxScreenAcceptedEventInfo = MessageBoxScreenType
			.GetEvent("Accepted", BindingFlags.Public | BindingFlags.Instance);
		static readonly Type MessageBoxScreenAcceptedEventType = MessageBoxScreenAcceptedEventInfo.EventHandlerType;
		static readonly MethodInfo MessageBoxScreenAcceptedEventAddMethod = MessageBoxScreenAcceptedEventInfo.GetAddMethod(false);

		public GameScreen Screen { get; }

		MessageBox(GameScreen screen)
		{
			Screen = screen;
		}

		public static MessageBox Create(ScreenManager screenManager, string text, Action<PlayerIndex> handler)
		{
			return Create(screenManager, text, (o, args) => handler(args.AsDynamic().PlayerIndex));
		}

		public static MessageBox Create(ScreenManager screenManager, string text, Action<object, EventArgs> handler)
		{
			var messageBoxScreen = MessageBoxScreenType.CreateInstance(false, text, screenManager.MenuControllerMapping);

			var handlerDelegate =
				Delegate.CreateDelegate(MessageBoxScreenAcceptedEventType, handler.Target, handler.Method);

			MessageBoxScreenAcceptedEventAddMethod.Invoke(messageBoxScreen, new object[] { handlerDelegate });

			return new MessageBox((GameScreen)messageBoxScreen);
		}
	}
}
