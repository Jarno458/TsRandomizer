using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Screens;
using TsRandomizer.Settings;

namespace TsRandomizer.Archipelago
{
	class DeathLinker
	{
		readonly DeathLinkService service;
		readonly SettingCollection settings;

		volatile DeathLink lastDeathLink;
		volatile bool rip;
		volatile EAFSM lastState;

		public DeathLinker(SettingCollection settings, DeathLinkService service)
		{
			this.service = service;
			this.settings = settings;

			service.OnDeathLinkReceived += OnDeathLinkReceived;

			service.EnableDeathLink();
		}

		void OnDeathLinkReceived(DeathLink deathLink)
		{
			if (!settings.DeathLink.Value)
				return;

			if (lastDeathLink == null || deathLink.Timestamp - lastDeathLink.Timestamp > TimeSpan.FromSeconds(5))
			{
				lastDeathLink = deathLink;
				rip = true;
				lastState = EAFSM.Dying;
			}
		}

		public void Update(Level level, ScreenManager screenManager)
		{
			if (!settings.DeathLink.Value || level.MainHero == null)
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

					try
					{
						service.SendDeathLink(deathLink);
					}
					catch {}
				}

				lastState = level.MainHero.CurrentState;
			}
		}
	}
}
