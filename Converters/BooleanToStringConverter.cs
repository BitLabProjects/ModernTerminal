using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModernTerminal {
  public class BooleanToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var paramAsString = parameter as string;
      if (paramAsString == null)
        return "";

      if ((bool)value) {
        return paramAsString.Split('|')[0];
      } else {
        return paramAsString.Split('|')[1];
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
