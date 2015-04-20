using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JanusApi
{
  class JanusWebSocketClient : IJanusClient
  {
    public JanusWebSocketClient(string url) {
      throw new NotImplementedException();
    }

    T Execute<T>(dynamic Request, JanusRequestType type)
    {
      throw new NotImplementedException();
    }

    T Execute<T>(dynamic Request, JanusRequestType type, JanusPluginType plugin)
    {
      throw new NotImplementedException();
    }

    void ClearConnectionInfo()
    {
      throw new NotImplementedException();
    }

    long GetSessionHandle()
    {
      throw new NotImplementedException();
    }

    List<JanusPluginType> GetAttachedPlugins()
    {
      throw new NotImplementedException();
    }
  }
}
