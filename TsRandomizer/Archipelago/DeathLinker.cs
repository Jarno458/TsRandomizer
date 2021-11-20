using System;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using TsRandomizer.Screens;

namespace TsRandomizer.Archipelago
{
	class DeathLinker
	{
		readonly DeathLinkService service;

		volatile DeathLink lastDeathLink;
		volatile bool rip;
		volatile EAFSM lastState;

		public DeathLinker(DeathLinkService service)
		{
			this.service = service;

			service.OnDeathLinkReceived += OnDeathLinkReceived;
		}

		void OnDeathLinkReceived(DeathLink deathlink)
		{
			if (lastDeathLink == null || deathlink.Timestamp - lastDeathLink.Timestamp > TimeSpan.FromSeconds(5))
			{
				lastDeathLink = deathlink;
				rip = true;
				lastState = EAFSM.Dying;
			}
		}

		public void Update(Level level, ScreenManager screenManager)
		{
			if (level.MainHero == null)
				return;

			if (rip)
			{
				rip = false;

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
					var deathLink = new DeathLink("Yo momma");
					lastDeathLink = deathLink;
					service.SendDeathLink(deathLink);
				}

				lastState = level.MainHero.CurrentState;
			}
		}
	}
}
