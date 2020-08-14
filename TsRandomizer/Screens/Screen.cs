using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsRandomizer.Extensions;
using TsRandomizer.IntermediateObjects;
using TsRandomizer.Randomisation;

namespace TsRandomizer.Screens
{
	abstract class Screen<T> : Screen where T : GameScreen
	{
		protected Screen(ScreenManager screenManager, GameScreen gameScreen) : base(screenManager, gameScreen)
		{
		}
	}

	class Screen
	{
		public static readonly Dictionary<Type, Type> RegisteredTypes = new Dictionary<Type, Type>(); //TypeToReplace, ReplacerType

		protected readonly ScreenManager ScreenManager;
		public readonly GameScreen GameScreen;

		protected readonly dynamic Reflected;

		static Screen()
		{
			var screen = MethodBase.GetCurrentMethod().DeclaringType
			               ?? throw new TypeLoadException("Cannot load Type Screen");

			var dirievedTypes = screen.Assembly.GetTypes()
				.Where(t => screen.IsAssignableFrom(t)
				            && !t.IsGenericType
				            && t != screen);

			foreach (var dirivedType in dirievedTypes)
			{
				var supportedGameScreenTypes = dirivedType
					.GetCustomAttributes(typeof(TimeSpinnerType), true)
					.Cast<TimeSpinnerType>()
					.Select(a => a.Type)
					.ToArray();

				if (!supportedGameScreenTypes.Any())
					// ReSharper disable once PossibleNullReferenceException
					RegisteredTypes.Add(dirivedType.BaseType.GetGenericArguments()[0], dirivedType);
				else
					foreach (var supportedGameEventType in supportedGameScreenTypes)
						RegisteredTypes.Add(supportedGameEventType, dirivedType);
			}
		}

		public Screen(ScreenManager screenManager, GameScreen gameScreen)
		{
			ScreenManager = screenManager;
			GameScreen = gameScreen;
			Reflected = gameScreen.AsDynamic();
		}

		public virtual void Initialize(ItemLocationMap itemLocationMap, GCM gameContentManager)
		{
		}

		public virtual void Unload()
		{
		}

		public virtual void Update(GameTime gameTime, InputState input)
		{
		}

		public virtual void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
		}
	}
}
