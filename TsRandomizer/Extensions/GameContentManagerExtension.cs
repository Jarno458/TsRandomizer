using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using TsRandomizer.Settings;

namespace TsRandomizer.Extensions
{
	static class GameContentManagerExtension
	{
		static readonly Dictionary<EMinimapRoomColor, Rectangle> ColorPositions =
			new Dictionary<EMinimapRoomColor, Rectangle> {
				{ EMinimapRoomColor.Blue, new Rectangle(0, 0, 5, 5) },
				{ EMinimapRoomColor.Purple, new Rectangle(5, 0, 5, 5) },
				{ EMinimapRoomColor.Green, new Rectangle(10, 0, 5, 5) },
				{ EMinimapRoomColor.Orange, new Rectangle(15, 0, 5, 5) },
				{ EMinimapRoomColor.Red, new Rectangle(26, 1, 3, 3) },
				{ EMinimapRoomColor.Yellow, new Rectangle(31, 1, 3, 3) },
				{ EMinimapRoomColor.SpecialBlue, new Rectangle(36, 1, 3, 3) },
				{ EMinimapRoomColor.SpecialPurple, new Rectangle(41, 1, 3, 3) },
			};

		public static void UpdateMinimapColors(this GCM gameContentManager, SettingCollection settings)
		{
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Blue], settings.PastMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Purple], settings.PresentMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Green], settings.PyramidMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Orange], settings.LootMinimapColor.Color);

			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Red], settings.SaveStatueMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.Yellow], settings.SpecailLootMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.SpecialBlue], settings.PresentTransitionMinimapColor.Color);
			SetColor(gameContentManager, ColorPositions[EMinimapRoomColor.SpecialPurple], settings.PastTransitionMinimapColor.Color);
		}

		static void SetColor(GCM gameContentManager, Rectangle target, Color color)
		{
			var colorData = new Color[target.Width * target.Height];

			for (int i = 0; i < colorData.Length; i++)
				colorData[i] = color;

			gameContentManager.SpMiniMap.Texture.SetData(0, target, colorData, 0, target.Width * target.Height);
		}
	}
}
