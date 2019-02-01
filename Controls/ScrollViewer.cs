using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernTerminal {
  public class ScrollViewer : System.Windows.Controls.ScrollViewer {
    private double mTotalVerticalOffset;
    private double mTotalHorizontalOffset;
    private bool mIsRunning;

    #region Orientation
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        "Orientation", typeof(Orientation), typeof(ScrollViewer), new PropertyMetadata(Orientation.Vertical));

    public Orientation Orientation {
      get => (Orientation)GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }
    #endregion

    protected override void OnMouseWheel(MouseWheelEventArgs e) {
      if (!EnableInertia) {
        base.OnMouseWheel(e);
      } else {
        e.Handled = true;
        if (Orientation == Orientation.Vertical) {
          if (!mIsRunning) {
            mTotalVerticalOffset = VerticalOffset;
            CurrentVerticalOffset = VerticalOffset;
          }
          mTotalVerticalOffset = Math.Min(Math.Max(0, mTotalVerticalOffset - e.Delta), ScrollableHeight);
          ScrollToVerticalOffsetInternal(mTotalVerticalOffset);
        } else {
          if (!mIsRunning) {
            mTotalHorizontalOffset = HorizontalOffset;
            CurrentHorizontalOffset = HorizontalOffset;
          }
          mTotalHorizontalOffset = Math.Min(Math.Max(0, mTotalHorizontalOffset - e.Delta), ScrollableWidth);
          ScrollToHorizontalOffsetInternal(mTotalHorizontalOffset);
        }
      }
    }

    internal void ScrollToVerticalOffsetInternal(double offset, double milliseconds = 500) {
      var animation = AnimationUtils.CreateAnimation(offset, milliseconds);
      animation.EasingFunction = new CubicEase {
        EasingMode = EasingMode.EaseOut
      };
      animation.FillBehavior = FillBehavior.Stop;
      animation.Completed += (s, e1) => {
        CurrentVerticalOffset = offset;
        mIsRunning = false;
      };
      mIsRunning = true;
      BeginAnimation(CurrentVerticalOffsetProperty, animation);
    }

    internal void ScrollToHorizontalOffsetInternal(double offset, double milliseconds = 500) {
      var animation = AnimationUtils.CreateAnimation(offset, milliseconds);
      animation.EasingFunction = new CubicEase {
        EasingMode = EasingMode.EaseOut
      };
      animation.FillBehavior = FillBehavior.Stop;
      animation.Completed += (s, e1) => {
        CurrentHorizontalOffset = offset;
        mIsRunning = false;
      };
      mIsRunning = true;
      BeginAnimation(CurrentHorizontalOffsetProperty, animation);
    }

    public bool EnableInertia {
      get;
      set;
    }

    private static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
        "CurrentVerticalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(default(double), OnCurrentVerticalOffsetChanged));

    private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is ScrollViewer ctl && e.NewValue is double v) {
        ctl.ScrollToVerticalOffset(v);
      }
    }

    private double CurrentVerticalOffset {
      // ReSharper disable once UnusedMember.Local
      get => (double)GetValue(CurrentVerticalOffsetProperty);
      set => SetValue(CurrentVerticalOffsetProperty, value);
    }

    public static readonly DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
        "CurrentHorizontalOffset", typeof(double), typeof(ScrollViewer), new PropertyMetadata(default(double), OnCurrentHorizontalOffsetChanged));

    private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is ScrollViewer ctl && e.NewValue is double v) {
        ctl.ScrollToHorizontalOffset(v);
      }
    }

    public double CurrentHorizontalOffset {
      get => (double)GetValue(CurrentHorizontalOffsetProperty);
      set => SetValue(CurrentHorizontalOffsetProperty, value);
    }
  }
}
