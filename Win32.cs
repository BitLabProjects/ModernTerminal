using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ModernTerminal {
  class Win32 {
    [DllImport("user32.dll", SetLastError =true)]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    [DllImport("kernel32.dll")]
    static extern uint GetLastError();

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData {
      public WindowCompositionAttribute Attribute;
      public IntPtr Data;
      public int SizeOfData;
    }

    internal enum WindowCompositionAttribute {
      // ...
      WCA_ACCENT_POLICY = 19
      // ...
    }

    internal enum AccentState {
      ACCENT_DISABLED = 0,
      ACCENT_ENABLE_GRADIENT = 1,
      ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
      ACCENT_ENABLE_BLURBEHIND = 3,
      ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy {
      public AccentState AccentState;
      public int AccentFlags;
      public int GradientColor;
      public int AnimationId;
    }

    internal static void EnableBlur(Window window, byte alpha, byte gray) {
      var windowHelper = new WindowInteropHelper(window);
      EnableBlur(windowHelper.Handle, alpha, gray);
    }
    internal static void EnableBlur(HwndSource hwndSource, byte alpha, byte gray) {
      EnableBlur(hwndSource.Handle, alpha, gray);
    }

    private static void EnableBlur(IntPtr handle, byte alpha, byte gray) {
      var accent = new AccentPolicy();
      var accentStructSize = Marshal.SizeOf(accent);
      //accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
      accent.AccentState = AccentState.ACCENT_INVALID_STATE;
      //accent.GradientColor = 0x7F000000;
      accent.GradientColor = (alpha << 24) | (gray << 16) | (gray << 8) | gray;

      var accentPtr = Marshal.AllocHGlobal(accentStructSize);
      Marshal.StructureToPtr(accent, accentPtr, false);

      var data = new WindowCompositionAttributeData();
      data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
      data.SizeOfData = accentStructSize;
      data.Data = accentPtr;

      var res = SetWindowCompositionAttribute(handle, ref data);

      Marshal.FreeHGlobal(accentPtr);
    }
  }
}
