using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModernTerminal {
  class Helpers {
    public static T GetParentOfType<T>(DependencyObject source) where T : class {
      var curr = source;
      while (curr != null) {
        curr = VisualTreeHelper.GetParent(curr);
        var currAsT = curr as T;
        if (currAsT != null) {
          return currAsT;
        }
      }
      return null;
    }
  }
}
