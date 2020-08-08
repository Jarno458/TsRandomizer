using System.Linq;
using Microsoft.Xna.Framework;

namespace TsRandomizer.Extensions
{
    public static class GameComponentCollectionExtensions
    {
	    internal static void ReplaceComponent<T>(
            this GameComponentCollection components, T toBeReplaced, T replacement
        ) where T : IGameComponent
        {
            components.Remove(toBeReplaced);
            components.Add(replacement);
        }

	    internal static T FirstOfType<T>(this GameComponentCollection components) where T : IGameComponent
        {
            return (T)components.First(c => c.GetType() == typeof(T));
        }
    }
}
