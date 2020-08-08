using System.Reflection;

namespace TsRandomizer.Extensions
{
	static class MethodInfoExtensions
	{
		internal static object InvokeStatic(this MethodInfo methodInfo, params object[] arguments)
		{
			return methodInfo.Invoke(null, arguments);
		}
	}
}
