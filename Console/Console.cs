using bitLab.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;

namespace ModernTerminal {
  public class Console : ViewModelBase, ILogListener {
    private int mLastLineNumber;
    private bool mLastLineIsOpen;

    public ObservableCollection<ConsoleLine> Lines { get; set; }
    public ConsoleHistory History { get; }

    private bool mSaveIsDirty;
    private DispatcherTimer mSaveTimer;

    public Console() {
      mLastLineNumber = 0;
      mLastLineIsOpen = false;

      Lines = new ObservableCollection<ConsoleLine>();
      History = new ConsoleHistory();

      mSaveIsDirty = false;
      mSaveTimer = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Background, mSaveTimer_Tick, Dispatcher);

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
      Dispatcher.Invoke(new Action(() => {
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
      History.AddToHistory(input);
      mSaveIsDirty = true;

      if (!Serial.IsConnected) {
        Log.LogError("Serial port not connected");
        return;
      }

      mLastLineIsOpen = false;
      ReceiveText(input);

      Serial.Send(input);
    }

    #region Configuration Load-Save
    private void mSaveTimer_Tick(object sender, EventArgs e) {
      if (mSaveIsDirty) {
        mSaveIsDirty = false;
        SaveConfig();
      }
    }
    const string configFileName = "config.xml";
    public void LoadConfig() {
      if (!System.IO.File.Exists(configFileName)) {
        return;
      }

      try {
        using (var file = System.IO.File.OpenRead(configFileName)) {
          var xDoc = XDocument.Load(file);
          History.LoadConfig(xDoc);
        }
      } catch (Exception ex) {
        Log.LogError($"Could not restore configuration from {configFileName}: {ex.Message}");
      }
    }

    public void SaveConfig() {
      try {
        // Create a backup copy of the configuration
        const string configFileNameTemp = configFileName + ".temp";

        var xDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("ModernTerminal",
                      History.ToXElement()
                    )
                );

        xDoc.Save(configFileNameTemp, SaveOptions.None);

        // Delete the backup file: no exception encountered
        System.IO.File.Delete(configFileName);
        System.IO.File.Move(configFileNameTemp, configFileName);

      } catch (Exception ex) {
        Log.LogError($"Could not save configuration to {configFileName}: {ex.Message}");
      }
    }
    #endregion
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
