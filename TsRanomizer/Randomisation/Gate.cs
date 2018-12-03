namespace TsRanodmizer.Randomisation
{
	abstract class Gate
	{
		public abstract bool CanOpen(Requirement obtainedRequirements);

		public static Gate operator &(Gate a, Gate b)
		{
			return new AndGate(a, b);
		}

		public static Gate operator |(Gate a, Gate b)
		{
			if(a is ProgressionItemGate x && b is ProgressionItemGate y)
				return new ProgressionItemGate(x.RequiredItems | y.RequiredItems);

			return new OrGate(a, b);
		}

		public static Gate operator &(Gate a, Requirement b)
		{
			return a & new ProgressionItemGate(b);
		}

		public static Gate operator &(Requirement b, Gate a)
		{
			return a & new ProgressionItemGate(b);
		}

		public static Gate operator |(Gate a, Requirement b)
		{
			return a | new ProgressionItemGate(b);
		}

		public static Gate operator |(Requirement b, Gate a)
		{
			return a | new ProgressionItemGate(b);
		}

		public static explicit operator Gate(Requirement requiredItems)
		{
			return new ProgressionItemGate(requiredItems);
		}

		class ProgressionItemGate : Gate
		{
			public readonly Requirement RequiredItems;

			public ProgressionItemGate(Requirement requiredItems)
			{
				RequiredItems = requiredItems;
			}

			public override bool CanOpen(Requirement obtainedRequirements)
			{
				return RequiredItems == Requirement.None || RequiredItems.Contains(obtainedRequirements);
			}

			public override string ToString()
			{
				return $"({RequiredItems})".Replace(", ", "|");
			}
		}

		class AndGate : Gate
		{
			readonly Gate a;
			readonly Gate b;

			internal AndGate(Gate a, Gate b)
			{
				this.a = a;
				this.b = b;
			}

			public override bool CanOpen(Requirement obtainedRequirements)
			{
				return a.CanOpen(obtainedRequirements) && b.CanOpen(obtainedRequirements);
			}

			public override string ToString()
			{
				if (a is ProgressionItemGate x && b is ProgressionItemGate y)
				{
					if (x.RequiredItems == Requirement.TimeStop && y.RequiredItems == Requirement.TimespinnerSpindle)
						return "(AccessToPast)";
					if (x.RequiredItems == (Requirement.DoubleJump | Requirement.UpwardDash) 
					    && y.RequiredItems == Requirement.TimeStop)
						return "(DoubleJumpOfNpc)";
				}

				return $"({a} & {b})";
			}
		}

		class OrGate : Gate
		{
			readonly Gate a;
			readonly Gate b;

			internal OrGate(Gate a, Gate b)
			{
				this.a = a;
				this.b = b;
			}

			public override bool CanOpen(Requirement obtainedRequirements)
			{
				return a.CanOpen(obtainedRequirements) || b.CanOpen(obtainedRequirements);
			}

			public override string ToString()
			{
				return $"({a} | {b})";
			}
		}
	}
}