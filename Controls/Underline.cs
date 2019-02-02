using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernTerminal
{
  class Underline: Control
  {
    public Underline() {
      DefaultStyleKey = typeof(Underline);
    }

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      UpdateVisualState();
    }

    private void UpdateVisualState() {
      var newState = IsActive ? "Active" : "Inactive";
      VisualStateManager.GoToState(this, newState, true);
    }

    public bool IsActive {
      get { return (bool)GetValue(IsActiveProperty); }
      set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(Underline), new PropertyMetadata(false, IsActive_PropertyChanged));

    private static void IsActive_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var u = (Underline)d;
      u.UpdateVisualState();
    }
  }
}
