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
				var currentIndex = AllowedValues.IndexOf(Value);
				var newIndex = currentIndex + 1 >= AllowedValues.Count ? 0 : currentIndex + 1;
				Value = AllowedValues[newIndex];
			}
			catch
			{
				Value = (string)DefaultValue;
			}

		}

		internal override void UpdateMenuEntry(MenuEntry menuEntry)
		{
			base.UpdateMenuEntry(menuEntry);
			string ShortName = Value.Substring(Value.LastIndexOf("\\") + 1).Replace(".xnb", "");
			if (ShortName.Contains(Character))
			{
				ShortName = GetSpriteName(ShortName);
			}
			menuEntry.Text = $"{Name} - {ShortName}";
		}

		internal string GetSpriteName(string sprite)
		{
			switch (sprite)
			{
				case "LunaisSprite":
					return "Lunais";
				case "LunaisAltSprite":
					return "Eternal Brooch";
				case "LunaisAltSprite2":
					return "Goddess Brooch";
				case "FamiliarMeyef":
					return "Meyef";
				case "FamiliarAltMeyef":
					return "Wyrm Brooch";
				case "FamiliarCrow":
					return "Merchant Crow";
				case "FamiliarAltCrow":
					return "Greed Brooch";
				default:
					return sprite;
			}
		}
	}
}
