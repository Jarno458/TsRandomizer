using System;
using System.Linq.Expressions;
using NUnit.Framework;
using TsRandomizer.Randomisation;
using R = TsRandomizer.Randomisation.Requirement;

namespace TsRandomizer.Tests
{
	[TestFixture]
	class ItemLocationMapStaticFixture
	{
		/*static readonly TC[] TestCases = {
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
			new TC(() => AccessToPast, R.GateLakeSereneRight, true),
			new TC(() => AccessToPast, R.GateLakeSereneLeft, true),
			new TC(() => AccessToPast, R.GateAccessToPast, true),
			new TC(() => AccessToPast, R.GateKittyBoss, false),
			new TC(() => AccessToPast, R.GateLeftLibrary, false),

			new TC(() => LeftSideForestCaves, R.GateLakeSereneRight, true),
			new TC(() => LeftSideForestCaves, R.GateLakeSereneLeft, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast, false),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.TimeStop, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => LeftSideForestCaves, R.GateAccessToPast | R.ForwardDash, true),
			new TC(() => LeftSideForestCaves, R.GateKittyBoss, false),
			new TC(() => LeftSideForestCaves, R.GateLeftLibrary, false),

			new TC(() => UpperLakeSerene, R.GateLakeSereneRight, false),
			new TC(() => UpperLakeSerene, R.GateLakeSereneRight | R.TimeStop, true),
			new TC(() => UpperLakeSerene, R.GateLakeSereneRight | R.DoubleJump, true),
			new TC(() => UpperLakeSerene, R.GateLakeSereneRight | R.ForwardDash, true),
			new TC(() => UpperLakeSerene, R.GateLakeSereneLeft, true),
			new TC(() => UpperLakeSerene, R.GateAccessToPast, false),
			new TC(() => UpperLakeSerene, R.GateAccessToPast | R.TimeStop, true),
			new TC(() => UpperLakeSerene, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => UpperLakeSerene, R.GateAccessToPast | R.ForwardDash, true),
			new TC(() => UpperLakeSerene, R.GateKittyBoss, false),
			new TC(() => UpperLakeSerene, R.GateLeftLibrary, false),

			new TC(() => LowerlakeSerene, R.GateLakeSereneRight, false),
			new TC(() => LowerlakeSerene, R.GateLakeSereneRight | R.TimeStop, false),
			new TC(() => LowerlakeSerene, R.GateLakeSereneRight | R.DoubleJump, false),
			new TC(() => LowerlakeSerene, R.GateLakeSereneRight | R.ForwardDash, false),
			new TC(() => LowerlakeSerene, R.GateLakeSereneRight | R.Swimming, true),
			new TC(() => LowerlakeSerene, R.GateLakeSereneLeft, false),
			new TC(() => LowerlakeSerene, R.GateLakeSereneLeft | R.Swimming, true),
			new TC(() => LowerlakeSerene, R.GateAccessToPast, false),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.TimeStop, false),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.DoubleJump, false),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.ForwardDash, false),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.Swimming, false),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.TimeStop | R.Swimming, true),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.DoubleJump | R.Swimming, true),
			new TC(() => LowerlakeSerene, R.GateAccessToPast | R.ForwardDash | R.Swimming, true),
			new TC(() => LowerlakeSerene, R.GateKittyBoss, false),
			new TC(() => LowerlakeSerene, R.GateLeftLibrary, false),

			new TC(() => RoyalTower, R.DoubleJump | R.TimeStop | R.TimespinnerSpindle | R.CardD, true),
			new TC(() => RoyalTower, R.GateLakeSereneRight, false),
			new TC(() => RoyalTower, R.GateLakeSereneRight | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateLakeSereneRight | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateLakeSereneLeft, false),
			new TC(() => RoyalTower, R.GateLakeSereneLeft | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateLakeSereneLeft | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateAccessToPast, false),
			new TC(() => RoyalTower, R.GateAccessToPast | R.DoubleJump, true),
			new TC(() => RoyalTower, R.GateAccessToPast | R.UpwardDash, true),
			new TC(() => RoyalTower, R.GateKittyBoss, false),
			new TC(() => RoyalTower, R.GateLeftLibrary, false),

			new TC(() => UpperRoyalTower, R.DoubleJump | R.TimeStop | R.TimespinnerSpindle | R.CardD, true),
			new TC(() => UpperRoyalTower, R.GateLakeSereneRight | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateLakeSereneRight | R.DoubleJump | R.TimeStop, true),
			new TC(() => UpperRoyalTower, R.GateLakeSereneRight | R.UpwardDash, true),
			new TC(() => UpperRoyalTower, R.GateLakeSereneLeft | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateLakeSereneLeft | R.DoubleJump | R.TimeStop, true), 
			new TC(() => UpperRoyalTower, R.GateLakeSereneLeft | R.UpwardDash, true),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.DoubleJump, false),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.DoubleJump | R.TimeStop, true),
			new TC(() => UpperRoyalTower, R.GateAccessToPast | R.UpwardDash, true),

			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.TimeStop, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.TimeStop | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.DoubleJump, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.DoubleJump | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.ForwardDash, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneRight | R.ForwardDash | R.AntiWeed, true),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneLeft, false),
			new TC(() => UpperLakeDesolation, R.GateLakeSereneLeft | R.AntiWeed, true),
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
		}*/
	}
}
