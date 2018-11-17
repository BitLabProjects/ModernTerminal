using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace ModernTerminal {
  public class ButtonDropDown : ContentControl {
    private bool mIsOpen;
    private bool mIsCapturing;

    public ButtonDropDown() {
      DefaultStyleKey = GetType();
      mIsOpen = false;
      mIsCapturing = false;
      VisualStateManager.GoToState(this, "Normal", false);
    }

    #region Dependency properties
    public object DropDownContent {
      get { return (object)GetValue(DropDownContentProperty); }
      set { SetValue(DropDownContentProperty, value); }
    }
    public static readonly DependencyProperty DropDownContentProperty =
        DependencyProperty.Register("DropDownContent", typeof(object), typeof(ButtonDropDown), new PropertyMetadata(null));

    public DataTemplate DropDownContentTemplate {
      get { return (DataTemplate)GetValue(DropDownContentTemplateProperty); }
      set { SetValue(DropDownContentTemplateProperty, value); }
    }
    public static readonly DependencyProperty DropDownContentTemplateProperty =
        DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(ButtonDropDown), new PropertyMetadata(null));
    #endregion

    #region Mouse handling
    protected override void OnMouseEnter(MouseEventArgs e) {
      if (!mIsOpen) {
        //System.Diagnostics.Debug.WriteLine("State: MouseOver");
        VisualStateManager.GoToState(this, "MouseOver", true);
      }
    }
    protected override void OnMouseLeave(MouseEventArgs e) {
      if (!mIsOpen) {
        //System.Diagnostics.Debug.WriteLine("State: Normal");
        VisualStateManager.GoToState(this, "Normal", true);
      }
    }
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
      if (mIsOpen) return;

      mIsOpen = !mIsOpen;
      //System.Diagnostics.Debug.WriteLine("State: Pressed");
      VisualStateManager.GoToState(this, "Pressed", true);
      mIsCapturing = CaptureMouse();
      e.Handled = true;
    }
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
      if (mIsCapturing) {
        if (mIsOpen) {
          //System.Diagnostics.Debug.WriteLine("State: Open");
          VisualStateManager.GoToState(this, "Open", true);
        } else {
          //System.Diagnostics.Debug.WriteLine("State: MouseOver");
          VisualStateManager.GoToState(this, "MouseOver", true);
        }
        ReleaseMouseCapture();
        mIsCapturing = false;
        e.Handled = true;
      }
    }
    #endregion

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      var popup = Template.FindName("Popup", this) as Popup;
      if (popup != null) {
        popup.Opened += Popup_Opened;
        popup.Closed += Popup_Closed;
      }

    }

    private void Popup_Closed(object sender, EventArgs e) {
      mIsOpen = false;
      //System.Diagnostics.Debug.WriteLine("State: Normal");
      VisualStateManager.GoToState(this, "Normal", true);
    }

    private void Popup_Opened(object sender, EventArgs e) {
    }

    /*
    private void EnableBlurForPopup(Popup popup) {
      FieldInfo field1 = typeof(Popup).GetField("_secHelper", BindingFlags.NonPublic | BindingFlags.Instance);
      var _secHelper = field1.GetValue(popup);
      if (_secHelper == null) return;

      FieldInfo field2 = _secHelper.GetType().GetField("_window", BindingFlags.NonPublic | BindingFlags.Instance);
      var _window = field2.GetValue(_secHelper);
      if (_window == null) return;

      FieldInfo field3 = _window.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
      var hwndSource = field3.GetValue(_window) as HwndSource;
      if (hwndSource == null) return;

      byte mBlurAlpha = 0xB0;
      byte mBlurGray = 0x00;
      Win32.EnableBlur(hwndSource, mBlurAlpha, mBlurGray);
    }
    */
  }
}
