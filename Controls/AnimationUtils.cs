using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernTerminal {
  public class AnimationUtils {
    public static ThicknessAnimation CreateAnimation(Thickness thickness = default(Thickness), double milliseconds = 200) {
      return new ThicknessAnimation(thickness, new Duration(TimeSpan.FromMilliseconds(milliseconds)));
    }
    public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200) {
      return new DoubleAnimation(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)));
    }
    public static DoubleAnimation CreateAnimation(double fromValue, double toValue, double milliseconds = 200) {
      return new DoubleAnimation(fromValue, toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)));
    }
  }
}
