using System;
using System.Collections.Generic;
using System.Linq;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	abstract class Gate : IEquatable<Gate>
	{
		public abstract bool CanBeOpenedWith(Requirement obtainedRequirements);

		public abstract bool Requires(Requirement requirementsToCheck);

		public abstract Gate[] Gates { get; }

		public List<Gate> GetRequirementGates()
        {
			return GetRequirementGates(new List<Gate>());
        }

		private List<Gate> GetRequirementGates(List<Gate> currentGates)
		{
			if (Gates.Length == 0)
			{
				currentGates.Add(this);
				return currentGates;
			}
			else
			{
				var subgates = Gates;
				foreach (var subgate in subgates)
				{
					if (subgate.Gates != null)
						subgate.GetRequirementGates(currentGates);
					else
					{
						currentGates.Add(subgate);
					}
				}
				return currentGates;
			}
		}

		public static Gate operator &(Gate a, Gate b) => new AndGate(a, b);

		public static Gate operator |(Gate a, Gate b)
		{
			if(a is RequirementGate requirementGateA && b is RequirementGate requirementGateB)
				return new RequirementGate(requirementGateA.Requirements | requirementGateB.Requirements);

			return new OrGate(a, b);
		}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Gate other)
        {
			return ToString() == other.ToString();
        }

        public static Gate operator &(Gate a, Requirement b) => a & new RequirementGate(b);

		public static Gate operator &(Requirement b, Gate a) => a & new RequirementGate(b);

		public static Gate operator |(Gate a, Requirement b) => a | new RequirementGate(b);

		public static Gate operator |(Requirement b, Gate a) => a | new RequirementGate(b);

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
			private Gate[] _gates;
			public override Gate[] Gates { get { return _gates; } }

			internal AndGate(Gate a, Gate b)
			{
				if (a is AndGate andGateA && b is AndGate andGateB)
					_gates = andGateA.Gates.Union(andGateB.Gates).ToArray();
				else if (a is AndGate gateA)
					_gates = gateA.Gates.Concat(b).ToArray();
				else if (b is AndGate gateB)
					_gates = gateB.Gates.Concat(a).ToArray();
				else
					_gates = new[] {a, b};
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				Gates.All(g => g.CanBeOpenedWith(obtainedRequirements));

			public override bool Requires(Requirement requirementsToCheck) =>
				Gates.Any(g => g.Requires(requirementsToCheck));

			public override string ToString() => $"AND({string.Join(",", Gates.Select(g => g.ToString()))})";
		}

		internal class OrGate : Gate
		{
			private Gate[] _gates;
			public override Gate[] Gates { get { return _gates; } }

			internal OrGate(Gate a, Gate b)
			{
				if (a is OrGate orGateA && b is OrGate orGateB)
					_gates = orGateA.Gates.Union(orGateB.Gates).ToArray();
				else if (a is OrGate gateA)
					_gates = gateA.Gates.Concat(b).ToArray();
				else if (b is OrGate gateB)
					_gates = gateB.Gates.Concat(a).ToArray();
				else
					_gates = new[] { a, b };
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements) =>
				Gates.Any(g => g.CanBeOpenedWith(obtainedRequirements));

			public override bool Requires(Requirement requirementsToCheck) =>
				Gates.All(g => g.Requires(requirementsToCheck));

			public override string ToString() => $"OR({string.Join(",", Gates.Select(g => g.ToString()))})";
		}
	}
}