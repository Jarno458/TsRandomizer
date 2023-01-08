using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.Extensions
{
	static class GameContentManagerExtension
	{
		static readonly HashSet<EMinimapRoomColor> Is3By3 =
			new HashSet<EMinimapRoomColor> {
				EMinimapRoomColor.Red,
				EMinimapRoomColor.Yellow,
				EMinimapRoomColor.SpecialBlue,
				EMinimapRoomColor.SpecialPurple
			};

		static readonly HashSet<EMinimapRoomColor> Is4By4 =
			new HashSet<EMinimapRoomColor> {
				MinimapScreen.EMinimapRoomColor_Hinted,
				MinimapScreen.EMinimapRoomColor_FinalBoss
			};

		public static void UpdateMinimapColors(this GCM gameContentManager, SettingCollection settings)
		{
			SetColor(gameContentManager, EMinimapRoomColor.Blue, settings.PastMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.Purple, settings.PresentMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.Green, settings.PyramidMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.Orange, settings.LootMinimapColor.Color);

			SetColor(gameContentManager, EMinimapRoomColor.Red, settings.SaveStatueMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.Yellow, settings.SpecailLootMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.SpecialBlue, settings.PresentTransitionMinimapColor.Color);
			SetColor(gameContentManager, EMinimapRoomColor.SpecialPurple, settings.PastTransitionMinimapColor.Color);

			SetColor(gameContentManager, MinimapScreen.EMinimapRoomColor_Hinted, settings.HintedMinimapColor.Color);
			SetColor(gameContentManager, MinimapScreen.EMinimapRoomColor_FinalBoss, settings.FinalBossMinimapColor.Color);
		}

		static void SetColor(GCM gameContentManager, EMinimapRoomColor colorIndex, Color newColor)
		{
			var target = gameContentManager.SpMiniMap.GetFrameSource((int)colorIndex);

			if (Is3By3.Contains(colorIndex))
			{
				target.X += 1;
				target.Y += 1;
				target.Width = 3;
				target.Height = 3;
			}
			else if (Is4By4.Contains(colorIndex))
			{
				target.Width = 4;
				target.Height = 4;
			}
			
			var colorData = new Color[target.Width * target.Height];

			for (int i = 0; i < colorData.Length; i++)
				colorData[i] = newColor;

			gameContentManager.SpMiniMap.Texture.SetData(0, target, colorData, 0, target.Width * target.Height);
		}
	}
}
