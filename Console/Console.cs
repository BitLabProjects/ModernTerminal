using bitLab.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernTerminal {
  public class Console: ViewModelBase, ILogListener {
    private int mLastLineNumber;
    private bool mLastLineIsOpen;

    private Dispatcher mDispatcher;
    public ObservableCollection<ConsoleLine> Lines { get; set; }

    public Console(Dispatcher dispatcher) {
      mDispatcher = dispatcher;
      Lines = new ObservableCollection<ConsoleLine>();
      mLastLineNumber = 0;
      mLastLineIsOpen = false;
      Serial = new Serial(this.mSerial_DataReadCallback);

      Log.Register(this);
    }

    public Serial Serial { get; }

    private void mSerial_DataReadCallback(string data) {
      var idxOfNewline = data.IndexOf('\n');
      if (idxOfNewline < 0) {
        // No newline in the input data: append the data considering it an incomplete line
        ReceiveText(data, isCompleteLine: false);
        return;
      }

      var idxStart = 0;
      while (idxOfNewline >= 0) {
        ReceiveText(data.Substring(idxStart, idxOfNewline - idxStart), isCompleteLine: true);
        idxStart = idxOfNewline + 1;
        idxOfNewline = data.IndexOf('\n', idxStart);
      }

      if (idxStart < data.Length - 1) {
        ReceiveText(data.Substring(idxStart), isCompleteLine: false);
      }
    }

    public void LogMessage(LogMessage message) {
      mLastLineIsOpen = false;

      ConsoleLineGlyph glyph;
      switch (message.Type) {
        case ELogMessageType.Info: glyph = ConsoleLineGlyph.Info; break;
        case ELogMessageType.Warning: glyph = ConsoleLineGlyph.Warning; break;
        case ELogMessageType.Error: glyph = ConsoleLineGlyph.Error; break;
        default: glyph = ConsoleLineGlyph.None; break;
      }

      ReceiveText(message.Message, glyph);
    }

    public void ReceiveText(string input, ConsoleLineGlyph glyph = ConsoleLineGlyph.None, bool isCompleteLine = true) {
      mDispatcher.Invoke(new Action(() => {
        if (mLastLineIsOpen) {
          var l = Lines[Lines.Count - 1];
          Lines[Lines.Count - 1] = new ConsoleLine(l.LineNumber, l.Text + input, l.Glyph);
        } else {
          mLastLineNumber += 1;
          Lines.Add(new ConsoleLine(mLastLineNumber, input, glyph));
        }
        mLastLineIsOpen = !isCompleteLine;
      }));
    }

    public void SendText(string input) {
      mLastLineIsOpen = false;

      if (!Serial.IsConnected) {
        ReceiveText("Error: Serial port not connected");
        return;
      }

      ReceiveText(input);

      Serial.Send(input);
    }
  }

  public enum ConsoleLineGlyph {
    None,
    Info,
    Warning,
    Error
  }

  public class ConsoleLine {
    public ConsoleLineGlyph Glyph { get; }
    public int LineNumber { get; }
    public DateTime Date { get; }
    public string Text { get; set; }

    public ConsoleLine(int lineNumber, string text, ConsoleLineGlyph glyph) {
      this.Date = DateTime.Now;
      this.LineNumber = lineNumber;
      this.Text = text;
      this.Glyph = glyph;
    }
  }
}
