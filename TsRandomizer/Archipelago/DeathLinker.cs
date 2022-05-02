using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.Archipelago
{
	class DeathLinker
	{
		readonly SettingCollection settings;
		readonly GameSave gamesave;

		volatile DeathLink lastDeathLink;
		volatile bool rip;
		volatile EAFSM lastState;

		public DeathLinker(GameSave gamesave, SettingCollection settings)
		{
			this.gamesave = gamesave;
			this.settings = settings;

			settings.DeathLinkSetting.OnValueChangedAction += OnDeathLinkSettingChanged;
			Client.OnDeathLinkAction = OnDeathLinkReceived;
		}

		void OnDeathLinkSettingChanged(bool deathLinkEnabled)
		{
			gamesave.SetValue("DeathLinkEnabled", deathLinkEnabled);
			gamesave.SetValue("DeathLinkDisabled", !deathLinkEnabled);
			if (deathLinkEnabled)
			{
				Client.AddTag("DeathLink");
				Client.AddDeathLinker();
			}
			else
			{
				Client.RemoveTag("DeathLink");
			}
		}

		void OnDeathLinkReceived(DeathLink deathLink)
		{
			if (lastDeathLink != null && deathLink.Timestamp - lastDeathLink.Timestamp <= TimeSpan.FromSeconds(5))
				return;

			lastDeathLink = deathLink;
			if (!settings.DeathLinkSetting.Value)
				return;

			rip = true;
			lastState = EAFSM.Dying;
		}

		public void Update(Level level, ScreenManager screenManager)
		{
			if (level.MainHero == null || !settings.DeathLinkSetting.Value)
				return;

			if (rip)
			{
				rip = false;
				if (level.ID == 17 || (level.ID == 16 && level.RoomID == 27))
				{
					lastState = level.MainHero.CurrentState;
					return; //Do not kill the player during the ending.
				}

				ScreenManager.Console.AddLine( 
					!string.IsNullOrEmpty(lastDeathLink.Cause)
						? $"DeathLink received from {lastDeathLink.Source}, Reason: {lastDeathLink.Cause}"
						: $"DeathLink received from {lastDeathLink.Source}");

				var message = $"Your soul was linked across time to {lastDeathLink.Source} who has perished, and so have you!";

				if (!string.IsNullOrEmpty(lastDeathLink.Cause))
					message += $"\nThey died of: {lastDeathLink.Cause}";

				var messageBox = MessageBox.Create(screenManager, message);

				screenManager.AddScreen(messageBox.Screen, null);

				level.MainHero.Kill();
			}
			else
			{
				if (level.MainHero.CurrentState == EAFSM.Dying && lastState != EAFSM.Dying)
				{
					var deathLink = new DeathLink(Client.GetCurrentPlayerName());
					lastDeathLink = deathLink;
					Client.SendDeathLink(deathLink);
				}

				lastState = level.MainHero.CurrentState;
			}
		}
	}
}
