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

		public bool deathLinkEnabled = true;

		public DeathLinker(DeathLinkService service)
		{
			this.service = service;

			service.OnDeathLinkReceived += OnDeathLinkReceived;
		}

		void OnDeathLinkReceived(DeathLink deathLink)
		{
			if (!deathLinkEnabled)
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
			if (level.MainHero == null || !deathLinkEnabled)
				return;

			if (rip)
			{
				rip = false;

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
					service.SendDeathLink(deathLink);
				}

				lastState = level.MainHero.CurrentState;
			}
		}
	}
}
