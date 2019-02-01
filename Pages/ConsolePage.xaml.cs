using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernTerminal {
  public partial class ConsolePage : UserControl {
    public ConsolePage() {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
      icLines.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;

      var pagesView = Helpers.GetParentOfType<PagesView>(this);
      if (pagesView != null) {
        var argument = pagesView.GetArgument();
        if (argument != null) {
          tbInput.Text = argument;
          tbInput.CaretIndex = tbInput.Text.Length;
        }
      }

      tbInput.Focus();
    }

    private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e) {
      svLines.ScrollToEnd();
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
      var textBox = sender as TextBox;
      var console = DataContext as Console;
      if (e.Key == Key.Return) {
        console.SendText((sender as TextBox).Text);
        textBox.Text = "";
        e.Handled = true;

      } else if (e.Key == Key.Up) {
        var previousEntry = console.History.MovePrevious();
        if (previousEntry != null) {
          textBox.Text = previousEntry;
          textBox.CaretIndex = textBox.Text.Length;
        }
        e.Handled = true;

      } else if (e.Key == Key.Down) {
        var nextEntry = console.History.MoveNext();
        if (nextEntry != null) {
          textBox.Text = nextEntry;
          textBox.CaretIndex = textBox.Text.Length;
        }
        e.Handled = true;

      }
    }
  }
}
