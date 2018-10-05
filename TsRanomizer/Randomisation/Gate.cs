using System;

namespace TsRanodmizer.Randomisation
{
	abstract class Gate
	{
		public abstract bool CanOpen(ProgressionItem obtainedProgressionItems);

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

		public static Gate operator &(Gate a, ProgressionItem b)
		{
			return a & new ProgressionItemGate(b);
		}

		public static Gate operator &(ProgressionItem b, Gate a)
		{
			return a & new ProgressionItemGate(b);
		}

		public static Gate operator |(Gate a, ProgressionItem b)
		{
			return a | new ProgressionItemGate(b);
		}

		public static Gate operator |(ProgressionItem b, Gate a)
		{
			return a | new ProgressionItemGate(b);
		}

		public static explicit operator Gate(ProgressionItem requiredItems)
		{
			return new ProgressionItemGate(requiredItems);
		}

		class ProgressionItemGate : Gate
		{
			public readonly ProgressionItem RequiredItems;

			public ProgressionItemGate(ProgressionItem requiredItems)
			{
				RequiredItems = requiredItems;
			}

			public override bool CanOpen(ProgressionItem obtainedProgressionItems)
			{
				return RequiredItems == ProgressionItem.None || RequiredItems.HasMatchingFlag(obtainedProgressionItems);
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

			public override bool CanOpen(ProgressionItem obtainedProgressionItems)
			{
				return a.CanOpen(obtainedProgressionItems) && b.CanOpen(obtainedProgressionItems);
			}

			public override string ToString()
			{
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

			public override bool CanOpen(ProgressionItem obtainedProgressionItems)
			{
				return a.CanOpen(obtainedProgressionItems) || b.CanOpen(obtainedProgressionItems);
			}

			public override string ToString()
			{
				return $"({a} | {b})";
			}
		}
	}
}