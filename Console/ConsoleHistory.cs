using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModernTerminal {
  public class ConsoleHistory {
    private int mHistoryLineCurrIdx;
    public ObservableCollection<ConsoleHistoryLine> HistoryLines { get; set; }

    public ConsoleHistory() {
      HistoryLines = new ObservableCollection<ConsoleHistoryLine>();
    }

    public void AddToHistory(string text) {
      HistoryLines.Add(new ConsoleHistoryLine(text, DateTime.Now));
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
            HistoryLines.Add(new ConsoleHistoryLine(xAttrText.Value, date));
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
    public string Text { get; }
    public DateTime Date { get; }
    public ConsoleHistoryLine(string text, DateTime date) {
      this.Text = text;
      this.Date = date;
    }
  }
}
