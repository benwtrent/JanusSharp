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
    static JanusRestClient client = new JanusRestClient("url");
    private static object randSyncObj = new object();
    static void Main(string[] args)
    {
      client.InitializeConnection();
      client.InitializeVideoRoomConnection();
      client.CreateRoom(100);
      JanusVideoRoomExistsResponse existResponse = client.RoomExists(100);
      JanusVideoRoomListResponse listResponse = client.ListRooms();
      client.RemoveRoom(100);
      client.CleanUp();
    }

    #region TestFunctions
    //create and destroy rooms with random names and times...could be used in numerous threads to test against janus
    public static void CreateAndDestroyRoomThread()
    {
      int wait;
      lock (randSyncObj)
      {
        wait = rand.Next(100, 500);
      }
      Thread.Sleep(wait);
      Console.WriteLine("Creating room: " + wait.ToString());
      client.CreateRoom(wait);
      Thread.Sleep(wait);
      Console.WriteLine("Removing room: " + wait.ToString());
      client.RemoveRoom(wait);
    }


    #endregion
  }
}
