using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;
using Timespinner.GameObjects.BaseClasses;
using Microsoft.Xna.Framework;
using Timespinner.GameAbstractions;
using System.Collections.Generic;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class LightsOutTrap : Trap
	{
		public LightsOutTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.LightsOutTrap) { }

		delegate void checkForHit(Protagonist lunais, ScriptAction throwScript);

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);
			Color newColor = Color.Black;
			foreach (List<Tile> tileList in level.AsDynamic().BackgroundTiles.Values)
			{
				if (tileList != null)
				{
					foreach (Tile tile in tileList)
						tile.AsDynamic().SetDrawColor(newColor);
				}
			}
			foreach (Tile tile in level.AsDynamic().SolidTiles.Values)
				tile.AsDynamic().SetDrawColor(newColor);

			level.JukeBox.PlayCue(ESFX.BossDemonSpotlight);
		}
	}
}
