using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JanusApi
{
  public class JanusWebSocketClient : JanusClient
  {
    public JanusWebSocketClient(string url) {
      throw new NotImplementedException();
    }

    public override T Execute<T>(dynamic Request, JanusRequestType type)
    {
      throw new NotImplementedException();
    }

    public override T Execute<T>(dynamic Request, JanusRequestType type, JanusPluginType plugin)
    {
      throw new NotImplementedException();
    }

    public override void ClearConnectionInfo()
    {
      throw new NotImplementedException();
    }
  }
}
