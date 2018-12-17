
using NUnit.Framework;
using TsRanodmizer.Randomisation;

namespace TsRandomizer.Tests
{
	[TestFixture]
    public class GateFixture
	{
		static object[] shouldBeRequired = {
			new []{ Requirement.DoubleJump, Requirement.DoubleJump},
			new []{ Requirement.DoubleJump, Requirement.DoubleJump | Requirement.AntiWeed},
			new []{ Requirement.DoubleJump, Requirement.None},
			new []{ Requirement.DoubleJump | Requirement.AntiWeed, Requirement.DoubleJump},
		};
		
		[TestCaseSource(nameof(shouldBeRequired))]
	    public void Should_Be_required(Requirement gateRequirements, Requirement requirementsToCheck)
	    {
			var gate = new Gate.RequirementGate(gateRequirements);

			Assert.That(gate.Requires(requirementsToCheck), Is.True);
	    }

    }
}
