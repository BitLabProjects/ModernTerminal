using bitLab.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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
  public partial class MainWindow {
    // Inspiration
    // https://www.hanselman.com/blog/TerminusAndFluentTerminalAreTheStartOfAWorldOf3rdPartyOSSConsoleReplacementsForWindows.aspx
    // https://github.com/ConfusedHorse/BlurryControls

    public MainWindow() {
      InitializeComponent();

      Console = new Console();
      Console.LoadConfig();
      DataContext = this;
    }

    public Console Console { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      Console.Serial.Autoconnect();
    }

    private void btnConnect_Click(object sender, RoutedEventArgs e) {
      Console.Serial.ConnectOrDisconnect();
    }
  }
}
