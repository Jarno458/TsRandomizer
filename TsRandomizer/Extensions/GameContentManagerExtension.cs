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

			gameContentManager.SpMiniMap.Texture.SetData(0, target, colorData, 0, colorData.Length);
		}

		public static void ChangeLabWaterColor(this GCM gameContentManager)
		{
			var tiles = gameContentManager.TsEventTiles;

			var labWaterTile = tiles.GetFrameSource(7);
			var labWaterTileColors = new Color[labWaterTile.Width * labWaterTile.Height];

			gameContentManager.TsEventTiles.Texture.GetData(0, labWaterTile, labWaterTileColors, 0, labWaterTileColors.Length);

			for (int i = 0; i < labWaterTileColors.Length; i++)
			{
				labWaterTileColors[i].G = (byte)(labWaterTileColors[i].G * 0.6f);
				labWaterTileColors[i].A = 1;
			}

			gameContentManager.TsEventTiles.Texture.SetData(0, labWaterTile, labWaterTileColors, 0, labWaterTileColors.Length);

			var leftLabWaterTopTile = tiles.GetFrameSource(8);
			var rightLabWaterTopTile = tiles.GetFrameSource(13);

			var frameSource = new Rectangle(leftLabWaterTopTile.X, leftLabWaterTopTile.Y,
				(rightLabWaterTopTile.X + rightLabWaterTopTile.Width) - leftLabWaterTopTile.X,
				(rightLabWaterTopTile.Y + rightLabWaterTopTile.Height) - leftLabWaterTopTile.Y);

			var labWaterTopTileColors = new Color[frameSource.Width * frameSource.Height];

			gameContentManager.TsEventTiles.Texture.GetData(0, frameSource, labWaterTopTileColors, 0, labWaterTopTileColors.Length);

			for (int i = 0; i < labWaterTopTileColors.Length; i++)
			{
				if (labWaterTopTileColors[i].A == 0) 
					continue;

				labWaterTopTileColors[i].A = 1;

				if (labWaterTopTileColors[i].R == 16 && labWaterTopTileColors[i].G == 112 && labWaterTopTileColors[i].B == 24)
				{
					labWaterTopTileColors[i].R = labWaterTileColors[0].R;
					labWaterTopTileColors[i].G = labWaterTileColors[0].G;
					labWaterTopTileColors[i].B = labWaterTileColors[0].B;
				}
				else
				{
					labWaterTopTileColors[i].G = (byte)(labWaterTopTileColors[i].G * 0.6f);
				}
			}

			gameContentManager.TsEventTiles.Texture.SetData(0, frameSource, labWaterTopTileColors, 0, labWaterTopTileColors.Length);
		}
	}
}
