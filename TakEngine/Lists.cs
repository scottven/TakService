using System;
using System.Collections.Generic;

namespace TakEngine
{
    /// <summary>
    /// Utility methods for manipulating lists
    /// </summary>
    public static class ListExtensions
    {
        public static void RandomizeOrder<T>(this IList<T> list, Random rand)
        {
            int listCount = list.Count;

            // Start randomizing at the beginning of the list
            for (int i = 0; i < listCount - 1; i++)
            {
                // Get the element which currently occupies this position
                T currentElement = list[i];

                // Pick a random element to put into this position
				int choose = rand.Next(listCount - i) + i;

                // Swap the random element into this position
                list[i] = list[choose];
                list[choose] = currentElement;
            }
        }

        public static T RemoveLast<T>(this IList<T> list)
        {
            var lastIndex = list.Count - 1;
            var removing = list[lastIndex];
            list.RemoveAt(lastIndex);
            return removing;
        }
	}
}
