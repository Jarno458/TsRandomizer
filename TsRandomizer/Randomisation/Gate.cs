using System;
using System.Linq;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	abstract class Gate : IEquatable<Gate>
	{
		public abstract bool CanBeOpenedWith(Requirement obtainedRequirements);

		public abstract bool Requires(Requirement requirementsToCheck);

		public abstract Gate[] Gates { get; }

		public static Gate operator &(Gate a, Gate b)
		{
			if (a is RequirementGate requirementGateA && requirementGateA.Requirements == Requirement.None)
				return b is RequirementGate requirementGateB && requirementGateB.Requirements == Requirement.None
					? (Gate)Requirement.None
					: b;

			if (b is RequirementGate rGateB && rGateB.Requirements == Requirement.None)
				return a;

			return new AndGate(a, b);
		} 

		public static Gate operator |(Gate a, Gate b)
		{
			if (a is RequirementGate rGateA)
			{
				if (rGateA.Requirements == Requirement.None)
					return (Gate)Requirement.None;

				if (b is RequirementGate rGateB)
				{
					if (rGateB.Requirements == Requirement.None)
						return (Gate)Requirement.None;
					
					return new RequirementGate(rGateA.Requirements | rGateB.Requirements);
				}
			}

			return new OrGate(a, b);
		}

        public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Gate other) => ToString() == other.ToString();

        public static Gate operator &(Gate a, Requirement b) => a & (Gate)b;

		public static Gate operator &(Requirement b, Gate a) => a & (Gate)b;

		public static Gate operator |(Gate a, Requirement b) => a | (Gate)b;

		public static Gate operator |(Requirement b, Gate a) => a | (Gate)b;

		public static explicit operator Gate(Requirement requiredItems) => new RequirementGate(requiredItems);

		internal class RequirementGate : Gate
		{
			public readonly Requirement Requirements;

			public override Gate[] Gates { get; }

			public RequirementGate(Requirement requirements)
			{
				Requirements = requirements;
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				Requirements == Requirement.None || Requirements.Contains(obtainedRequirements);

			public override bool Requires(Requirement requirementsToCheck) =>
				Requirements == Requirement.None || ((ulong)Requirements & (ulong)requirementsToCheck) > 0;

			public override string ToString() => $"{Requirements}";
		}

		internal class AndGate : Gate
		{
			public override Gate[] Gates { get; }

			internal AndGate(Gate a, Gate b)
			{
				if (a is AndGate andGateA && b is AndGate andGateB)
					Gates = andGateA.Gates.Union(andGateB.Gates).ToArray();
				else if (a is AndGate gateA)
					Gates = gateA.Gates.Concat(b).ToArray();
				else if (b is AndGate gateB)
					Gates = gateB.Gates.Concat(a).ToArray();
				else
					Gates = new[] {a, b};
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				Gates.All(g => g.CanBeOpenedWith(obtainedRequirements));

			public override bool Requires(Requirement requirementsToCheck) =>
				Gates.Any(g => g.Requires(requirementsToCheck));

			public override string ToString() => $"AND({string.Join(",", Gates.Select(g => g.ToString()))})";
		}

		internal class OrGate : Gate
		{
			public override Gate[] Gates { get; }

			internal OrGate(Gate a, Gate b)
			{
				if (a is OrGate orGateA && b is OrGate orGateB)
					Gates = orGateA.Gates.Union(orGateB.Gates).ToArray();
				else if (a is OrGate gateA)
					Gates = gateA.Gates.Concat(b).ToArray();
				else if (b is OrGate gateB)
					Gates = gateB.Gates.Concat(a).ToArray();
				else
					Gates = new[] { a, b };
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				Gates.Any(g => g.CanBeOpenedWith(obtainedRequirements));

			public override bool Requires(Requirement requirementsToCheck) =>
				Gates.All(g => g.Requires(requirementsToCheck));

			public override string ToString() => $"OR({string.Join(",", Gates.Select(g => g.ToString()))})";
		}
	}
}