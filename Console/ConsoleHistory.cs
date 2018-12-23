using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal {
  public class ConsoleHistory {
    private int mHistoryLineCurrIdx;
    public ObservableCollection<ConsoleHistoryLine> HistoryLines { get; set; }

    public ConsoleHistory() {
      HistoryLines = new ObservableCollection<ConsoleHistoryLine>();
    }

    public void AddToHistory(string text) {
      HistoryLines.Add(new ConsoleHistoryLine(text));
      mHistoryLineCurrIdx = HistoryLines.Count;
    }

    public string MovePrevious() {
      if (mHistoryLineCurrIdx > 0) {
        mHistoryLineCurrIdx -= 1;
      }
      return mGetHistoryLine();
    }
    public string MoveNext() {
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

  public class ConsoleHistoryLine {
    public string Text { get; }
    public ConsoleHistoryLine(string text) {
      this.Text = text;
    }
  }
}
