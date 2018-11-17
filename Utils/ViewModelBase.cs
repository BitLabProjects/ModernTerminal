using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal {
  public class ViewModelBase: INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void Notify(string propName) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }

    protected void SetAndNotify<T>(ref T oldValue, T newValue, [CallerMemberName()] string propertyName = null) {
      if (!Object.Equals(oldValue, newValue)) {
        oldValue = newValue;
        Notify(propertyName);
      }
    }
  }
}
