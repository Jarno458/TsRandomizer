using System.Linq;
using TsRandomizer.Extensions;

namespace TsRandomizer.Randomisation
{
	abstract class Gate
	{
		public abstract bool CanBeOpenedWith(Requirement obtainedRequirements);

		public abstract bool Requires(Requirement requirementsToCheck);

		public static Gate operator &(Gate a, Gate b)
		{
			return new AndGate(a, b);
		}

		public static Gate operator |(Gate a, Gate b)
		{
			if(a is RequirementGate requirementGateA && b is RequirementGate requirementGateB)
				return new RequirementGate(requirementGateA.Requirements | requirementGateB.Requirements);

			return new OrGate(a, b);
		}

		public static Gate operator &(Gate a, Requirement b)
		{
			return a & new RequirementGate(b);
		}

		public static Gate operator &(Requirement b, Gate a)
		{
			return a & new RequirementGate(b);
		}

		public static Gate operator |(Gate a, Requirement b)
		{
			return a | new RequirementGate(b);
		}

		public static Gate operator |(Requirement b, Gate a)
		{
			return a | new RequirementGate(b);
		}

		public static explicit operator Gate(Requirement requiredItems)
		{
			return new RequirementGate(requiredItems);
		}

		internal class RequirementGate : Gate
		{
			public readonly Requirement Requirements;

			public RequirementGate(Requirement requirements)
			{
				Requirements = requirements;
			}

			public override bool CanBeOpenedWith(Requirement obtainedRequirements)
			{
				return Requirements == Requirement.None || Requirements.Contains(obtainedRequirements);
			}

			public override bool Requires(Requirement requirementsToCheck)
			{
				return Requirements == Requirement.None || ((ulong)Requirements & (ulong)requirementsToCheck) > 0;
			}

			public override string ToString()
			{
				return $"{Requirements}";
			}
		}

		internal class AndGate : Gate
		{
			public Gate[] Gates;

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

			public override bool CanBeOpenedWith(Requirement obtainedRequirements)
			{
				return Gates.All(g => g.CanBeOpenedWith(obtainedRequirements));
			}

			public override bool Requires(Requirement requirementsToCheck)
			{
				return Gates.Any(g => g.Requires(requirementsToCheck));
			}

			public override string ToString()
			{
				// ReSharper disable once CoVariantArrayConversion
				return $"AND({string.Join(",", (object[])Gates)})";
			}
		}

		internal class OrGate : Gate
		{
			public Gate[] Gates;

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

			public override bool CanBeOpenedWith(Requirement obtainedRequirements)
			{
				return Gates.Any(g => g.CanBeOpenedWith(obtainedRequirements));
			}

			public override bool Requires(Requirement requirementsToCheck)
			{
				return Gates.All(g => g.Requires(requirementsToCheck));
			}

			public override string ToString()
			{
				// ReSharper disable once CoVariantArrayConversion
				return $"OR({string.Join(",", (object[])Gates)})";
			}
		}
	}
}