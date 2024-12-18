using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace TsRandomizer.Randomisation
{
	struct Requirement : IEquatable<Requirement>
	{
		static readonly Dictionary<Requirement, string> Flags;

		public static readonly Requirement AllGates;

		public static readonly Requirement None = 0UL;
		public static readonly Requirement TimeStop = 1UL << 0;
		public static readonly Requirement ForwardDash = 1UL << 1;
		public static readonly Requirement Fire = 1UL << 3;
		public static readonly Requirement CardA = 1UL << 4;
		public static readonly Requirement CardB = 1UL << 5;
		public static readonly Requirement CardC = 1UL << 6;
		public static readonly Requirement CardD = 1UL << 7;
		public static readonly Requirement CardE = 1UL << 8;
		public static readonly Requirement CardV = 1UL << 9;
		public static readonly Requirement TimespinnerSpindle = 1UL << 10;
		public static readonly Requirement Swimming = 1UL << 11;
		public static readonly Requirement UpwardDash = 1UL << 12;
		public static readonly Requirement DoubleJump = 1UL << 13;
		public static readonly Requirement GasMask = 1UL << 14;
		public static readonly Requirement Teleport = 1UL << 15;
		public static readonly Requirement TimespinnerPiece1 = 1UL << 21;
		public static readonly Requirement TimespinnerPiece2 = 1UL << 22;
		public static readonly Requirement TimespinnerPiece3 = 1UL << 23;
		public static readonly Requirement PinkOrb = 1UL << 25;
		public static readonly Requirement TimespinnerWheel = 1UL << 26;
		public static readonly Requirement Tablet = 1UL << 27;
		public static readonly Requirement OculusRift = 1UL << 28;
		public static readonly Requirement Kobo = 1UL << 29;
		public static readonly Requirement MerchantCrow = 1UL << 30;
		public static readonly Requirement LaserA = 1UL << 31;
		public static readonly Requirement LaserI = 1UL << 32;
		public static readonly Requirement LaserM = 1UL << 33;
		public static readonly Requirement LabGenza = 1UL << 34;
		public static readonly Requirement LabDynamo = 1UL << 35;
		public static readonly Requirement LabExperiment = 1UL << 36;
		public static readonly Requirement LabResearch = 1UL << 37;
		public static readonly Requirement DrawbridgeKey = 1UL << 38;

		public static readonly Requirement GateRefugeeCamp = 1UL << 42;
		public static readonly Requirement GateSealedCaves = 1UL << 43;
		public static readonly Requirement GateMaw = 1UL << 44;
		public static readonly Requirement GateCavesOfBanishment = 1UL << 45;
		public static readonly Requirement GateCastleRamparts = 1UL << 46;
		public static readonly Requirement GateCastleKeep = 1UL << 47;
		public static readonly Requirement GateRoyalTowers = 1UL << 48;
		public static readonly Requirement GateMilitaryGate = 1UL << 49;
		public static readonly Requirement GateKittyBoss = 1UL << 50;
		public static readonly Requirement GateLeftLibrary = 1UL << 51;
		public static readonly Requirement GateSealedSirensCave = 1UL << 52;
		public static readonly Requirement GateLakeSereneLeft = 1UL << 53;
		public static readonly Requirement GateLakeSereneRight = 1UL << 54;
		public static readonly Requirement GateAccessToPast = 1UL << 55;
		public static readonly Requirement GateLakeDesolation = 1UL << 56;
		public static readonly Requirement GateXarion = 1UL << 57;
		public static readonly Requirement GateGyre = 1UL << 58;
		public static readonly Requirement GateLeftPyramid = 1UL << 59;
		public static readonly Requirement GateRightPyramid = 1UL << 60;
		public static readonly Requirement GateLabEntrance = 1UL << 61;
		public static readonly Requirement GateDadsTower = 1UL << 62;

		public static readonly Requirement Free = 1UL << 63;

		readonly ulong flags;

		static Requirement()
		{
			Flags = typeof(Requirement)
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.Where(f => f.Name != nameof(None))
				.ToDictionary(f => (Requirement)f.GetValue(null), f => f.Name);

			AllGates = typeof(Requirement)
				.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
				.Where(f => f.Name.StartsWith("Gate"))
				.Select(f => (Requirement)f.GetValue(null))
				.Aggregate((a, b) => a | b);
		}

		Requirement(ulong flags)
		{
			this.flags = flags;
		}

		[Pure]
		public bool Contains(Requirement other) => (flags & other.flags) > 0;

		[Pure]
		public bool IsSingleRequirement() => (flags & (flags - 1)) == 0;

		[Pure]
		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return obj is Requirement item && Equals(item);
		}

		[Pure]
		public bool Equals(Requirement other) => flags == other.flags;

		[Pure]
		public override int GetHashCode() => flags.GetHashCode();

		public static implicit operator Requirement(ulong value) => new Requirement(value);
		public static implicit operator ulong(Requirement value) => value.flags;

		public static Requirement operator |(Requirement a, Requirement b)
		{
			if (a == Free || b == Free)
				return Free;

			return a.flags | b.flags;
		}

		public static Gate operator &(Requirement a, Requirement b) => (Gate)a & b;
		public static bool operator ==(Requirement a, Requirement b) => a.Equals(b);
		public static bool operator !=(Requirement a, Requirement b) => !(a == b);

		public override string ToString() => GetFlagNames(flags);

		static string GetFlagNames(ulong flags)
		{
			if (flags == None) return "";
			if (flags == Free) return "Free";

			var flagNames = Flags
				.Where(f => ((ulong)f.Key & flags) > 0)
				.Select(f => ToShortName(f.Value));

			return string.Join("|", flagNames);
		}

		static string ToShortName(string name)
		{
			switch (name)
			{
				case "TimeStop": return "TS";
				case "ForwardDash": return "FD";
				case "Lightwall": return "LW";
				case "Fire": return "Fire";
				case "CardA": return "cA";
				case "CardB": return "cB";
				case "CardC": return "cC";
				case "CardD": return "cD";
				case "CardE": return "cE";
				case "CardV": return "cV";
				case "TimespinnerSpindle": return "Spindle";
				case "Swimming": return "Sw";
				case "GasMask": return "Gas";
				case "Teleport": return "TP";
				case "PinkOrb": return "PO";
				default:
					return name;
			}
		}
	}
}
