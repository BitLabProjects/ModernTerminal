using bitLab.Log;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal {
  public class Serial : ViewModelBase {
    private string mName;
    private Int32 mBaudRate;
    private Int32 mDataBits;
    private StopBits mStopBits;
    private Parity mParity;
    private List<string> mAvailableNames;
    private List<Int32> mAvailableBaudRates;
    private List<StopBits> mAvailableStopBits;
    private List<Parity> mAvailableParity;
    private SerialPort port;
    private Action<string> dataReadCallback;

    public Serial(Action<string> dataReadCallback) {
      mName = "COM1";
      mBaudRate = 115200;
      mDataBits = 8;
      mStopBits = StopBits.One;
      mParity = Parity.None;
      mAvailableNames = new List<string>();
      mAvailableBaudRates = new List<int>();
      mAvailableStopBits = new List<StopBits>();
      mAvailableStopBits.Add(StopBits.None);
      mAvailableStopBits.Add(StopBits.One);
      mAvailableStopBits.Add(StopBits.OnePointFive);
      mAvailableStopBits.Add(StopBits.Two);
      mAvailableParity = new List<Parity>();
      mAvailableParity.Add(Parity.None);
      mAvailableParity.Add(Parity.Odd);
      mAvailableParity.Add(Parity.Even);
      mAvailableParity.Add(Parity.Mark);
      mAvailableParity.Add(Parity.Space);
      this.dataReadCallback = dataReadCallback;
    }

    public void Autoconnect() {
      Refresh();

      if (!IsConnected) {
        if (AvailableNames.Count > 0) {
          Name = AvailableNames[0];
          Log.LogInfo($"Connecting automatically to port {mName}");
          ConnectOrDisconnect();
        } else {
          Log.LogInfo($"No port detected");
        }
      }
    }

    public void ConnectOrDisconnect() {
      if (IsConnected) {
        Close();
      } else {
        TryOpen();
      }
      Notify(nameof(IsConnected));
    }

    public string Name { get { return mName; } set { SetAndNotify(ref mName, value); } }
    public Int32 BaudRate { get { return mBaudRate; } set { SetAndNotify(ref mBaudRate, value); } }
    public Int32 DataBits { get { return mDataBits; } set { SetAndNotify(ref mDataBits, value); } }
    public StopBits StopBits { get { return mStopBits; } set { SetAndNotify(ref mStopBits, value); } }
    public Parity Parity { get { return mParity; } set { SetAndNotify(ref mParity, value); } }
    public List<string> AvailableNames { get { return mAvailableNames; } }
    public List<Int32> AvailableBaudRates { get { return mAvailableBaudRates; } }
    public List<StopBits> AvailableStopBits { get { return mAvailableStopBits; } }
    public List<Parity> AvailableParity { get { return mAvailableParity; } }
    public bool IsConnected { get { return port != null && port.IsOpen; } }

    public void Refresh() {
      mAvailableNames = SerialPort.GetPortNames().ToList();
      Notify(nameof(AvailableNames));
    }

    public bool TryOpen() {
      try {
        port = new SerialPort(mName, mBaudRate, mParity, mDataBits, mStopBits);
        port.NewLine = "\n";
        port.DataReceived += port_DataReceived;
        port.Open();
        return true;
      } catch (Exception ex) {
        Log.LogError($"Could not open serial port {mName}: " + ex.Message);
        return false;
      }
    }

    public void Close() {
      port.Close();
      port.DataReceived -= port_DataReceived;
      port.Dispose();
      port = null;
    }

    public void Send(string text) {
      port.WriteLine(text);
    }

    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
      dataReadCallback(port.ReadExisting());
    }
  }
}
