using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ModernTerminal {
  public class ConsoleHistory {
    public readonly Console Console;

    private int mHistoryLineCurrIdx;
    public ObservableCollection<ConsoleHistoryLine> HistoryLines { get; set; }
    public Grouping<string, ConsoleHistoryLine> HistoryLinesGroupedByDate { get; }

    public ConsoleHistory(Console console) {
      this.Console = console;
      HistoryLines = new ObservableCollection<ConsoleHistoryLine>();
      HistoryLinesGroupedByDate = new Grouping<string, ConsoleHistoryLine>(HistoryLines, 
                                                                           (hl) => GetGroupNameFromDate(hl.Date),
                                                                           CompareGroupNames);
    }

    private string GetGroupNameFromDate(DateTime date) {
      if (date >= DateTime.Today) return "Today";
      if (date >= DateTime.Today.AddDays(-7)) return "Last week";
      if (date >= DateTime.Today.AddMonths(-1)) return "Last month";
      return "Older than one month";
    }

    private int CompareGroupNames(string x, string y) {
      return GetGroupNameSortNumber(x).CompareTo(GetGroupNameSortNumber(y));
    }
    private int GetGroupNameSortNumber(string x) {
      switch(x) {
        case "Today": return 0;
        case "Last week": return 1;
        case "Last month": return 2;
        default: return 3;
      }
    }

    public void AddToHistory(string text) {
      HistoryLines.Add(new ConsoleHistoryLine(this, text, DateTime.Now));
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

    public void LoadConfig(XDocument xDoc) {
      HistoryLines.Clear();

      var xHistory = xDoc.Descendants(XName.Get("History")).FirstOrDefault();
      if (xHistory != null) {
        foreach (var xHistoryLine in xHistory.Elements(XName.Get("HistoryLine"))) {
          var xAttrText = xHistoryLine.Attribute(XName.Get("Text"));
          var xAttrDate = xHistoryLine.Attribute(XName.Get("Date"));
          DateTime date;
          if (xAttrText != null &&
              xAttrDate != null &&
              DateTime.TryParse(xAttrDate.Value, out date)) {
            HistoryLines.Add(new ConsoleHistoryLine(this, xAttrText.Value, date));
          }
        }
      }

      mHistoryLineCurrIdx = HistoryLines.Count;
    }

    public XElement ToXElement() {
      var result = new XElement("History");

      foreach (var h in HistoryLines) {
        result.Add(new XElement(XName.Get("HistoryLine"),
                                new XAttribute(XName.Get("Text"), h.Text),
                                new XAttribute(XName.Get("Date"), h.Date.ToString("O"))
                                ));
      }

      return result;
    }
  }

  public class ConsoleHistoryLine {
    private readonly ConsoleHistory mConsoleHistory;
    public string Text { get; }
    public DateTime Date { get; }
    public ConsoleHistoryLine(ConsoleHistory consoleHistory, string text, DateTime date) {
      mConsoleHistory = consoleHistory;
      this.Text = text;
      this.Date = date;
      //CopyToConsoleCommand = new DelegateCommand(() => {
      //  mConsoleHistory.Console.
      //});
    }

    //public ICommand CopyToConsoleCommand { get; }
  }
}
