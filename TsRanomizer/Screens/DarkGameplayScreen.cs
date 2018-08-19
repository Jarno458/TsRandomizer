using System;
using System.Collections.Generic;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.Heroes;
using TsRanodmizer.Extensions;

namespace TsRanodmizer.Screens
{
	// ReSharper disable once UnusedMember.Global
	partial class GameplayScreen
	{
		int levelId = 1;

		void LoadLevel(int levelId, int roomNumber, int whichCheckpoint)
		{
			var self = ScreenReflected;

			int num = self._level == null ? -1 : self._level.ID;
			if (levelId == 1 && roomNumber == 0 && whichCheckpoint == 0)
				levelId = 0;

			LoadLevel(new LevelChangeRequest
			{
				LevelID = levelId,
				RoomID = roomNumber,
				CheckpointID = whichCheckpoint,
				ShouldPlayLevelSong = levelId != num
			});
		}

		void LoadLevel(LevelChangeRequest levelChangeRequest)
		{
			var self = ScreenReflected;

			LevelSpecification levelSpec = null;
			int levelId = levelChangeRequest.LevelID;
			if (levelId < 0)
				levelId = 18;
			else if (levelId > 18)
				levelId = 0;

			try
			{
				levelSpec = LevelSpecification.FromCompressedFile(Level.GetLevelPathFromID(levelId, true));
				if (levelSpec == null || levelSpec.ID != levelId)
				{
					Console.WriteLine("Failed to load level.");
					self.ExitScreen();
					return;
				}
				levelSpec.ID = levelId;
			}
			catch
			{
				Console.WriteLine("Level does not exist!");
			}
			if (levelSpec == null)
				return;
			if (!self._cachedWarpBackgrounds.ContainsKey(levelSpec.ID))
				self._cachedWarpBackgrounds.Add(levelSpec.ID, (IEnumerable<BackgroundSpecification>)levelSpec.WarpBackgrounds.ToArray());
			if (self._level != null)
			{
				if (self._level.MainHero != null)
				{
					Protagonist mainHero = self._level.MainHero;
					var characterStats = ((CharacterStats)self.SaveFile.CharacterStats).Reflect();
					characterStats.HP = mainHero.HP;
					characterStats.Sand = mainHero.MP;
					characterStats.Aura = mainHero.Aura;
					characterStats.CurrentStatus = 0;
				}
				self._level.Dispose();
				self.ScreenManager.Jukebox.StopAllSFX();
				GC.Collect();
			}
			self._level = new Level(self._gcm, levelSpec, self._minimapSpecification, self.ScreenManager.Jukebox, self._gameScreenCenter, self._levelScreenCenter, self._gamePadWrapper.PlayerNumber, self.SaveFile, self.GameConfigSave, levelChangeRequest, self._cachedWarpBackgrounds)
			{
				BackgroundZoom = self._camZoom
			};
			if (levelChangeRequest.ShouldPlayLevelSong)
				self._isWaitingToPlayLevelSong = true;
			self._currentTransitionTimer = levelChangeRequest.FadeInTime > 0.0 ? levelChangeRequest.FadeInTime : 0.1f;
			self._fadeInOutMax = self._currentTransitionTimer;
			self.ScreenManager.Game.ResetElapsedTime();
		}

		void LoadNextLevel()
		{
			levelId++;

			if (levelId > 18)
				levelId = 1;

			LoadLevel(levelId, 0, 0);
		}
	}
}
