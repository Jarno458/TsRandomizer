using System.Linq;
using Microsoft.Xna.Framework;

namespace TsRanodmizer.Extensions
{
    public static class GameComponentCollectionExtensions
    {
        public static void ReplaceComponent<T>(
            this GameComponentCollection components, T toBeReplaced, T replacement
        ) where T : IGameComponent
        {
            components.Remove(toBeReplaced);
            components.Add(replacement);
        }

        public static T FirstOfType<T>(this GameComponentCollection components) where T : IGameComponent
        {
            return (T)components.First(c => c.GetType() == typeof(T));
        }
    }
}
