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
      JanusRestClient client = new JanusRestClient("http://192.168.1.197:8088/janus", true);
      if (client.InitializeConnection())
        Console.WriteLine("Initialized OK");
      else
        Console.WriteLine("Not OK");
      if(!client.InitializeVideoRoomConnection())
        Console.WriteLine("Room Not OK");
      JanusVideoRoomCreationObject videoresp = client.CreateRoom(1111, "Judge: ", null, 2048000, null, false, null, 25, false);
      client.RemoveRoom(1111);
      client.CleanUp();
      client.Dispose();
      client = null;
      client = new JanusRestClient("http://192.168.1.197:8088/janus", true);

      client.InitializeConnection();
      client.InitializeVideoRoomConnection();

      JanusVideoRoomCreationObject resp = client.CreateRoom(1111);
      if (resp.error != null)
      {
        Console.WriteLine(resp.error.code);
        Console.WriteLine(resp.error.reason);
      }
      client.RemoveRoom(1111);
      client.CleanUp();
    }    
  }
}
