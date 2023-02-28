using System;
using System.Collections.Generic;
using System.Linq;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	abstract class Gate : IEquatable<Gate>
	{
		public abstract bool CanBeOpenedWith(Requirement obtainedRequirements);

		public static Gate operator &(Gate a, Gate b)
		{
			if (a is RequirementGate requirementGateA && requirementGateA.Requirements == Requirement.Free)
				return b;
			if (b is RequirementGate requirementGateB && requirementGateB.Requirements == Requirement.Free)
				return a;

			return new AndGate(a, b);
		} 

		public static Gate operator |(Gate a, Gate b)
		{
			if (
					(a is RequirementGate requirementGateA && requirementGateA.Requirements == Requirement.Free)
					|| (b is RequirementGate requirementGateB && requirementGateB.Requirements == Requirement.Free)
				)
				return (Gate)Requirement.Free;

			return new OrGate(a, b);
		}

        public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Gate other) => ToString() == other.ToString();

        public static Gate operator &(Gate a, Requirement b) => a & (Gate)b;

		public static Gate operator &(Requirement b, Gate a) => a & (Gate)b;

		public static Gate operator |(Gate a, Requirement b) => a | (Gate)b;

		public static Gate operator |(Requirement b, Gate a) => a | (Gate)b;

		public static implicit operator Gate(Requirement requiredItems) => new RequirementGate(requiredItems);

		internal class RequirementGate : Gate
		{
			internal Requirement Requirements { get; }

			public RequirementGate(Requirement requirements)
			{
				Requirements = requirements;
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				CanBeOpenedWith(Requirements, obtainedRequirements);
			
			public static bool CanBeOpenedWith(Requirement requirements, Requirement obtainedRequirements)
				=> requirements == Requirement.Free || requirements.Contains(obtainedRequirements);

			public override string ToString() => Requirements.ToString()
		}

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
			Requirement OrRequirements { get; }
			Gate[] AnyGates { get; }

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
					case AndGate _:
						gates.Add(a);
						break;
					case RequirementGate regGateA:
						OrRequirements |= regGateA.Requirements;
						break;
				}

				switch (b)
				{
					case OrGate orGateB:
						OrRequirements |= orGateB.OrRequirements;
						gates.AddRange(orGateB.AnyGates);
						break;
					case AndGate _:
						gates.Add(b);
						break;
					case RequirementGate regGateB:
						OrRequirements |= regGateB.Requirements;
						break;
				}

				AnyGates = gates.ToArray();
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				RequirementGate.CanBeOpenedWith(OrRequirements, obtainedRequirements)
				|| AnyGates.Any(g => g.CanBeOpenedWith(obtainedRequirements));

			public override string ToString() => $" OR({string.Join(" || ", AnyGates.Concat((Gate)OrRequirements).Select(g => g.ToString()))}) ";
		}
	}
}