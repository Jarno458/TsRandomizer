using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using TsRandomizer.Screens.Menu;

namespace TsRandomizer.Settings.GameSettingObjects
{
	public class SpriteGameSetting : GameSetting<string>
	{
		[JsonIgnoreDeserialize]
		public List<string> AllowedValues { get; }

		[JsonIgnore]
		public new string Value {
			get { return CurrentValue as string ?? DefaultValue as string; }
			set { CurrentValue = value; }
		}

		readonly string character;

		public SpriteGameSetting(string name, string description, string character,
			string defaultValue, bool canBeChangedInGame = false)
				: base(name, description, defaultValue, canBeChangedInGame)
		{
			this.character = character;

			try
			{
				AllowedValues = new List<string>(0);

				AllowedValues.AddRange(GetFiles(Path.Combine("Custom Sprites", character), "*.xnb"));
				AllowedValues.AddRange(GetFiles(Path.Combine("Custom Sprites", character), "*.png"));
				AllowedValues.AddRange(GetFiles("Content\\Sprites\\Heroes\\", $"*{character}*.xnb"));
			}
			catch
			{
				AllowedValues = new List<string>(0);
			}
		}

		static string[] GetFiles(string directory, string pattern)
		{
			if (!Directory.Exists(directory))
				return new string[0];

			return Directory.GetFiles(directory, pattern);
		}

		[JsonConstructor]
		public SpriteGameSetting()
		{
		}
		
		public override void ToggleValue()
		{
			try
			{
				if (AllowedValues.Count == 0)
					Value = Default;

				var currentIndex = AllowedValues.IndexOf(Value);
				var newIndex = currentIndex + 1 >= AllowedValues.Count ? 0 : currentIndex + 1;

				Value = AllowedValues[newIndex];
			}
			catch
			{
				Value = Default;
			}
		}

		internal override void UpdateMenuEntry(MenuEntry menuEntry)
		{
			base.UpdateMenuEntry(menuEntry);

			var shortName = Path.GetFileNameWithoutExtension(Value);

			if (shortName.Contains(character))
				shortName = GetSpriteName(shortName);

			menuEntry.Text = $"{Name} - {shortName}";
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
				case "FamiliarGriffin":
					return "Griffin";
				case "FamiliarKobo":
					return "Kobo";
				case "FamiliarDemon":
					return "Demon";
				case "FamiliarSprite":
					return "Sprite";
				default:
					return sprite;
			}
		}
	}
}
