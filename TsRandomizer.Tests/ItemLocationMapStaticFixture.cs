using System;
using System.Linq.Expressions;
using NUnit.Framework;
using TsRandomizer.Randomisation;
using R = TsRandomizer.Randomisation.Requirement;
using static TsRandomizer.Randomisation.ItemLocationMap;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemLocationMapStaticFixture
	{
		static readonly TC[] TestCases = {
			new TC(() => DoubleJumpOfNpc, R.DoubleJump, false),
			new TC(() => DoubleJumpOfNpc, R.TimeStop, false),
			new TC(() => DoubleJumpOfNpc, R.ForwardDash, false), 
			new TC(() => DoubleJumpOfNpc, R.DoubleJump | R.TimeStop, true),
			new TC(() => DoubleJumpOfNpc, R.UpwardDash, true),

			new TC(() => LowerLakeDesolationBridge, R.DoubleJump, true),
			new TC(() => LowerLakeDesolationBridge, R.ForwardDash, true),
			new TC(() => LowerLakeDesolationBridge, R.TimeStop, true),
			new TC(() => LowerLakeDesolationBridge, R.GateKittyBoss, true),
			new TC(() => LowerLakeDesolationBridge, R.GateLeftLibrary, true),
			new TC(() => LowerLakeDesolationBridge, R.UpwardDash, false),

			new TC(() => AccessToPast, R.DoubleJump | R.TimeStop | R.TimespinnerSpindle | R.CardD, true),
			new TC(() => AccessToPast, R.GateLakeSirineRight, true),
			new TC(() => AccessToPast, R.GateLakeSirineLeft, true),
			new TC(() => AccessToPast, R.GateAccessToPast, true),
			new TC(() => AccessToPast, R.GateKittyBoss, false),
			new TC(() => AccessToPast, R.GateLeftLibrary, false),

			new TC(() => LeftSideForestCaves, R.GateLakeSirineRight, true),
			new TC(() => LeftSideForestCaves, R.GateLakeSirineLeft, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast, false),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.TimeStop, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.ForwardDash, true),
			new TC(() => LeftSideForestCaves, R.GateKittyBoss, false),
			new TC(() => LeftSideForestCaves, R.GateLeftLibrary, false),

			new TC(() => UpperLakeSirine, R.GateLakeSirineRight, false),
			new TC(() => UpperLakeSirine, R.GateLakeSirineRight | R.TimeStop, true),
			new TC(() => UpperLakeSirine, R.GateLakeSirineRight | R.DoubleJump, true),
			new TC(() => UpperLakeSirine, R.GateLakeSirineRight | R.ForwardDash, true),
			new TC(() => UpperLakeSirine, R.GateLakeSirineLeft, true),
			new TC(() => UpperLakeSirine, R.GateAccessToPast, false),
			new TC(() => UpperLakeSirine, R.GateAccessToPast | R.TimeStop, true),
			new TC(() => UpperLakeSirine, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => UpperLakeSirine, R.GateAccessToPast | R.ForwardDash, true),
			new TC(() => UpperLakeSirine, R.GateKittyBoss, false),
			new TC(() => UpperLakeSirine, R.GateLeftLibrary, false),

			new TC(() => LowerlakeSirine, R.GateLakeSirineRight, false),
			new TC(() => LowerlakeSirine, R.GateLakeSirineRight | R.TimeStop, false),
			new TC(() => LowerlakeSirine, R.GateLakeSirineRight | R.DoubleJump, false),
			new TC(() => LowerlakeSirine, R.GateLakeSirineRight | R.ForwardDash, false),
			new TC(() => LowerlakeSirine, R.GateLakeSirineRight | R.Swimming, true),
			new TC(() => LowerlakeSirine, R.GateLakeSirineLeft, false),
			new TC(() => LowerlakeSirine, R.GateLakeSirineLeft | R.Swimming, true),
			new TC(() => LowerlakeSirine, R.GateAccessToPast, false),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.TimeStop, false),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.DoubleJump, false),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.ForwardDash, false),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.Swimming, false),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.TimeStop | R.Swimming, true),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.DoubleJump | R.Swimming, true),
			new TC(() => LowerlakeSirine, R.GateAccessToPast | R.ForwardDash | R.Swimming, true),
			new TC(() => LowerlakeSirine, R.GateKittyBoss, false),
			new TC(() => LowerlakeSirine, R.GateLeftLibrary, false),

			new TC(() => RoyalTower, R.DoubleJump | R.TimeStop | R.TimespinnerSpindle | R.CardD, true),
			new TC(() => RoyalTower, R.GateLakeSirineRight, false),
			new TC(() => RoyalTower, R.GateLakeSirineRight | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateLakeSirineRight | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateLakeSirineLeft, false),
			new TC(() => RoyalTower, R.GateLakeSirineLeft | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateLakeSirineLeft | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateAccessToPast, false),
			new TC(() => RoyalTower, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateAccessToPast | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateKittyBoss, false),
			new TC(() => RoyalTower, R.GateLeftLibrary, false),

			new TC(() => UpperRoyalTower, R.DoubleJump | R.TimeStop | R.TimespinnerSpindle | R.CardD, true),
			new TC(() => UpperRoyalTower, R.GateLakeSirineRight | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateLakeSirineRight | R.DoubleJump | R.TimeStop, true),
			new TC(() => UpperRoyalTower, R.GateLakeSirineRight | R.UpwardDash, true),
			new TC(() => UpperRoyalTower, R.GateLakeSirineLeft | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateLakeSirineLeft | R.DoubleJump | R.TimeStop, true), 
			new TC(() => UpperRoyalTower, R.GateLakeSirineLeft | R.UpwardDash, true),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.DoubleJump | R.TimeStop, true),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.UpwardDash, true),

			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.TimeStop, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.TimeStop | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.DoubleJump, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.DoubleJump | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.ForwardDash, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineRight | R.ForwardDash | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineLeft, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSirineLeft | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.TimeStop, false),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.TimeStop | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.DoubleJump, false),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.DoubleJump | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.ForwardDash, false),
			new TC(() => UpperLakeDesolation, R.GateAccessToPast | R.ForwardDash | R.AntiWeed, true),

		};

		[TestCaseSource(nameof(TestCases))]
		public void TestIt(TC testcase)
		{
			Assert.That(testcase.Gate.CanBeOpenedWith(testcase.Requirements), Is.EqualTo(testcase.ShouldOpen));
		}

		// ReSharper disable once InconsistentNaming
		public class TC
		{
			readonly string name;
			public Gate Gate { get; }
			public R Requirements { get; }
			public bool ShouldOpen { get; }

			public TC(Expression<Func<Gate>> gateFunc, R requirements, bool shouldOpen)
			{
				Requirements = requirements;
				ShouldOpen = shouldOpen;

				var expression = (MemberExpression)gateFunc.Body;

				name = expression.Member.Name;
				Gate = gateFunc.Compile()();
			}

			public TC(Expression<Func<R>> gateFunc, R requirements, bool shouldOpen)
			{
				Requirements = requirements;
				ShouldOpen = shouldOpen;

				var expression = (MemberExpression)gateFunc.Body;

				name = expression.Member.Name;
				Gate = new Gate.RequirementGate(gateFunc.Compile()());
			}

			public override string ToString()
			{
				return $"{name}, {Requirements}, {ShouldOpen}";
			}
		}
	}
}
