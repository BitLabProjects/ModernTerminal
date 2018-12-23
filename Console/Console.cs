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
    private int mHistoryLineCurrIdx;
    public ObservableCollection<ConsoleHistoryLine> HistoryLines { get; set; }

    public Console(Dispatcher dispatcher) {
      mDispatcher = dispatcher;
      Lines = new ObservableCollection<ConsoleLine>();
      HistoryLines = new ObservableCollection<ConsoleHistoryLine>();
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
      mDispatcher.Invoke(() => {
        HistoryLines.Add(new ConsoleHistoryLine(input));
        mHistoryLineCurrIdx = HistoryLines.Count;
      });

      if (!Serial.IsConnected) {
        Log.LogError("Serial port not connected");
        return;
      }

      mLastLineIsOpen = false;
      ReceiveText(input);

      Serial.Send(input);
    }

    public string HistoryMovePrevious() {
      if (mHistoryLineCurrIdx > 0) {
        mHistoryLineCurrIdx -= 1;
      }
      return mGetHistoryLine();
    }
    public string HistoryMoveNext() {
      if (mHistoryLineCurrIdx < HistoryLines.Count) {
        mHistoryLineCurrIdx += 1;
      }
      return mGetHistoryLine();
    }
    private string mGetHistoryLine() {
      if (mHistoryLineCurrIdx < 0 || mHistoryLineCurrIdx >= HistoryLines.Count) return "";
      return HistoryLines[mHistoryLineCurrIdx].Text;
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

  public class ConsoleHistoryLine {
    public string Text { get; }
    public ConsoleHistoryLine(string text) {
      this.Text = text;
    }
  }
}
