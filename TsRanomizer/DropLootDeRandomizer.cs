using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameObjects.Events.Misc;
using TsRanodmizer.Extensions;
using TsRanodmizer.IntermediateObjects;

namespace TsRanodmizer
{
	class DropLootDeRandomizer : Random
	{
		readonly Dictionary<MethodBase, Func<StackTrace, Random>> methodsToDeRandimize;
		readonly Dictionary<Type, Random> randomizersPerKey = new Dictionary<Type, Random>();

		readonly Random defaultRandom;
		readonly Seed seed;

		public DropLootDeRandomizer(Random defaultRandom, Seed seed)
		{
			this.defaultRandom = defaultRandom;
			this.seed = seed;

			methodsToDeRandimize = new Dictionary<MethodBase, Func<StackTrace, Random>> {
				// TODO check for moar
				{typeof(Monster).GetPrivateMethod("DropLoot"), s => RandomizerPerTypeInNamespace(s, "Timespinner.GameObjects.Enemies")},
				{typeof(RareEnemySpawnerEvent).GetPrivateMethod("SpawnEnemy"), RandomiserPerType},
				{TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Passives.BloodOrbPassive").GetMethod("Update"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Heroes.Familiars.FamiliarCrow").GetPrivateMethod("OnSuccessfulEnemyHit"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.LakeBirdEgg").GetPrivateMethod("OnAggroed"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Events.Lanterns.BaseLantern").GetPrivateMethod("DropLoot"), s => RandomizerPerTypeInNamespace(s, "Timespinner.GameObjects.Events.Lanterns") },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Enemies.LakeEel").GetMethod("Update"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Zel.ZelBossSpikeShard").GetPrivateMethod("Push"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Xarion.XarionBossMoth").GetPrivateMethod("StartNewFlightPhase"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Xarion.XarionBossCaterpillar").GetPrivateMethod("Reset"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Z_Raven.RavenBoss").GetPrivateMethod("ResetIdleTimer"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.Varndagroth.XarionBossMothProjectile").GetPrivateMethod("Reset"), RandomiserPerType },
				{TimeSpinnerType.Get("Timespinner.GameObjects.Bosses.OtherBosses.NightmareBoss").GetPrivateMethod("EmitFireBreath"), RandomiserPerType },


			};
		}

		public override int Next(int minValue, int maxValue)
		{
			return GetRandomProvider().Next(minValue, maxValue);
		}

		public override double NextDouble()
		{
			return GetRandomProvider().NextDouble();
		}

		Random GetRandomProvider()
		{
			var stackTrace = new StackTrace(2,false);
			var callerFrame = stackTrace.GetFrame(0);

			return methodsToDeRandimize.TryGetValue(callerFrame.GetMethod(), out Func<StackTrace, Random> randomizerFunc) 
				? randomizerFunc(stackTrace) 
				: defaultRandom;
		}

		Random RandomizerPerTypeInNamespace(StackTrace stackTrace, string nameSpace)
		{
			for (var i = 1; i < stackTrace.FrameCount; i++)
			{
				var type = stackTrace.GetFrame(i).GetMethod().ReflectedType;

				if (type == null || type.Namespace != nameSpace)
					continue;

				return GetRandomizerForType(type);
			}

			return defaultRandom;
		}

		Random RandomiserPerType(StackTrace stackTrace)
		{
			var type = stackTrace.GetFrame(0).GetMethod().ReflectedType;
			if (type == null)
				return defaultRandom;

			return GetRandomizerForType(type);
		}

		Random GetRandomizerForType(Type type)
		{
			if (randomizersPerKey.TryGetValue(type, out Random randomiserForKey))
				return randomiserForKey;

			var random = new Random(seed);
			randomizersPerKey.Add(type, random);

			return random;
		}
	}
}