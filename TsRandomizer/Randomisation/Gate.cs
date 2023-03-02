using System;
using System.Collections.Generic;
using System.Linq;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	abstract class Gate : IEquatable<Gate>
	{
		public abstract bool CanBeOpenedWith(Requirement obtainedRequirements);

		public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Gate other) => ToString() == other.ToString();

        public static Gate operator &(Gate a, Requirement b) => a & (Gate)b;
		public static Gate operator &(Requirement b, Gate a) => a & (Gate)b;
		public static Gate operator &(Gate a, Gate b)
		{
			if (a is OrGate orGateA && orGateA.IsFree)
				return b;
			if (b is OrGate orGateB && orGateB.IsFree)
				return a;

			return new AndGate(a, b);
		}

		public static Gate operator |(Gate a, Requirement b) => a | (Gate)b;
		public static Gate operator |(Requirement b, Gate a) => a | (Gate)b;
		public static Gate operator |(Gate a, Gate b) => new OrGate(a, b);

		public static implicit operator Gate(Requirement requiredItems) => new OrGate(requiredItems);

		internal class AndGate : Gate
		{
			Gate[] AllGates { get; }

			internal AndGate(Gate a, Gate b)
			{
				if (a is AndGate andGateA && b is AndGate andGateB)
					AllGates = andGateA.AllGates.Union(andGateB.AllGates).ToArray();
				else if (a is AndGate gateA)
					AllGates = gateA.AllGates.Concat(b).ToArray();
				else if (b is AndGate gateB)
					AllGates = gateB.AllGates.Concat(a).ToArray();
				else
					AllGates = new[] {a, b};
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				AllGates.All(g => g.CanBeOpenedWith(obtainedRequirements));

			public override string ToString() => $" AND({string.Join(" && ", AllGates.Select(g => g.ToString()))}) ";
		}

		internal class OrGate : Gate
		{
			public Requirement OrRequirements { get; }
			Gate[] AnyGates { get; }

			internal OrGate(Requirement requirements)
			{
				OrRequirements = requirements;
				AnyGates = new Gate[0];
			}

			internal OrGate(Gate a, Gate b)
			{
				OrRequirements = Requirement.None;
				var gates = new List<Gate>(2);

				switch (a)
				{
					case OrGate orGateA:
						OrRequirements |= orGateA.OrRequirements;
						gates.AddRange(orGateA.AnyGates);
						break;
					default:
						gates.Add(a);
						break;
				}

				switch (b)
				{
					case OrGate orGateB:
						OrRequirements |= orGateB.OrRequirements;
						gates.AddRange(orGateB.AnyGates);
						break;
					default:
						gates.Add(b);
						break;
				}

				AnyGates = gates.ToArray();
			}

			public bool IsFree => OrRequirements == Requirement.Free && !AnyGates.Any();

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				OrRequirements == Requirement.Free || OrRequirements.Contains(obtainedRequirements)
				|| AnyGates.Any(g => g.CanBeOpenedWith(obtainedRequirements));

			public override string ToString()
			{
				if (!AnyGates.Any())
					return OrRequirements.ToString();

				var gateNames = AnyGates.Select(g => g.ToString());

				if (OrRequirements != Requirement.None)
					gateNames = gateNames.Concat(OrRequirements.ToString());

				return $" OR({string.Join(" || ", gateNames)}) ";
			}
		}
	}
}