using System;
using System.Collections.Generic;
using System.Linq;

namespace Ice {
  namespace Extensions {

    public static class CollectionExtensions {
      private static Random rand = new Random();

      public static T RandomElement<T>(this T[] array) {
        return array[rand.Next(array.Length)];
      }

      public static T RandomElement<T>(this IList<T> list) {
        return list[rand.Next(list.Count)];
      }

      public static T RemoveRandomElement<T>(this IList<T> list) {
        var index = rand.Next(list.Count);
        var element = list[index];
        list.RemoveAt(index);
        return element;
      }

      public static IList<T> Shuffled<T>(this IList<T> list) {
        return list.OrderBy(_ => rand.Next()).ToList();
      }

      public static void EnqueueAll<T>(this Queue<T> queue, IEnumerable<T> enumerable) {
        foreach (var item in enumerable) {
          queue.Enqueue(item);
        }
      }
    }

  } // namespace Extensions
} // namespace Ice
