using System.Reflection;

namespace TsRandomizer.Extensions
{
	static class MethodInfoExtensions
	{
		internal static object InvokeStatic(this MethodInfo methodInfo, params object[] arguments)
			=> methodInfo.Invoke(null, arguments);
	}
}
