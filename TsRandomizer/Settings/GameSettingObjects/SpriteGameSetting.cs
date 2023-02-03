using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TsRandomizer.Extensions;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class SpriteGameSetting : StringGameSetting
	{
		[JsonIgnoreDeserialize]
		public List<string> AllowedValues { get; }

		[JsonIgnore]
		public new string Value {
			get { return CurrentValue as string ?? DefaultValue as string; }
			set { CurrentValue = value; }
		}

		string realValue;
		string Character;

		public SpriteGameSetting(string name, string description, string character,
			string defaultValue, bool canBeChangedInGame = false) 
				: base(name, description, defaultValue, 20, canBeChangedInGame)
		{
			Character = character;
			AllowedValues = Directory.GetFiles($"Custom Sprites\\{Character}\\", "*.xnb").ToList();
			AllowedValues.AddRange(Directory.GetFiles($"Content\\Sprites\\Heroes\\", $"*{Character}*.xnb").ToList());
		}

		[JsonConstructor]
		public SpriteGameSetting()
		{
		}


		public override void ToggleValue()
		{
			try
			{
				var currentIndex = AllowedValues.IndexOf(realValue);
				var newIndex = currentIndex + 1 >= AllowedValues.Count ? 0 : currentIndex + 1;
				realValue = AllowedValues[newIndex];

				var newString = realValue.Substring(realValue.LastIndexOf("\\") + 1).Replace(".xnb", "");
				Value = newString;
			}
			catch
			{
				Value = (string)DefaultValue;
				realValue = (string)DefaultValue;
			}

		}
	}
}
