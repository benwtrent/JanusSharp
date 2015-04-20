using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JanusApi
{
  static public class JanusInterfaceFactory
  {
    public static IJanusClient GetNewJanusClient(String url)
    {
      if (url[0] == 'w')
        return new JanusWebSocketClient(url);
      else if (url[0] == 'h')
        return new JanusRestClient(url);
      else
        throw new Exception("Invalid URL type, must be ws://, wss://, http://, https://");
    }
  }
}
