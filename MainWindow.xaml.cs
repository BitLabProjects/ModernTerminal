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
  public partial class MainWindow : Window, INotifyPropertyChanged {
    // Inspiration
    // https://www.hanselman.com/blog/TerminusAndFluentTerminalAreTheStartOfAWorldOf3rdPartyOSSConsoleReplacementsForWindows.aspx
    // https://github.com/ConfusedHorse/BlurryControls

    public MainWindow() {
      InitializeComponent();
      this.StateChanged += this_StateChanged;

      mBlurAlpha = 0xB0;
      mBlurGray = 0x00;
      mBackgroundAlpha = 0xff;
      mBackgroundGray = 0x10;

      Console = new Console();
      Console.LoadConfig();
      DataContext = this;
    }

    public Console Console { get; set; }

    private void this_StateChanged(object sender, EventArgs e) {
      if (grRoot != null) {
        grRoot.Margin = new Thickness(WindowState == WindowState.Maximized ? 7 : 0);
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      update();

      Console.Serial.Autoconnect();
    }

    Button btnMinimize;
    Button btnMaximize;
    Button btnClose;
    Grid grRoot;
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      btnMinimize = Template.FindName("btnMinimize", this) as Button;
      btnMaximize = Template.FindName("btnMaximize", this) as Button;
      btnClose = Template.FindName("btnClose", this) as Button;
      grRoot = Template.FindName("grRoot", this) as Grid;

      btnMinimize.Click += btnMinimize_Click;
      btnMaximize.Click += btnMaximize_Click;
      btnClose.Click += btnClose_Click;
    }

    private void btnMinimize_Click(object sender, RoutedEventArgs e) {
      WindowState = WindowState.Minimized;
    }
    private void btnMaximize_Click(object sender, RoutedEventArgs e) {
      if (WindowState == WindowState.Maximized) {
        WindowState = WindowState.Normal;
      } else {
        WindowState = WindowState.Maximized;
      }
    }
    private void btnClose_Click(object sender, RoutedEventArgs e) {
      Close();
    }

    private byte mBlurAlpha;
    public byte BlurAlpha { get { return mBlurAlpha; } set { mBlurAlpha = value; update(); } }
    private byte mBlurGray;
    public byte BlurGray { get { return mBlurGray; } set { mBlurGray = value; update(); } }

    private byte mBackgroundAlpha;
    public byte BackgroundAlpha { get { return mBackgroundAlpha; } set { mBackgroundAlpha = value; update(); } }
    private byte mBackgroundGray;
    public byte BackgroundGray { get { return mBackgroundGray; } set { mBackgroundGray = value; update(); } }
    public SolidColorBrush BackgroundColor { get; set; }

    private void update() {
      Win32.EnableBlur(this, mBlurAlpha, mBlurGray);
      BackgroundColor = new SolidColorBrush(Color.FromArgb(BackgroundAlpha, BackgroundGray, BackgroundGray, BackgroundGray));
      Notify(nameof(BackgroundColor));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string propName) {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
      }
    }

    private void btnConnect_Click(object sender, RoutedEventArgs e) {
      Console.Serial.ConnectOrDisconnect();
    }

    private void DragBorder_MouseDown(object sender, MouseButtonEventArgs e) {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }
  }
}
