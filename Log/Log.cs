using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitLab.Log
{
  public class Log
  {
    private List<ILogListener> mListeners;

    private Log()
    {
      mListeners = new List<ILogListener>();
    }

    private static Log mInstance;
    private static Log Instance
    {
      get
      {
        if (mInstance == null)
          mInstance = new Log();
        return mInstance;
      }
    }

    public static void Register(ILogListener listener)
    {
      Instance.mListeners.Add(listener);
    }

    public static void LogInfo(string message)
    {
      LogMessage(ELogMessageType.Info, message);
    }

    public static void LogWarning(string message) {
      LogMessage(ELogMessageType.Warning, message);
    }

    public static void LogError(string message) {
      LogMessage(ELogMessageType.Error, message);
    }

    private static void LogMessage(ELogMessageType type, string message) {
      var logMessage = new LogMessage(message, type);
      foreach (var listener in Instance.mListeners)
        listener.LogMessage(logMessage);
    }
  }
}
