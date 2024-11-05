﻿using System;

namespace TsRandomizer.Randomisation
{
	static class WarpNames
	{
		public static string Get(Requirement gate)
		{
			// Present
			if (gate.Contains(Requirement.GateKittyBoss))
				return "Sewers";
			if (gate.Contains(Requirement.GateLeftLibrary))
				return "Library";
			if (gate.Contains(Requirement.GateMilitaryGate))
				return "Military Hangar";
			if (gate.Contains(Requirement.GateSealedCaves))
				return "Xarion's Cave Entrance";
			if (gate.Contains(Requirement.GateXarion))
				return "Xarion's Lair";
			if (gate.Contains(Requirement.GateSealedSirensCave))
				return "Sirens' Cave";
			if (gate.Contains(Requirement.GateLakeDesolation))
				return "Lake Desolation";
			if (gate.Contains(Requirement.GateLabEntrance))
				return "Amadeus' Laboratory";
			if (gate.Contains(Requirement.GateDadsTower))
				return "Emperor's Tower";

			// Past
			if (gate.Contains(Requirement.GateRefugeeCamp))
				return "Refugee Camp";
			if (gate.Contains(Requirement.GateLakeSereneLeft))
				return "Azure Queen";
			if (gate.Contains(Requirement.GateAccessToPast))
				return "Upper Caves of Banishment";
			if (gate.Contains(Requirement.GateLakeSereneRight))
				return "East Lake Serene";
			if (gate.Contains(Requirement.GateCastleRamparts))
				return "Castle Ramparts";
			if (gate.Contains(Requirement.GateCastleKeep))
				return "Castle Keep";
			if (gate.Contains(Requirement.GateRoyalTowers))
				return "Royal Towers";
			if (gate.Contains(Requirement.GateMaw))
				return "Maw's Lair";
			if (gate.Contains(Requirement.GateCavesOfBanishment))
				return "Maw's Cave Entrance";

			// Ancient Pyramid
			if (gate.Contains(Requirement.GateGyre))
				return "Temporal Gyre Entrance";
			if (gate.Contains(Requirement.GateLeftPyramid))
				return "Ancient Pyramid Entrance";
			if (gate.Contains(Requirement.GateRightPyramid))
				return "Inner Ancient Pyramid";

			throw new Exception($"Name for Gate {gate} is not defined");
		}
	}
}