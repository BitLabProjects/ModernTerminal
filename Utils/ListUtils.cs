using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal {
  class ListUtils {
    public static Int32 BinarySearch<T>(IList<T> inputArray, 
                                        T item, 
                                        Comparison<T> comparison) {
      int min = 0;
      int max = inputArray.Count - 1;
      while (min <= max) {
        int mid = (min + max) / 2;
        var comp = comparison(item, inputArray[mid]);
        if (comp == 0) {
          return ++mid;
        } else if (comp < 0) {
          max = mid - 1;
        } else {
          min = mid + 1;
        }
      }
      return min;
    }
  }
}
