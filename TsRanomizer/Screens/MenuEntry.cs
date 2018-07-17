using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer.Screens
{
	class MenuEntry
	{
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly EventInfo MainMenuEntrySelectEventInfo = MainMenuEntryType
			.GetEvent("Selected", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly Type MainMenuSelectedEventType = MainMenuEntrySelectEventInfo.EventHandlerType;
		static readonly MethodInfo SelectedEventAddMethod = MainMenuEntrySelectEventInfo.GetAddMethod(true);
		
		public object Entry { get; }

		public bool DoesDrawLargeShadow
		{
			get => Entry.Reflect().DoesDrawLargeShadow;
			set => Entry.Reflect().DoesDrawLargeShadow = value;
		}

		public bool IsCenterAligned
		{
			get => Entry.Reflect().IsCenterAligned;
			set => Entry.Reflect().IsCenterAligned = value;
		}

		MenuEntry(object entry)
		{
			Entry = entry;
			DoesDrawLargeShadow = true;
			IsCenterAligned = true;
		}

		public static MenuEntry Create(string text, Action<PlayerIndex> handler)
		{
			return Create(text, (o, args) => handler(args.Reflect().PlayerIndex));
		}
		
		static MenuEntry Create(string text, Action<object, EventArgs> handler)
		{
			var handlerDelegate = 
				Delegate.CreateDelegate(MainMenuSelectedEventType, handler.Target, handler.Method);

			var entry = Activator.CreateInstance(MainMenuEntryType, text);
			SelectedEventAddMethod.Invoke(entry, new object[] { handlerDelegate });

			return new MenuEntry(entry);
		}
	}
}