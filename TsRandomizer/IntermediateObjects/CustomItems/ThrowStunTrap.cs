using System;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.Heroes;
using TsRandomizer.Extensions;
using TsRandomizer.Randomisation;
using TsRandomizer.Screens;

namespace TsRandomizer.IntermediateObjects.CustomItems
{
	class ThrowStunTrap : Trap
	{
		static readonly Type GrabScriptType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Raven.RavenBossGrabStunScript");
		static readonly Type ThrowScriptType = TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Raven.RavenBossThrowStunScript");

		public ThrowStunTrap(ItemUnlockingMap unlockingMap) : base(unlockingMap, CustomItemType.ThrowStunTrap) { }

		delegate void checkForHit(Protagonist lunais, ScriptAction throwScript);

		internal override void OnPickup(Level level, GameplayScreen gameplayScreen)
		{
			base.OnPickup(level, gameplayScreen);

			int damage = 0; //the script seems to ignore this number anyway
			var grabScript = (ScriptAction)GrabScriptType.CreateInstance(false, level, damage, level.MainHero.IsFacingLeft);
			var throwScript = (ScriptAction)ThrowScriptType.CreateInstance(false, level, damage, level.MainHero);

			Yeet(level.MainHero, grabScript, throwScript);
		}

		internal void Yeet(Protagonist lunais, ScriptAction grabScript, ScriptAction throwScript)
		{
			lunais.AddScriptAction(grabScript);
			lunais.AddScriptAction(throwScript);

			lunais.PlayCue(Timespinner.GameAbstractions.ESFX.BossBirdAuraPush);

			checkForHit checkForHitTask = DidWeHitSomething;
			checkForHitTask.BeginInvoke(lunais, throwScript, ClearTimer, new { throwScript, lunais });
		}

		internal void ClearTimer(dynamic parameters)
		{
			var script = (ScriptAction)parameters.AsyncState.throwScript;
			var lunais = (Protagonist)parameters.AsyncState.lunais;

			script.AsDynamic().ActionTimer = 0.0f;

			lunais.ManageSubtleDamage(lunais.MaxHP / 10, true, Timespinner.Core.EElementalWeaknessState.None);

			lunais.PlayCue(Timespinner.GameAbstractions.ESFX.LunaisTakeDamage);
			lunais.PlayCue(Timespinner.GameAbstractions.ESFX.VO_Lun_TakeDamage);
		}

		internal void DidWeHitSomething(Protagonist lunais, ScriptAction throwScript)
		{
			var dynamicScript = throwScript.AsDynamic();
			float lastXPosition = lunais.FloatPosition.X;

			bool throwCompleted = false;
			float lastCheck = dynamicScript.ActionTimer;

			/*didn't use velocity because the velocity just kept getting bigger and bigger even if lunais
				was stopped on screen*/
			while (!throwCompleted)
			{
				if (lastCheck - dynamicScript.ActionTimer > 0.005)
				{
					lastCheck = dynamicScript.ActionTimer;
					if (lastXPosition == lunais.FloatPosition.X) throwCompleted = true;
					lastXPosition = lunais.FloatPosition.X;
					if (Math.Abs(lunais.Velocity.X) > 1000)
					{
						//lunais please do not clip through stairs and slopes thx <3
						if (lunais.Velocity.X > 0) lunais.Velocity = new Microsoft.Xna.Framework.Vector2(1000, lunais.Velocity.Y);
						else lunais.Velocity = new Microsoft.Xna.Framework.Vector2(-1000, lunais.Velocity.Y);
					}
				}
			}
		}
	}
}