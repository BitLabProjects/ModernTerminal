using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bitLab.Log
{
  public class LogMessage
  {
    internal LogMessage(string message, ELogMessageType type)
    {
      Message = message;
      Type = type;
    }

    public string Message { get; }
    public ELogMessageType Type { get; }
  }
}
