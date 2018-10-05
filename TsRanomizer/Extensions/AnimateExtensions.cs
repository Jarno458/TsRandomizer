using Timespinner.GameObjects.BaseClasses;

namespace TsRanodmizer.Extensions
{
	public static class AnimateExtensions
	{
		public static void ChangeAnimation(this Animate animate, int id)
		{
			animate.ChangeAnimation(new AnimationSpec
			{
				Start = id,
				Length = 1,
				Speed = 1,
				Type = EAnimationType.None,
				InitialIndex = 0
			});
		}
	}
}
