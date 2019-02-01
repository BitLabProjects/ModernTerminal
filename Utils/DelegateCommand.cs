using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernTerminal {
  class DelegateCommand : ICommand {
    public event EventHandler CanExecuteChanged;

    private readonly Action mAction;
    public DelegateCommand(Action action) {
      mAction = action;
    }

    public bool CanExecute(object parameter) {
      return true;
    }

    public void Execute(object parameter) {
      mAction();
    }
  }
}
