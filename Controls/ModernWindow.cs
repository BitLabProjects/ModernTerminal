using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernTerminal {
  public class ModernWindow : Window {
    static ModernWindow() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernWindow), new FrameworkPropertyMetadata(typeof(ModernWindow)));
    }

    public ModernWindow() {
      SetResourceReference(StyleProperty, typeof(ModernWindow));

      this.StateChanged += this_StateChanged;
      this.Loaded += this_Loaded;
    }

    private void this_Loaded(object sender, RoutedEventArgs e) {
      byte mBlurAlpha = 0xB0;
      byte mBlurGray = 0x00;
      Win32.EnableBlur(this, mBlurAlpha, mBlurGray);
    }

    public SolidColorBrush BackgroundColor { get; set; }

    Button mMinimize;
    Button mMaximize;
    Button mClose;
    Grid mRoot;
    FrameworkElement mDrag;
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      mMinimize = Template.FindName("Minimize", this) as Button;
      mMaximize = Template.FindName("Maximize", this) as Button;
      mClose = Template.FindName("Close", this) as Button;
      mRoot = Template.FindName("Root", this) as Grid;
      mDrag = Template.FindName("Drag", this) as FrameworkElement;

      mMinimize.Click += mMinimize_Click;
      mMaximize.Click += mMaximize_Click;
      mClose.Click += mClose_Click;
      mDrag.MouseDown += mDrag_MouseDown;
    }

    private void mMinimize_Click(object sender, RoutedEventArgs e) {
      WindowState = WindowState.Minimized;
    }
    private void mMaximize_Click(object sender, RoutedEventArgs e) {
      if (WindowState == WindowState.Maximized) {
        WindowState = WindowState.Normal;
      } else {
        WindowState = WindowState.Maximized;
      }
    }
    private void mClose_Click(object sender, RoutedEventArgs e) {
      Close();
    }
    private void mDrag_MouseDown(object sender, MouseButtonEventArgs e) {
      if (e.ChangedButton == MouseButton.Left)
        DragMove();
    }

    private void this_StateChanged(object sender, EventArgs e) {
      if (mRoot != null) {
        mRoot.Margin = new Thickness(WindowState == WindowState.Maximized ? 7 : 0);
      }
    }

    #region TitlebarDataTemplate
    public DataTemplate TitlebarDataTemplate {
      get { return (DataTemplate)GetValue(TitlebarDataTemplateProperty); }
      set { SetValue(TitlebarDataTemplateProperty, value); }
    }
    public static readonly DependencyProperty TitlebarDataTemplateProperty =
        DependencyProperty.Register("TitlebarDataTemplate", typeof(DataTemplate), typeof(ModernWindow), new PropertyMetadata(null));
    #endregion
  }
}
