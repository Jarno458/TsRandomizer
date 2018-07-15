namespace TsRanodmizer.Randomisation
{
    class Gate
    {
        readonly ProgressionItem requiredItems;

        public Gate(ProgressionItem requiredItems)
        {
            this.requiredItems = requiredItems;
        }

        public virtual bool CanOpen(ProgressionItem obtainedProgressionItems)
        {
            return requiredItems == ProgressionItem.None || requiredItems.HasMatchingFlag(obtainedProgressionItems);
        }

        public static Gate operator &(Gate a, Gate b)
        {
            return new AndGate(a, b);
        }

        public static Gate operator |(Gate a, Gate b)
        {
            return new OrGate(a, b);
        }

        public static Gate operator &(Gate a, ProgressionItem b)
        {
            return new AndGate(a, new Gate(b));
        }

        public static Gate operator &(ProgressionItem b, Gate a)
        {
            return a & b;
        }

        public static Gate operator |(Gate a, ProgressionItem b)
        {
            return new OrGate(a, new Gate(b));
        }

        public static Gate operator |(ProgressionItem b, Gate a)
        {
            return a | b;
        }

        public override string ToString()
        {
            return $"({requiredItems})".Replace(", ", "|");
        }

        class AndGate : Gate
        {
            readonly Gate a;
            readonly Gate b;

            internal AndGate(Gate a, Gate b) : base(ProgressionItem.None)
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

            internal OrGate(Gate a, Gate b) : base(ProgressionItem.None)
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