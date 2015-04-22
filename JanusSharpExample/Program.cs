using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JanusApi;
using JanusApi.Model;
using System.Threading;
namespace JanusSharpExample
{
  class Program
  {
    static Random rand = new Random();
    private static object randSyncObj = new object();
    static void Main(string[] args)
    {
      JanusRestClient client = new JanusRestClient("ws://192.168.1.197:8188/janus");
        client.InitializeConnection();
      client.InitializeVideoRoomConnection();
      client.CreateRoom(1111);
      client.RemoveRoom(1111);
      client.CleanUp();
    }

    
  }
}
