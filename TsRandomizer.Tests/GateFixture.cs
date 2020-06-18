using NUnit.Framework;
using TsRanodmizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
    public class GateFixture
	{
		 static readonly object[] ShouldBeRequired = {
			new ulong[]{ Requirement.DoubleJump, Requirement.DoubleJump},
			new ulong[]{ Requirement.DoubleJump, Requirement.DoubleJump | Requirement.AntiWeed},
			new ulong[]{ Requirement.None, Requirement.DoubleJump },
			new ulong[]{ Requirement.DoubleJump | Requirement.AntiWeed, Requirement.DoubleJump},
		};

		[TestCaseSource(nameof(ShouldBeRequired))]
		public void Should_Be_required(ulong gateRequirements, ulong requirementsToCheck)
		{
			var gate = new Gate.RequirementGate(gateRequirements);

			Assert.That(gate.Requires(requirementsToCheck), Is.True, 
				$"Gate of {(Requirement)gateRequirements} should require {(Requirement)requirementsToCheck}");
		}
	}
}
