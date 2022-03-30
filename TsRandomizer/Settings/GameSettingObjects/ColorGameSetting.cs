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
	public class ColorGameSetting : StringGameSetting
	{
		static readonly string[] XnaColors = {
			"AliceBlue",
			"AntiqueWhite",
			"Aqua",
			"Aquamarine",
			"Azure",
			"Beige",
			"Bisque",
			"Black",
			"BlanchedAlmond",
			"Blue",
			"BlueViolet",
			"Brown",
			"BurlyWood",
			"CadetBlue",
			"Chartreuse",
			"Chocolate",
			"Coral",
			"CornflowerBlue",
			"Cornsilk",
			"Crimson",
			"Cyan",
			"DarkBlue",
			"DarkCyan",
			"DarkGoldenrod",
			"DarkGray",
			"DarkGreen",
			"DarkKhaki",
			"DarkMagenta",
			"DarkOliveGreen",
			"DarkOrange",
			"DarkOrchid",
			"DarkRed",
			"DarkSalmon",
			"DarkSeaGreen",
			"DarkSlateBlue",
			"DarkSlateGray",
			"DarkTurquoise",
			"DarkViolet",
			"DeepPink",
			"DeepSkyBlue",
			"DimGray",
			"DodgerBlue",
			"Firebrick",
			"FloralWhite",
			"ForestGreen",
			"Fuchsia",
			"Gainsboro",
			"GhostWhite",
			"Gold",
			"Goldenrod",
			"Gray",
			"Green",
			"GreenYellow",
			"Honeydew",
			"HotPink",
			"IndianRed",
			"Indigo",
			"Ivory",
			"Khaki",
			"Lavender",
			"LavenderBlush",
			"LawnGreen",
			"LemonChiffon",
			"LightBlue",
			"LightCoral",
			"LightCyan",
			"LightGoldenrodYellow",
			"LightGray",
			"LightGreen",
			"LightPink",
			"LightSalmon",
			"LightSeaGreen",
			"LightSkyBlue",
			"LightSlateGray",
			"LightSteelBlue",
			"LightYellow",
			"Lime",
			"LimeGreen",
			"Linen",
			"Magenta",
			"Maroon",
			"MediumAquamarine",
			"MediumBlue",
			"MediumOrchid",
			"MediumPurple",
			"MediumSeaGreen",
			"MediumSlateBlue",
			"MediumSpringGreen",
			"MediumTurquoise",
			"MediumVioletRed",
			"MidnightBlue",
			"MintCream",
			"MistyRose",
			"Moccasin",
			"NavajoWhite",
			"Navy",
			"OldLace",
			"Olive",
			"OliveDrab",
			"Orange",
			"OrangeRed",
			"Orchid",
			"PaleGoldenrod",
			"PaleGreen",
			"PaleTurquoise",
			"PaleVioletRed",
			"PapayaWhip",
			"PeachPuff",
			"Peru",
			"Pink",
			"Plum",
			"PowderBlue",
			"Purple",
			"Red",
			"RosyBrown",
			"RoyalBlue",
			"SaddleBrown",
			"Salmon",
			"SandyBrown",
			"SeaGreen",
			"SeaShell",
			"Sienna",
			"Silver",
			"SkyBlue",
			"SlateBlue",
			"SlateGray",
			"Snow",
			"SpringGreen",
			"SteelBlue",
			"Tan",
			"Teal",
			"Thistle",
			"Tomato",
			"Turquoise",
			"Violet",
			"Wheat",
			"White",
			"WhiteSmoke",
			"Yellow",
			"YellowGreen"
		};

		[JsonIgnoreDeserialize]
		public HashSet<string> AllowedValues { get; }

		[JsonIgnore]
		public Color Color {
			get { return GetColor(Value); }
			set { Value = $"#{value.R:X2}{value.G:X2}{value.B:X2}"; }
		}

		public ColorGameSetting(string name, string description, 
			string defaultValue, bool canBeChangedInGame = false) 
				: base(name, description, defaultValue, 20, canBeChangedInGame)
		{
			AllowedValues = XnaColors.Concat("#html-hex-color").ToHashSet();
		}

		[JsonConstructor]
		public ColorGameSetting()
		{
		}

		static Color GetColor(string text)
		{
			if (XnaColors.Contains(text))
				return (Color)typeof(Color).GetProperty(text).GetValue(null, null);

			if (text.StartsWith("#"))
				text = text.Substring(1);

			if (text.Length == 6
				&& int.TryParse(text.Substring(0, 2), NumberStyles.HexNumber, null, out var red)
			    && int.TryParse(text.Substring(2, 2), NumberStyles.HexNumber, null, out var green)
			    && int.TryParse(text.Substring(4, 2), NumberStyles.HexNumber, null, out var blue))

				return new Color(red, green, blue);

			return Color.White;
		}

		public override void ToggleValue()
		{
			var currentIndex = Array.IndexOf(XnaColors, Value);
			var newIndex = currentIndex + 1 >= XnaColors.Length ? 0 : currentIndex + 1;

			Value = XnaColors[newIndex];
		}

		internal override void UpdateMenuEntry(MenuEntry menuEntry)
		{
			base.UpdateMenuEntry(menuEntry);

			if(menuEntry.BaseDrawColor != MenuEntry.UnAvailableColor)
				menuEntry.BaseDrawColor = GetColor(Value);
		}
	}
}
