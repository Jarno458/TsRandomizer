using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;

namespace TsRandomizer.Screens
{
	class MenuEntry
	{
		static readonly Type MainMenuEntryType = TimeSpinnerType
			.Get("Timespinner.GameStateManagement.MenuEntry");
		static readonly EventInfo MainMenuEntrySelectEventInfo = MainMenuEntryType
			.GetEvent("Selected", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly Type MainMenuSelectedEventType = MainMenuEntrySelectEventInfo.EventHandlerType;
		static readonly MethodInfo SelectedEventAddMethod = MainMenuEntrySelectEventInfo.GetAddMethod(true);

		// ReSharper disable PossibleNullReferenceException
		public static readonly Color UnselectedColor = (Color)MainMenuEntryType
			.GetField("UnselectedColor", BindingFlags.Static | BindingFlags.Public)
			.GetValue(null);
		public static readonly Color UnavailableColor = (Color)MainMenuEntryType
			.GetField("UnavailableColor", BindingFlags.Static | BindingFlags.Public)
			.GetValue(null);
		// ReSharper restore PossibleNullReferenceException

		readonly object entry;
		readonly dynamic reflected;

		public string Text
		{
			get => reflected.Text;
			set => reflected.Text = value;
		}

		public bool DoesDrawLargeShadow
		{
			get => reflected.DoesDrawLargeShadow;
			set => reflected.DoesDrawLargeShadow = value;
		}

		public bool IsCenterAligned
		{
			get => reflected.IsCenterAligned;
			set => reflected.IsCenterAligned = value;
		}

		public string Description
		{
			get => reflected.Description;
			set => reflected.Description = value;
		}

		MenuEntry(object entry)
		{
			this.entry = entry;
			reflected = entry.AsDynamic();

			DoesDrawLargeShadow = true;
			IsCenterAligned = true;
		}

		public static MenuEntry Create(string text, Action<PlayerIndex> handler)
		{
			return Create(text, (o, args) => handler(args.AsDynamic().PlayerIndex));
		}
		
		public static MenuEntry Create(string text, Action<object, EventArgs> handler)
		{
			var handlerDelegate = 
				Delegate.CreateDelegate(MainMenuSelectedEventType, handler.Target, handler.Method);

			var entry = Activator.CreateInstance(MainMenuEntryType, text);
			SelectedEventAddMethod.Invoke(entry, new object[] { handlerDelegate });

			return new MenuEntry(entry);
		}

		public object AsTimeSpinnerMenuEntry()
		{
			return entry;
		}
	}
}